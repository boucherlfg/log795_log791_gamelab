using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Player;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Constructions.Canons
{
    public class CanonConstruct : StateBehavior, IAuraReceiver, ITickable
    {
        private static readonly int IsFlashingId = Shader.PropertyToID("_Is_Flashing");
        internal static string PlayerTag = "Player";
        internal static string ShadowTag = "Shadow";
        
        internal event EventHandler<MovingObject> OnObjectCollideWithCatchRadius;
        
        public readonly UnityEvent OnCanonShoot = new();
        public readonly UnityEvent<PlayerNumber> OnCanonTaken = new();

        private const string EmissiveIntensity = "_Emissive_Intensity";

        // SET BY THE AURA MANAGER
        public List<AbstractBonusEffector> BonusEffectors { get; set; } = new();
        internal CanonAwaitState _awaitState;

        [Header("Line")] 
        [SerializeField] internal Renderer lineRenderer;
        [SerializeField] internal float lineAwait = 0f;
        [SerializeField] internal float lineLoading = 10f;
        [SerializeField] internal float lineCooldown = -1f;

        [Header("Shot")] 
        [SerializeField] internal Transform posInCanon;
        [SerializeField] private float timeInCannon = 1f;
        [SerializeField] internal float cooldownAfterShoot = 1f;
        [SerializeField] internal float cannonForce = 10f;
        
        [Header("VFX")] 
        [SerializeField] internal GameObject visualEffect;

        [Header("SFX")] 
        [SerializeField] internal StudioEventEmitter canonChargeSfx;
        [SerializeField] internal StudioEventEmitter canonShootSfx;

        internal MovingObject ObjectHeld;
        private bool _hasCanonBeenAffected = false;

        internal List<MovingObject> inRange = new ();

        public float TimeInCanon
        {
            get => timeInCannon;
            set => timeInCannon = value;
        }


        protected override void Start()
        {
            visualEffect.SetActive(false);

            base.Start();

            _awaitState = new CanonAwaitState(this);
            CanonPlayerLoadingState playerLoadingState = new CanonPlayerLoadingState(this);
            CanonShootCooldownState shootCooldownState = new CanonShootCooldownState(this);
            CanonShootReloadState shootReloadState = new CanonShootReloadState(this);
            CanonShadowHoldingState shadowHoldingState = new CanonShadowHoldingState(this);

            _awaitState.OnObjectEntered += (_, _) => changeState(ObjectHeld is ShadowScript ? shadowHoldingState : playerLoadingState);
            playerLoadingState.OnReadyToShootCooldown += (_, _) => changeState(shootCooldownState);
            playerLoadingState.OnReadyToShootReplace += (_, movingObject) =>
            {
                shootReloadState.SetMovingObject(movingObject);
                changeState(shootReloadState);
            };
            shootCooldownState.OnCooldownOver += (_, _) => changeState(_awaitState);
            shootReloadState.OnReload += (_, movingObject) =>
            {
                ObjectHeld = movingObject;
                changeState(playerLoadingState);
            };
            shadowHoldingState.OnShadowLeft += (_, _) => changeState(_awaitState);

            changeState(_awaitState);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out MovingObject movingObject))
            {
                inRange.Add(movingObject);
                OnObjectCollideWithCatchRadius?.Invoke(this, movingObject);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out MovingObject movingObject))
                inRange.Remove(movingObject);
        }

        internal void SetLineRendererRation(float ratio)
        {
            lineRenderer.material.SetFloat(EmissiveIntensity, ratio);
        }

        internal void SetCanonFlashVFX(bool active)
        {
            lineRenderer.material.SetFloat( IsFlashingId, active ? 1 : 0);
        }


        internal void ActiveVFX()
        {
            StartCoroutine(StartVFX());
        }

        private IEnumerator StartVFX()
        {
            visualEffect.SetActive(true);
            yield return new WaitForSeconds(1f);
            visualEffect.SetActive(false);
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
            if (_hasCanonBeenAffected)
            {
                return;
            }

            _hasCanonBeenAffected = true;
            effector.StartAffecting(this);
        }

        public void StopAffecting(AbstractBonusEffector effector)
        {
            effector.StopAffecting(this);
        }

        public int Priority
        {
            get => 1;
        }

        public void Tick(float time)
        {
        }

        public void Reset()
        {
            inRange.Clear();
            StopAllCoroutines();
            ObjectHeld = null;
            visualEffect.SetActive(false);
            SetCanonFlashVFX(false);
            changeState(_awaitState);
            canonChargeSfx.Stop();
            canonShootSfx.Stop();
        }
    }
}