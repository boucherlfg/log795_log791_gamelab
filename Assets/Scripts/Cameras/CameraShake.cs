using System;
using System.Collections;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using Unity.Cinemachine;
using UnityEngine;


namespace GameLab.Cameras

{
    public class CameraShake : MonoBehaviour

    {
        [SerializeField] private CinemachineBasicMultiChannelPerlin shakerComponent;
        [SerializeField] private ScreenShakeConfig screenShakeConfig;

        private Coroutine _currentShake;


        // Start is called once before the first execution of Update after the MonoBehaviour is created

        void Start()

        {
            CameraEvents.CameraShakeEvent.AddListener(OnCameraShake);
            RoundEvents.RunEnded.AddListener(OnRoundEnded);
        }

        private void OnRoundEnded()
        {
            shakerComponent.NoiseProfile = null;
            _currentShake = null;
        }


        private void OnCameraShake(EScreenShakeSource cameraShakeSource)
        {
            if (!shakerComponent)
            {
                return;
            }


            ScreenShakeConfig.ScreenShakeTuple screenShakeTuple;
            if (!screenShakeConfig.TryGetConfig(cameraShakeSource, out screenShakeTuple))
            {
                return;
            }


            if (_currentShake != null)
            {
                StopCoroutine(_currentShake);
            }


            shakerComponent.FrequencyGain = 1;
            shakerComponent.AmplitudeGain = 1;
            shakerComponent.NoiseProfile = screenShakeTuple.config;
            _currentShake = StartCoroutine(StopShakeAfterDelay(screenShakeTuple.duration));
        }


        IEnumerator StopShakeAfterDelay(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            shakerComponent.NoiseProfile = null;
            _currentShake = null;
        }


        private void OnDestroy()
        {
            CameraEvents.CameraShakeEvent.RemoveListener(OnCameraShake);
            RoundEvents.RunEnded.RemoveListener(OnRoundEnded);
        }
    }
}