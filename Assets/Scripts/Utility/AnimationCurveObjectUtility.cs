using System.Collections;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameLab.Utility
{
    public class AnimationCurveObjectUtility : MonoBehaviour
    {
        public readonly UnityEvent OnAnimationStart = new();
        public readonly UnityEvent<float> OnAnimationTick = new();
        public readonly UnityEvent OnAnimationEnd = new();

        public AnimationCurveObject animationCurve;

        public void StartAnimation()
        {
            StartCoroutine(AnimationCoroutine(0));
        }

        public void StartAnimationWithOffset(float offset)
        {
            StartCoroutine(AnimationCoroutine(offset));
        }

        public void StopAnimation()
        {
            StopAllCoroutines();
        }


        private IEnumerator AnimationCoroutine(float offset)
        {
            float duration = animationCurve.Duration;

            float durationOffset = duration * offset;

            float startTime = Time.time;
            float targetTime = Time.time + duration;

            OnAnimationStart.Invoke();

            while (Time.time + durationOffset <= targetTime)
            {
                float elapsedTime = Mathf.Clamp((Time.time + durationOffset) - startTime, 0f, duration);

                float value = animationCurve.AnimationCurve.Evaluate(elapsedTime);

                OnAnimationTick.Invoke(value);

                yield return null;
            }
            
            OnAnimationTick.Invoke(animationCurve.AnimationCurve.Evaluate(duration));
            OnAnimationEnd.Invoke();
        }
    }
}