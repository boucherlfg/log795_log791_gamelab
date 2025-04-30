using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.ColorMode;
using GameLab.Constructions;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects;
using GameLab.UI.Lobby;
using GameLab.Utility;
using TMPro;
using UnityEngine;

namespace GameLab.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private SceneLoader.SceneTarget nextScene = SceneLoader.SceneTarget.Game;
        [SerializeField] public GameObject playerPrefab;
        [SerializeField] private  Transform[] playerSpawns = new Transform[2];

        private JoinUIBehaviour[] _joinUIs;
        [SerializeField] private TextMeshProUGUI playerReadyText;

        private int _minimumPlayerCount = 2;

        private List<int> _playerReadyMap = new();
        private PlayerColorChangerRevamped _playerColorChangerRevamped;

        [SerializeField] private StudioEventEmitter joinSFX;
        [SerializeField] private StudioEventEmitter readySFX;
        [SerializeField] private PlayerConfigs playerConfigs;

        private void Start()
        {
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.MainMenuParam, (float)SoundEvents.MainMenuState.LOBBY);
            _joinUIs = FindObjectsByType<JoinUIBehaviour>(FindObjectsSortMode.None);
            SetPlayerReadyText();
            _playerColorChangerRevamped = GetComponent<PlayerColorChangerRevamped>();
            var paintables = Extensions.FindComponents<IPaintable>().ToList();
            ColorSplash.paintables = paintables;

            LobbyEvent.PlayerJoined.AddListener(OnPlayerJoined);
            LobbyEvent.ReadyUp.AddListener(OnReadyUp);
        }

        private void OnDestroy()
        {
            LobbyEvent.PlayerJoined.RemoveListener(OnPlayerJoined);
            LobbyEvent.ReadyUp.RemoveListener(OnReadyUp);
        }

        private void OnPlayerJoined(int playerId)
        {
            // SPAWN PLAYER
            Transform spawn = playerSpawns[playerId - 1];
            var instance = Instantiate(playerPrefab, spawn.position, spawn.rotation);
            VehicleData vehicleData = instance.AddComponent<VehicleData>();
            vehicleData.PlayerNumber = (PlayerNumber)playerId;
            vehicleData.VehicleId = playerId - 1;

            // UPDATE PLAYER SCRIPT VALUES
            var playerManager = instance.GetComponent<PlayerScript>();
            playerManager.SetPlayer(playerId, true);
            playerManager.SetVisualIndicator(playerConfigs.DecalsSelectMaterials[playerId - 1], playerConfigs.EmissionIntensities[playerId - 1], playerConfigs.EmissionTients[playerId - 1]);

            playerManager.IsRunNotStarted = false;
            _playerColorChangerRevamped.SetPlayerHue(playerManager, playerId);

            // UPDATE INTERNAL STATE
            _playerReadyMap.Add(playerId);

            // UPDATE UI
            if (_joinUIs.Length > 0)
            {
                _joinUIs.First(x => x.PlayerId == playerId).MakeTextRequestReady();
            }
            joinSFX.PlayWithTryCatch();
            SetPlayerReadyText();
        }
        
        private void OnReadyUp(int playerId)
        {
            // we start the game if there are exactly 2 players and someone has pressed start
            if (_playerReadyMap.Count < _minimumPlayerCount)
            {
                return;
            } 
            if (!_playerReadyMap.Contains(playerId))
            {
                return;
            }
            
            readySFX.PlayWithTryCatch();
            SceneLoader.Load(nextScene);
        }


        private void SetPlayerReadyText()
        {
            var playerCount = _playerReadyMap.Count;
            String message = playerCount == _minimumPlayerCount ? "Press start to begin" : $"Player join ({playerCount}/{_minimumPlayerCount})";
            playerReadyText.SetText(message);
        }
    }
}

