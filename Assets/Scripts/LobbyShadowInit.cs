using GameLab.Behaviours;
using GameLab.Enums;
using UnityEngine;

namespace GameLab
{
    public class LobbyShadowInit : MonoBehaviour
    {
        [SerializeField] private VehicleData _vehicleData;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _vehicleData.PlayerNumber = PlayerNumber.None;
            _vehicleData.VehicleId = 2;
        }
    }
}
