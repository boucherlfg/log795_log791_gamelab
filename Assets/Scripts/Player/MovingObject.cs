using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using Unity.Mathematics;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace GameLab.Player
{
    public abstract class MovingObject : MonoBehaviour, IAuraReceiver, ITickable
    {
        
        protected Vector3 LastNonZeroMovement;
        protected float DashDurationCounter;
        [HideInInspector] public bool isDashing;
        [HideInInspector] public bool isInCanon;
        protected float DashCooldownCounter;
        protected float SpeedBeforeDash;
        protected RigidbodyConstraints ConstraintsBeforeDash;
        
        // MAKS
        private static readonly Vector3 MOVEMENT_MASK = new Vector3(1, 0, 1);
        private static readonly float Epsilon = 0.25f;

        public delegate void OnSetEventHandler(MovingObject obj, int playerNumber);

        public static event OnSetEventHandler OnSet;

        public bool isGettingRampBoost;

        protected Vector2 _movement = Vector3.zero;
        protected Rigidbody _rigidbody;
        private Coroutine _knockbackCoroutine;
        protected bool IsOnRamp;
        public readonly MultiLock IsInputBlocked = new();
        private VehicleData _vehicleData;
        
        [SerializeField] private bool dontSimulate;
        [SerializeField] protected InputConfig inputConfig;
        [SerializeField] protected GameObject floorPositionDecal;

        [SerializeField] protected GameObject[] robotObjects;
        [SerializeField] protected GameObject[] ballsFollower;
        [SerializeField] protected GameObject ballEffect;
        [SerializeField] protected GameObject wheel;


        [SerializeField] private Collider collisionCollider;
        [Header("SFX")]
        [SerializeField] private StudioEventEmitter rampSfx;

        [Header("VFX")] 
        [SerializeField] protected GameObject dashVFXBlue;
        [SerializeField] protected GameObject dashVFXRed;
        /// <summary>
        /// will be a root object
        /// </summary>
        protected VisualEffect DashTarget;
        /// <summary>
        /// will be parented by player
        /// </summary>
        protected Transform DashTrail;
        
        public bool IsRunNotStarted
        {
            get => IsInputBlocked[nameof(IsRunNotStarted)];
            set => IsInputBlocked[nameof(IsRunNotStarted)] = value;
        }

        public bool IsKnockedBack
        {
            get => IsInputBlocked[nameof(IsKnockedBack)];
            set => IsInputBlocked[nameof(IsKnockedBack)] = value;
        }

        public bool IsOutOfEnergy
        {
            get => IsInputBlocked[nameof(IsOutOfEnergy)];
            set => IsInputBlocked[nameof(IsOutOfEnergy)] = value;
        }


        public float MaxSpeed { get; set; }


        public Vector3 MovementSpeed => Vector3.Scale(_rigidbody.linearVelocity, MOVEMENT_MASK);

        public List<AbstractBonusEffector> BonusEffectors { get; set; } = new();

        public bool IsOnGround
        {
            get;
            protected set;
        }

        protected virtual void Start()
        {
            IsInputBlocked[nameof(dontSimulate)] = dontSimulate;
            if (dontSimulate) return;

            MaxSpeed = inputConfig.MaxSpeed;


            _vehicleData = GetComponent<VehicleData>();
            if (_vehicleData)
                GameEvents.MovingObjects.TryAdd(_vehicleData.VehicleId, new MovingObjectEvent());
        }

        protected virtual void OnDestroy()
        {
            if (DashTarget)
            {
                DashTarget.Stop();
                Destroy(DashTarget.gameObject);
            }

            if (DashTrail)
            {
                Destroy(DashTrail.gameObject);
            }
        }

        protected void FixedUpdate()
        {
            Quaternion rotation = ballEffect.transform.rotation;

            if (_vehicleData)
                GameEvents.MovingObjects[_vehicleData.VehicleId].PositionChanged.Invoke(transform.position, rotation, isDashing);
        }

        protected void OnMoveEvent(Vector2 direction)
        {
            _movement = direction;
            if (direction.magnitude > 0.1f)
            {
                LastNonZeroMovement = new Vector3(direction.x, 0, direction.y).normalized;
            }
        }

        public virtual void CanonGrab()
        {
            if (floorPositionDecal)
            {
                floorPositionDecal.SetActive(false);
            }

            isInCanon = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _rigidbody.linearVelocity = Vector3.zero;
        }

        public virtual void ExternalImpulsiveForceAffect(Transform dir, float force, bool isFromCanon = false)
        {
            if (floorPositionDecal)
                floorPositionDecal.SetActive(true);

            if (isFromCanon)
            {
                ColliderDeactivationTemporary();
                isInCanon = false;
            }

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.AddForce(dir.forward.normalized * force, ForceMode.Impulse);
        }

        public void ColliderDeactivationTemporary()
        {
            StartCoroutine(SmallColliderDeactivation());
        }

        public void DeactivateCollider()
        {
            collisionCollider.enabled = false;
        }

        private IEnumerator SmallColliderDeactivation()
        {
            collisionCollider.enabled = false;
            yield return new WaitForSeconds(0.1f);
            collisionCollider.enabled = true;

        }

        public virtual void KnockBackWithDelay(float delay)
        {
            if (_knockbackCoroutine is not null)
            {
                StopCoroutine(_knockbackCoroutine);
            }

            _knockbackCoroutine = StartCoroutine(KnockBackWithDelayCoroutine(delay));
        }

        private IEnumerator KnockBackWithDelayCoroutine(float delay)
        {
            IsKnockedBack = true;
            yield return new WaitForSeconds(delay);
            IsKnockedBack = false;
        }

        public void Receive()
        {
            foreach (var bonusConfig in BonusEffectors)
            {
                bonusConfig.Affect(this);
            }
        }

        public void StartAffecting(AbstractBonusEffector effector)
        {
            effector.StartAffecting(this);
        }

        public void StopAffecting(AbstractBonusEffector effector)
        {
            effector.StopAffecting(this);
        }

        protected void InvokeOnSet(int playerNumber)
        {
            OnSet?.Invoke(this, playerNumber);
        }

        protected void ApplyGravity()
        {
            _rigidbody.AddForce(Vector3.down * (inputConfig.GravityAcceleration), ForceMode.Acceleration);
            // ClampLinearVelocity();
        }

        protected void UpdateVehicle(Vector2 movement, bool isAccelerating, bool isOnGround, bool isOnRamp = false)
        {
            switch (inputConfig.InputType)
            {
                case InputType.Directional:
                    Directional(movement, isAccelerating, isOnGround, isOnRamp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void Directional(Vector2 movement, bool isAccelerating, bool isOnGround, bool isOnRamp)
        {
            Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;
            if (isOnGround)
            {
                HandleDirectionalGroundMovement(direction, isAccelerating, isOnRamp);
            }
            else
            {
                HandleDirectionnalAirMovement(direction, isAccelerating, isOnRamp);
            }
        }

        private void HandleDirectionalGroundMovement(Vector3 direction, bool isAccelerating, bool isOnRamp)
        {
            // handle acceleration
            if (isAccelerating)
            {
                HandleAcceleration(inputConfig.Acceleration, inputConfig.Decceleration, direction, isOnRamp);
                float angle = Vector3.SignedAngle(MovementSpeed, direction, Vector3.up);

                // IF THE ANGLE BETWEEN OUR CURRENT DIRECTION AND THE INPUT IS GREATED THAN A VALUE
                if (Mathf.Abs(angle) > inputConfig.SharpRotationAngle && inputConfig.UseSharpRotation)
                {
                    // APPLY A FORCE ON THE SIDE AND A DECCELERATION TO MAKE THE TURN SHARPER
                    Vector3 side = transform.right * Mathf.Sign(angle);
                    _rigidbody.AddForce(side * inputConfig.Acceleration, ForceMode.Acceleration);
                    if (_rigidbody.linearVelocity.magnitude > 2)
                    {
                        _rigidbody.AddForce(-MovementSpeed.normalized * inputConfig.Acceleration, ForceMode.Acceleration);
                    }
                }
            }
            else
            {
                HandleDecceleration(inputConfig.Decceleration);
            }
        }

        protected void PrepareDashVFX(PlayerNumber player)
        {
            var instance = player switch
            {
                PlayerNumber.One => Instantiate(dashVFXRed),
                PlayerNumber.Two => Instantiate(dashVFXBlue),
            };
            if (!instance.TryGetComponent(out DashTarget)) throw new Exception("No visual effect found on dash vfx");
            
            DashTrail = DashTarget.transform.GetChild(0);
            DashTrail.transform.SetParent(transform);
            DashTrail.localPosition = Vector3.zero;
            
            DashTarget.Stop();
        }
        public void ToggleDashVfx(bool isEnabled)
        {
            if (!DashTarget)
            {
                return;
            }
            if (isEnabled)
            {
                DashTarget.Reinit();
                DashTarget.Play();
            }
            else
            {
                DashTarget.Stop();
            }
        }

        private void HandleDirectionnalAirMovement(Vector3 movement, bool isAccelerating, bool isOnRamp)
        {
            if (isAccelerating)
            {
                HandleAcceleration(inputConfig.AccelerationInAir, inputConfig.DeccelerationAir, movement, isOnRamp);
            }
            else
            {
                HandleDecceleration(inputConfig.DeccelerationAir);
            }
        }

        private void HandleAcceleration(float acceleration, float decceleration, Vector3 direction, bool isOnRamp)
        {
            if (MovementSpeed.magnitude > inputConfig.MaxSpeed)
            {
                float currentVelocity = MovementSpeed.magnitude;


                _rigidbody.AddForce((-MovementSpeed).normalized * Mathf.Min((currentVelocity - inputConfig.MaxSpeed) * inputConfig.DeccelerationOverMaxSpeedMultiplier, inputConfig.DeccelerationOverMaxSpeed),
                    ForceMode
                    .Acceleration);
            }

            float rampFactor = (isOnRamp ? inputConfig.RampVelBonus : 1f);
            if (isOnRamp)
            {
                rampSfx.PlayWithTryCatch();
            }


            // USE USER CONTROL FOR DIRECTION AND APPLY AN ACCELERATION
            _rigidbody.AddForce(direction * (acceleration * rampFactor), ForceMode.Acceleration);
        }

        private void HandleDecceleration(float decceleration)
        {
            // handle deceleration
            // FORCE VELOCITY TO 0 WHEN NEAR 0
            if (_rigidbody.linearVelocity.magnitude < Epsilon)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                return;
            }
            // APPLY A DECCELERATION FORCE TO THE OPPOSITE DIRECTION TO STOP
            _rigidbody.AddForce((-MovementSpeed).normalized * decceleration, ForceMode.Acceleration);
        }

        protected void UpdateRotationBall()
        {
            if (MovementSpeed.magnitude < 1)
            {
                return;
            }

            Quaternion currentRotation = ballsFollower[0].transform.rotation;
            quaternion newRotation = Quaternion.LookRotation(MovementSpeed.normalized);

            float torque;

            // ON GROUND
            if (IsOnGround)
            {
                torque = inputConfig.TorqueBallGround.AnimationCurve.Evaluate(Mathf.Clamp(MovementSpeed.magnitude, inputConfig.TorqueBallGround.MinX, inputConfig.TorqueBallGround.MaxX));
            }
            // IN AIR
            else
            {
                torque = inputConfig.TorqueBallAir.AnimationCurve.Evaluate(Mathf.Clamp(MovementSpeed.magnitude, inputConfig.TorqueBallAir.MinX, inputConfig
                    .TorqueBallAir.MaxX));
            }

            Quaternion yRotation = Quaternion.RotateTowards(currentRotation, newRotation, torque);
            // PRESERVE X ROTATION
            foreach (var ballFollower in ballsFollower)
            {
                ballFollower.transform.rotation = yRotation;
            }

            if (ballEffect && IsOnGround && !isDashing)
            {
                float ratio = Mathf.Clamp(MovementSpeed.magnitude / MaxSpeed, 0, 1);
                float angle = inputConfig.BallRotationSpeed * ratio;

                // Vector3 axis = wheel.transform.right;
                Vector3 forward = _movement.magnitude < Single.Epsilon ? robotObjects[0].transform.forward : new Vector3(_movement.x, 0, _movement.y).normalized;
                Vector3 up = Vector3.up;
                Vector3 axis = Vector3.Cross(up, forward);

                ballEffect.transform.Rotate(axis, angle, Space.World);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawRay(wheel.transform.position, wheel.transform.right * 2);
        }

        protected void UpdateRotationRobot()
        {
            if (_movement.magnitude < Single.Epsilon || robotObjects.Length == 0)
            {
                return;
            }

            Vector3 currentVelocity = MovementSpeed;
            if (currentVelocity.magnitude < 1)
            {
                return;
            }

            Quaternion currentRotation = robotObjects[0].transform.rotation;
            quaternion newRotation = Quaternion.LookRotation(new Vector3(_movement.x, 0, _movement.y).normalized);

            Quaternion rotation = Quaternion.RotateTowards(currentRotation, newRotation, inputConfig.TorqueRobot);

            foreach (GameObject robotObject in robotObjects)
            {
                robotObject.transform.rotation = rotation;
            }
        }

        public int Priority
        {
            get => 1;
        }

        public virtual void Tick(float time)
        {}

        public virtual void Reset()
        {}
    }
}