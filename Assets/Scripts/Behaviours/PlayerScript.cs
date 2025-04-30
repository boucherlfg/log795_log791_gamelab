using System;
using System.Collections;
using FMODUnity;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace GameLab.Behaviours
{
    public class PlayerScript : MovingObject
    {
        private static readonly int EmissiveColor = Shader.PropertyToID("_Emissive_Color");
        private static readonly int EmissionIntensity = Shader.PropertyToID("_Emission_Intensity");
        private static readonly int BaseColorTeint = Shader.PropertyToID("_Base_Color_Teint");
        private static readonly int EmissionTeint = Shader.PropertyToID("_Emission_Teint");
        private static readonly int RotationAnim = Shader.PropertyToID("_Rotation_Anim");
        private static readonly int RadialStartAngle = Shader.PropertyToID("_Radial_Start_Angle");
        public static event EventHandler<PlayerScript> onSpawnedTrigget;
        public static void ClearStaticVariables()
        {
            onSpawnedTrigget = null;
        }

        [SerializeField] private float decalAngle;
        private bool _canDash = true;
        private float _initialDecalIntensity;
        private bool _fromLobby;
        private bool _isAccelerating = false;
        private EnergyComponent _energyComponent;
        private int _dashChargesLeft;
        // private readonly Vector3 _offsetDecal = new Vector3(0, 0.01f, 0);

        // [SerializeField] private GameObject decal;

        [SerializeField] protected GameObject[] heads;
        [SerializeField] private PlayerConfigs playerConfigs;

        [Header("SFX")]
        [SerializeField] private StudioEventEmitter playerMovementSounds;
        [SerializeField] private DecalProjector selectorMaterial;
        [SerializeField] private StudioEventEmitter dashSFX;

        public GameObject[] Heads
        {
            get => heads;
        }

        protected override void Start()
        {
            base.Start();
            ConstraintsBeforeDash = _rigidbody.constraints;
            _initialDecalIntensity = selectorMaterial.material.GetFloat(EmissionIntensity);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            VehiculeEvent.OnOutOfEnergy.RemoveListener(OnOutOfEnergy);
            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
            if (!GameEvents.Players.TryGetValue(Id, out var player)) return;
            player.Move.RemoveListener(OnMoveEvent);
            player.Accelerate.RemoveListener(OnAccelerateEvent);
            playerMovementSounds.Stop();
            selectorMaterial.material.SetFloat(EmissionIntensity, _initialDecalIntensity);
            selectorMaterial.material.SetFloat(RotationAnim, 1);
            
        }
        
        private void OnRunStarted()
        {
            IsRunNotStarted = false;
            selectorMaterial.material = Instantiate(selectorMaterial.material);
            //var angleOffset = _fromLobby ? 0 : Mathf.PI;
        }

        private void OnAccelerateEvent(bool isAccelerating)
        {
            _isAccelerating = isAccelerating;
        }
        
        private void OnOutOfEnergy(bool isPlayer)
        {
            if (!isPlayer) return;
            _movement = Vector3.zero;
            _isAccelerating = false;
            IsOutOfEnergy = true;
        }

        public void SetPlayer(int playerId, bool fromLobby = false)
        {
            _dashChargesLeft = inputConfig.DashCharges;
            PrepareDashVFX((PlayerNumber)playerId);
            _fromLobby = fromLobby;
            Id = playerId;
            
            GameEvents.Players.TryAdd(playerId, new PlayerEvent());
            GameEvents.Players[Id].Move.AddListener(OnMoveEvent);
            GameEvents.Players[Id].Accelerate.AddListener(OnAccelerateEvent);
            GameEvents.Players[Id].Dash.AddListener(OnDashEvent);
            DashCooldownCounter = inputConfig.DashCooldown;
            _rigidbody = GetComponent<Rigidbody>();
            _movement = Vector2.zero;
            _energyComponent = GetComponent<EnergyComponent>();
            VehiculeEvent.OnOutOfEnergy.AddListener(OnOutOfEnergy);
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            IsRunNotStarted = true;
            onSpawnedTrigget?.Invoke(this, this);
            InvokeOnSet(playerId);
            ballEffect.GetComponent<Renderer>().material.SetColor(EmissiveColor, playerConfigs.PlayerColors[playerId - 1]);
        }

        private void OnDashEvent()
        {
            if (isInCanon) return;
            if (!_canDash) return;

            if (isDashing) return;

            if (_dashChargesLeft <= 0) return;

            if (IsInputBlocked) return;

            if (!inputConfig.CanDashInAir && !IsOnGround) return;

            _canDash = false;
            SpeedBeforeDash = _rigidbody.linearVelocity.magnitude;
            ToggleDashVfx(true);
            _dashChargesLeft--;
            GamepadEvents.CallVibration.Invoke((PlayerNumber)Id, VibrationSource.DashStart);
            isDashing = true;
            var dashDirectionFactor = inputConfig.DashDirection / 90;
            
            // change values for dash values
            _rigidbody.AddForce(inputConfig.DashInitialImpulse * Vector3.Slerp(GetDashDirection(), Vector3.up, dashDirectionFactor), ForceMode.Impulse);
            dashSFX.PlayWithTryCatch();
            if(!IsOnRamp) _rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
            DashCooldownCounter = 0;
            DashDurationCounter = 0;
        }

        public void ResetConstraints() => _rigidbody.constraints = ConstraintsBeforeDash;

        Vector3 GetDashDirection()
        {
            if (_movement.magnitude > 0.1f)
                return new Vector3(_movement.x, 0, _movement.y).normalized;
            if (LastNonZeroMovement.magnitude > 0.1f)
                return LastNonZeroMovement.normalized;
            if (robotObjects.Length > 0) 
                return robotObjects[0].transform.forward;

            return wheel.transform.forward;
        }

        public void SetVisualIndicator(Material decalSelection, float emissionItensity, Color emissionTeint)
        {
            selectorMaterial.material = decalSelection;
            selectorMaterial.material.SetFloat(EmissionIntensity, emissionItensity);
            selectorMaterial.material.SetColor(BaseColorTeint, Color.white);
            selectorMaterial.material.SetColor(EmissionTeint, emissionTeint);
        }
        
        public int Id { get; private set; } = -1;

        private void FixedUpdate()
        {
            var selectorRotation = selectorMaterial.transform.rotation.eulerAngles;
            selectorMaterial.transform.rotation = Quaternion.Euler(selectorRotation.x, 0, selectorRotation.z);
            selectorMaterial.material.SetFloat(RadialStartAngle, decalAngle);
            base.FixedUpdate();
            if (!_fromLobby) return;
            Tick(Time.fixedTime);
        }

        private void Awake()
        {
            playerMovementSounds.PlayWithTryCatch(); // TO UPDATE IN START
        }

        public override void Tick(float time)
        {
            HandleDashTime();

            base.Tick(time);

            if (!isDashing && !_canDash)
            {
                DashCooldownCounter += Time.fixedDeltaTime;
                if (DashCooldownCounter > inputConfig.DashCooldown)
                {
                    _canDash = true;
                    GamepadEvents.CallVibration.Invoke((PlayerNumber)Id, VibrationSource.DashCooldownDone);
                }
                
                var intensity = DashCooldownCounter < inputConfig.DashCooldown ? _initialDecalIntensity / 10 : _initialDecalIntensity;
                selectorMaterial.material.SetFloat(EmissionIntensity, intensity);
                var clamped = DashCooldownCounter / inputConfig.DashCooldown;
                clamped = clamped * 2 - 1;
                selectorMaterial.material.SetFloat(RotationAnim, clamped);
            }

            
            // APPLY GRAVITY
            RaycastHit raycastHit;
            bool isDetecting = Physics.Raycast(transform.position, Vector3.down, out raycastHit,  0.9f * transform.localScale.y);
            IsOnGround = isDetecting && (raycastHit.transform.CompareTag("Floor") || raycastHit.transform.CompareTag("Rampe") || raycastHit.transform
                .CompareTag("Wall"));
            IsOnRamp = isDetecting && raycastHit.transform.CompareTag("Rampe");
            
            if(IsOnRamp) HandleDashingStopped();

            ApplyGravity();

            // decal.transform.position = decalPosition + _offsetDecal;
            // APPLY MOVEMENT
            _energyComponent.SetIsAccelerating(_isAccelerating);
            if (!IsInputBlocked)
            {
                UpdateVehicle(_movement, _isAccelerating, IsOnGround, isGettingRampBoost);
                // ROTATE
            }
            else
            {
                UpdateVehicle(Vector3.zero, false, IsOnGround, isGettingRampBoost);
            }
            UpdateRotationBall();
            UpdateRotationRobot();
            playerMovementSounds.SetParameter("PLAYER_VELOCITY", Mathf.Clamp(MovementSpeed.magnitude, 0f, 30f));
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!inputConfig) return;
            var direction = Vector3.Slerp(transform.forward, transform.up, inputConfig.DashDirection/90);
            Gizmos.DrawRay(transform.position, direction * 2);
        }
        
        private void HandleDashTime()
        {
            if (!isDashing) return;
            DashDurationCounter += Time.fixedDeltaTime;
            if (DashDurationCounter < inputConfig.DashDuration) return;
            _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, inputConfig.MaxSpeed);
            HandleDashingStopped();
        }

        public override void CanonGrab()
        {
            if(isDashing) HandleDashingStopped();
            base.CanonGrab();
        }

        public override void KnockBackWithDelay(float delay)
        {
            if(isDashing) HandleDashingStopped();
            base.KnockBackWithDelay(delay);
        }
        
        public override void ExternalImpulsiveForceAffect(Transform dir, float force, bool isFromCanon = false)
        {
            if(isDashing) HandleDashingStopped();
            base.ExternalImpulsiveForceAffect(dir, force, isFromCanon);
        }
        public void HandleDashingStopped()
        {
            _rigidbody.constraints = ConstraintsBeforeDash;
            isDashing = false;
            ToggleDashVfx(false);
        }

    }
}