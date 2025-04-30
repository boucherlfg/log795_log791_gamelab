using System;
using GameLab.Events;
using UnityEngine;

namespace GameLab.Managers
{
    [DefaultExecutionOrder(-10)]
    public class PauseManager : MonoBehaviour
    {
        private bool _canPause = false;
        private bool _paused = false;
        private void Start()
        {
            GameEvents.GameStarted.AddListener(OnGameStarted);
            GameEvents.GameEnded.AddListener(OnGameEnded);
            RoundEvents.RunStarted.AddListener(OnRoundStarted);
            RoundEvents.RoundIsEnding.AddListener(OnRoundEnded);
            _paused = false;
            Time.timeScale = 1;
        }

        private void OnRoundStarted()
        {
            _canPause = true;
        }

        private void OnRoundEnded()
        {
            _canPause = false;
        }

        private void OnDestroy()
        {
            GameEvents.PauseInputTriggered.RemoveListener(OnPause);
            GameEvents.GameStarted.RemoveListener(OnGameStarted);
            GameEvents.GameEnded.RemoveListener(OnGameEnded);
            Time.timeScale = 1;
        }

        private void OnGameStarted()
        {
            GameEvents.PauseInputTriggered.AddListener(OnPause);
        }
        
        private void OnGameEnded()
        {
            GameEvents.PauseInputTriggered.RemoveListener(OnPause);
        }

        private void OnPause()
        {
            if (!_canPause) return;
            _paused = !_paused;
            Time.timeScale = _paused ? 0 : 1;
            GameEvents.GamePaused.Invoke(_paused);
        }
    }
}