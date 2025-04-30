using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameLab
{
    public class PinBotAnimationSelector : MonoBehaviour
    {
        [Serializable]
        public struct AnimEmitterTuple
        {
            public string TriggerName;
            public StudioEventEmitter SoundEmitter;
        }

        [SerializeField] private Animator animator;
        [SerializeField] private float initialDelay;
        [SerializeField] private float minAnimationDelay;
        [SerializeField] private float animRandomDelta;
        [SerializeField] private List<AnimEmitterTuple> _emitterTuples;

        private AnimatorControllerParameter[] _triggers;
        private int _lastIndex = -1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _triggers = animator.parameters;
            StartCoroutine(nameof(PlayInitialAnimation));
        }

        private void OnDestroy()
        {
            StopCoroutine(nameof(PlayInitialAnimation));
            StopCoroutine(nameof(Animate));
        }

        private IEnumerator PlayInitialAnimation()
        {
            yield return new WaitForSeconds(initialDelay);
            PlayRandomAnim();
            StartCoroutine(nameof(Animate));
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                float waitTime = minAnimationDelay + Random.Range(0, animRandomDelta);
                yield return new WaitForSeconds(waitTime);
                PlayRandomAnim();
            }
        }

        private void PlayRandomAnim()
        {
            int triggerIndex;
            // AVOID TO REPLAY TWICE THE SAME ANIM
            do
            {
                triggerIndex = Random.Range(0, _triggers.Length);
            } while (_lastIndex == triggerIndex);

            animator.SetTrigger(_triggers[triggerIndex].name);
            _emitterTuples.Find(tuple => tuple.TriggerName == _triggers[triggerIndex].name).SoundEmitter.PlayWithTryCatch();
        }
    }
}
