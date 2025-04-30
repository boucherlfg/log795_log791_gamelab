using System;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Cameras;
using GameLab.Core;
using GameLab.Events;
using GameLab.Player;
using GameLab.Enums;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Managers
{
    public class RoundManager : MonoBehaviour
    {
        private int _currentRound;
        private PlayerNumber _currentPlayer;
        private Transform _goal;
        private GameConfig _config;

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private ShadowManager shadowManager;
        [SerializeField] private GameCameraManager gameCameraManager;

        [Header("SFX")]
        [SerializeField] private StudioEventEmitter chronoStartSfx;
        [SerializeField] private StudioEventEmitter chronoEndSfx;

        private int _currentVehicleCount;
        private bool _canPlayChronoEndSound = true;

        private void OnDestroy()
        {
            gameCameraManager.OnDollyAnimationDone.RemoveListener(OnDollyAnimationDone);
            VehiculeEvent.OnOutOfEnergy.RemoveListener(OnOutOfEnergy);
            GameModeTimer.Instance.OnTimerDone.RemoveListener(OnStartTimerDone);
            GameEvents.GamePaused.RemoveListener(OnTimerPause);
        }

        private void OnTimerPause(bool isPaused)
        {
            chronoEndSfx.EventInstance.setPaused(isPaused);
        }

        public int RoundCount
        {
            get
            {
                if (!_config)
                {
                    return levelManager.RoundCount;
                }

                if(_config.RoundCount < levelManager.RoundCount)
                {
                    return _config.RoundCount;
                }
                
                return levelManager.RoundCount; 
            }
        }

        private void Start()
        {
            RoundEvents.EnergyRemaining.AddListener(OnEnergyRemaining);
            GameEvents.GamePaused.AddListener(OnTimerPause);
        }

        private void OnEnergyRemaining(float timeLeft)
        {
            if (timeLeft <= 3 && _canPlayChronoEndSound)
            {
                chronoEndSfx.PlayWithTryCatch();
                _canPlayChronoEndSound = false;
            }
        }

        public void InitializeRound(int round, GameConfig config)
        {
            _currentRound = round;
            GameObject goal = GameObject.FindGameObjectWithTag("Goal");
            if (goal)
            {
                _goal = goal.transform;
            }
            _config = config;
            InitializeRun();
            RoundEvents.RoundInitialized.Invoke();
            _canPlayChronoEndSound = true;
        }

        private void InitializeRun()
        {
            levelManager.ResetSpawners();
            var player1 = levelManager.PreparePlayer(PlayerNumber.One, _currentRound);
            if (_goal)
            {
                player1.transform.LookAt(_goal.position);
            }
            NotifyTrailTrackEvent(player1);
            var player2 = levelManager.PreparePlayer(PlayerNumber.Two, _currentRound);
            NotifyTrailTrackEvent(player2);

            var shadows = shadowManager.PrepareShadows();
            _currentVehicleCount = shadows.Count + 2; // + 1 is accounting for the player
            
            // UI
            RoundEvents.PlayerInitialized.Invoke(_currentPlayer);
            RoundEvents.RoundNumber.Invoke(_currentRound + 1, RoundCount);
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.GameParam, (float)_currentRound);
            RoundEvents.CurrentRoundUpdated.Invoke(_config.PotentialScorePerRound[_currentRound]);

            // start camera animation
            gameCameraManager.SetPlayerTransforms(new[] { player1.transform, player2.transform });
            // gameCameraManager.SetPlayerTransform(player1.transform);
            if (_goal)
            {
                gameCameraManager.SetObjectiveTransform(_goal);
                gameCameraManager.SwitchToDollyCamera();
                gameCameraManager.OnDollyAnimationDone.AddListener(OnDollyAnimationDone);
                gameCameraManager.StartAnimation();
            }
            else
            {
                gameCameraManager.SwitchToPlayerCamera();
            }

            // other events
            VehiculeEvent.OnOutOfEnergy.AddListener(OnOutOfEnergy);
            
            GameEvents.ButtonSouthTriggered.AddListener(OnIsStartingDone);
            
            return;

            void NotifyTrailTrackEvent(GameObject vehicleGameObject)
            {
                var vehicleData = vehicleGameObject.GetComponent<VehicleData>();
                TrailEvent.Track.Invoke(vehicleData.VehicleId, vehicleData.Color);
            }
        }

        private void OnIsStartingDone()
        {
            GameEvents.ButtonSouthTriggered.RemoveListener(OnIsStartingDone);
            GameModeTimer.Instance.SetTime(_config.StartCountDown, true);
            GameModeTimer.Instance.OnTimerDone.AddListener(OnStartTimerDone);
            GameModeTimer.Instance.StartTimer();
            chronoStartSfx.PlayWithTryCatch();
            RoundEvents.RoundIsStarting.Invoke();
        }


        private void OnStartTimerDone()
        {
            GameModeTimer.Instance.OnTimerDone.RemoveListener(OnStartTimerDone);
            RoundEvents.RunStarted.Invoke();
        }

        private void OnOutOfEnergy(bool isPlayer)
        {
            _currentVehicleCount -= 1;
            if (_currentVehicleCount > 0) return;

            RoundEvents.TimesOut.Invoke();
            
            var maxPoints = Mathf.Ceil(_config.PotentialScorePerRound.Select(round => round.potetialPoints).Sum() / 2f);
            var someoneHas3Points = ScoreWinnerData.Instance.Player1 >= maxPoints || ScoreWinnerData.Instance.Player2 >= maxPoints;

            if (someoneHas3Points)
            {
                GameEvents.GameEnded.Invoke();
                return;
            }
            RoundEvents.RoundIsEnding.Invoke();
            
            GameEvents.ButtonSouthTriggered.AddListener(OnEndTimerDone);
        }
        
        private void OnEndTimerDone()
        {
            GameEvents.ButtonSouthTriggered.RemoveListener(OnEndTimerDone);
            OnRunEndedSimultaneous();
        }



        private void OnDollyAnimationDone()
        {
            gameCameraManager.OnDollyAnimationDone.RemoveListener(OnDollyAnimationDone);
            gameCameraManager.SwitchToPlayerCamera();
        }

        private void OnRunEndedSimultaneous()
        {
            VehiculeEvent.OnOutOfEnergy.RemoveListener(OnOutOfEnergy);
            levelManager.ResetLevel();
            shadowManager.ResetShadows();
            RoundEvents.RunEnded?.Invoke();
            GameEvents.RoundEnded.Invoke();
        }
    }
}
    