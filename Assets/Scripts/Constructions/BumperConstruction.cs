using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.VFX;

namespace GameLab.Constructions
{
    public class BumperConstruction : MonoBehaviour, IAuraReceiver
    {

        [SerializeField] private AnimationCurveObject bumperForce;
        [SerializeField] private AnimationCurveObject inputDisableDelay;
        [SerializeField] private Transform bumperHeight;

        [Header("SFX")]
        [SerializeField] private StudioEventEmitter bumperSfx;
        public List<AbstractBonusEffector> BonusEffectors { get; set; } = new();

        private Animator _animator;
        private bool _hasCanonBeenAffected = false;
        private Coroutine _animCoroutine = null;
        private Coroutine _rippleCoroutine = null;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.transform.CompareTag("Player") && !other.transform.CompareTag("Shadow"))
            {
                return;
            }

            if(other.gameObject.TryGetComponent(out PlayerScript player))
            {
                GamepadEvents.CallVibration.Invoke((PlayerNumber)player.Id, VibrationSource.Bumper);
            }

            Rigidbody rb = other.rigidbody;
            MovingObject knockable = other.transform.gameObject.GetComponent<MovingObject>();

            Vector3 direction = other.transform.position - this.transform.position;


            if (other.transform.position.y < bumperHeight.position.y)
            {
                direction.y = 0;
            }
            direction = direction.normalized;

            float impulsionStrength = bumperForce.AnimationCurve.Evaluate(
                Mathf.Clamp(
                    rb.linearVelocity.magnitude,
                    bumperForce.AnimationCurve.keys[0].time,
                    bumperForce.AnimationCurve.keys[^1].time)
            );
            Debug.Assert(knockable != null, "knockable shouldn't be able to be null here");
            knockable.KnockBackWithDelay(
                inputDisableDelay.AnimationCurve.Evaluate(Mathf.Clamp(rb.linearVelocity.magnitude,
                    inputDisableDelay.AnimationCurve.keys[0].time,
                    inputDisableDelay.AnimationCurve.keys[^1].time)
                )
            );
            rb.AddForce(direction * impulsionStrength, ForceMode.Impulse);
            bumperSfx.SetParameter("BUMPER_BOUNCE_CURVE", Mathf.Clamp(impulsionStrength, 5, 15));
            bumperSfx.PlayWithTryCatch();
            
            if (_animator)
            {
                _animator.Play("Animated", -1, 0);
            }
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
    }
}