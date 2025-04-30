using System;
using System.Collections.Generic;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Events;
using GameLab.Player;
using UnityEngine;
using GameLab.Enums;
using GameLab.ScriptableObjects;

namespace GameLab.Managers
{
    public class ShadowManager : MonoBehaviour
    {
        [SerializeField] private GameObject shadowPrefab;
        [SerializeField] private Material[] playerMaterials;
        [SerializeField] private PlayerConfigs playerConfig;

        private readonly List<MementoPlayerInfo> _mementoList = new();
        private readonly List<GameObject> _shadowInstances = new();
        private PlayerColorChangerRevamped _playerColorChanger;
        private static readonly int FresnelColor = Shader.PropertyToID("_Fresnel_Color");

        private void Start()
        {
            GameEvents.MementoRequested.AddListener(AddShadowCircuit);
            _playerColorChanger = GetComponent<PlayerColorChangerRevamped>();
        }

        private void OnDestroy()
        {
            GameEvents.MementoRequested.RemoveListener(AddShadowCircuit);
        }

        private void AddShadowCircuit(MementoPlayerInfo mementoInfo)
        {
            _mementoList.Add(mementoInfo);
        }

        /// <summary>
        /// creates as many shadows as there were paths recorded
        /// </summary>
        /// <returns>the number of shadows that were spawned</returns>
        public List<GameObject> PrepareShadows()
        {
            var vehicleId = 2; // players are 0 and 1, so we start at 2
            foreach (var data in _mementoList)
            {
                var instance = Instantiate(shadowPrefab, data.InitialPosition, data.InitialRotation, transform);
                var shadow = instance.GetComponent<ShadowScript>();
                shadow.SetShadow(new List<MementoEntry>(data.MementoEntries), data.PlayerId, vehicleId);

                var vehicleData = shadow.gameObject.AddComponent<VehicleData>();
                vehicleData.PlayerNumber = (PlayerNumber)data.PlayerId;
                vehicleData.VehicleId = vehicleId;
                
                _playerColorChanger.SetPlayerHue(shadow.GetComponent<MovingObject>(), data.PlayerId);
                instance.GetComponentInChildren<Renderer>().material.SetColor(FresnelColor, playerConfig.ShadowColors[(int)data.PlayerId - 1]);

                vehicleId++;
                
                _shadowInstances.Add(instance);
            }
            return _shadowInstances;
        }

        public void ResetShadows()
        {
            foreach(var shadow in _shadowInstances) Destroy(shadow);
            _shadowInstances.Clear();
        }
    }
}