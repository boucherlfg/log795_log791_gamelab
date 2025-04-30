using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GameLab.Core;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;
using GameLab.Utility;
using UnityEngine.Serialization;
using GameLab.Enums;

namespace GameLab.Managers
{
    public class GameModeManager : MonoBehaviour
    { 
        private GameModeState _gameModeState = GameModeState.Initialization;
        private int _currentRound = 0;
        
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Transform goal;

        [Header("SFX")]
        [SerializeField] private StudioEventEmitter pauseSfx;
        [SerializeField] private StudioEventEmitter resumeSfx;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            GameEvents.GameEnded.AddListener(OnGameEnded);
            GameEvents.GamePaused.AddListener(OnGamePaused);
            Time.timeScale = 1f;
            
            Initialize();
        }

        private void OnDestroy()
        {
            GameEvents.GameEnded.RemoveListener(OnGameEnded);
            GameEvents.GamePaused.RemoveListener(OnGamePaused);
            GameEvents.RoundEnded.RemoveListener(OnRoundEnded);
        }

        void Initialize()
        {
            var initializables = Extensions.FindComponents<IInitializable>();
            foreach (var initializable in initializables)
            {
                initializable.OnInitialize();
            }
            
            _gameModeState = GameModeState.Ready;
            OnGameInitialized();
        }

        private void OnGameInitialized()
        {
            _gameModeState = GameModeState.GameStarted;
            
            var startables = Extensions.FindComponents<IStartable>();
            foreach (var startable in startables)
            {
                startable.OnStart();
            }
            
            levelManager.PrepareLevel(gameConfig);
            roundManager.InitializeRound(_currentRound, gameConfig);
            GameEvents.RoundEnded.AddListener(OnRoundEnded);
            GameEvents.GameStarted.Invoke();
        }

        private void OnRoundEnded()
        {
            GameEvents.RoundEnded.RemoveListener(OnRoundEnded);
            _currentRound++;
            
            if (_currentRound >= roundManager.RoundCount)
            {
                GameEvents.GameEnded.Invoke();
                return;
            }
            roundManager.InitializeRound(_currentRound, gameConfig);
            GameEvents.RoundEnded.AddListener(OnRoundEnded);
        }
        private void OnGameEnded()
        {
            _gameModeState = GameModeState.GameOver;
        }
        
        private void OnGamePaused(bool isPaused)
        {
            _gameModeState = isPaused ? GameModeState.GamePaused : GameModeState.GameStarted;
            if (_gameModeState == GameModeState.GamePaused)
            {
                pauseSfx.PlayWithTryCatch();
            }
            else if (_gameModeState == GameModeState.GameStarted)
            {
                resumeSfx.PlayWithTryCatch();
            }
        }
    }
}
