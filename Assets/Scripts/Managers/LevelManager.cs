using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Behaviours;
using GameLab.Constructions;
using GameLab.Player;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using GameLab.Enums;
using GameLab.Utility;
using Unity.VisualScripting;

namespace GameLab.Managers
{
    // this needs to be initialized before the GameModeManager
    public class LevelManager : MonoBehaviour
    {
        private readonly Dictionary<PlayerNumber, List<PlayerSpawner>> _playerSpawns = new()
        {
            { PlayerNumber.One, new()},
            { PlayerNumber.Two, new()}
        };
        private readonly Dictionary<int, GameObject> _playerInstances = new();
        
        [SerializeField] private MementoRecorder[] recorders;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PlayerColorChangerRevamped playerColorChanger;
        [SerializeField] private PlayerConfigs playerConfigs;

        public int RoundCount => _playerSpawns.Count > 0 ? _playerSpawns.Min(list => list.Value.Count) : 0;

        private void Start()
        {
            playerColorChanger = GetComponent<PlayerColorChangerRevamped>();
        }

        public void ResetSpawners()
        {
            foreach (var playerSpawns in _playerSpawns)
            {
                foreach (var spawn in playerSpawns.Value)
                {
                    spawn.SelectSpawner(false);
                    spawn.Reset();
                }
            }
        }

        ///<param name="player">values : 1 is player 1, 2 is player 2</param>
        /// <param name="round"></param>
        public GameObject PreparePlayer(PlayerNumber player, int round)
        {
            int playerIndex = (int)player - 1;
            
            var spawn = _playerSpawns[player][round];
            spawn.SelectSpawner(true);
            spawn.UpdateRendering();
            var playerInstance = Instantiate(playerPrefab, spawn.SpawnLocation.position, spawn.SpawnLocation.rotation);
            
            var playerManager = playerInstance.GetComponent<PlayerScript>();
            playerManager.SetPlayer(playerIndex + 1);
            playerManager.SetVisualIndicator(playerConfigs.DecalsSelectMaterials[playerIndex], playerConfigs.EmissionIntensities[playerIndex], playerConfigs.EmissionTients[playerIndex]);

            _playerInstances.TryAdd((int)player, playerInstance);
            recorders[playerIndex].SetPlayer((int)player, spawn.SpawnLocation.position, spawn.SpawnLocation.rotation);
   
            var vehicle = playerInstance.AddComponent<VehicleData>();
            vehicle.PlayerNumber = player;
            vehicle.VehicleId = playerIndex; // player is always 0 or 1
            
            playerColorChanger.SetPlayerHue(playerInstance.GetComponent<MovingObject>(), (int)player);
            
            return playerInstance;
        }

        public void PrepareLevel(GameConfig config)
        {
            var level = LevelSelector.Level ? LevelSelector.Level : config.Level;
            Instantiate(level, Vector3.zero, Quaternion.identity);

            var spawns = Extensions.FindComponents<PlayerSpawner>().ToList();

            // SELECT PLAYER ONE SPAWNS ORDERED BY THEIR INDEX
            var playerOneSpawns = spawns.Where(spawn => spawn.Player == PlayerNumber.One).OrderBy(spawn => spawn.SpawnIndex).ToList();
            var numDistinctSpawns = playerOneSpawns.DistinctBy(spawn => spawn.SpawnIndex).Count();

            // VALIDATE THAT THEY'RE UNIQUE
            if (numDistinctSpawns != playerOneSpawns.Count())
            {
                Debug.LogError("There is two spawner with the same index for player one");
            }
            _playerSpawns[PlayerNumber.One] = playerOneSpawns;

            // SELECT PLAYER TWO SPAWNS ORDERED BY THEIR INDEX
            var playerTwoSpawns = spawns.Where(spawn => spawn.Player == PlayerNumber.Two).OrderBy(spawn => spawn.SpawnIndex).ToList();
            numDistinctSpawns = playerTwoSpawns.DistinctBy(spawn => spawn.SpawnIndex).Count();

            // VALIDATE THAT THEY'RE UNIQUE
            if (numDistinctSpawns != playerTwoSpawns.Count())
            {
                Debug.LogError("There is two spawner with the same index for player two");
            }
            _playerSpawns[PlayerNumber.Two] = playerTwoSpawns;
        }

        public void ResetLevel()
        {
            foreach(var memento in recorders) memento.SaveMemento();
            foreach (var player in _playerInstances.Values) Destroy(player);
            _playerInstances.Clear();
        }
    }
}