using System;
using UnityEngine;

namespace GameLab.Utility
{
    [Serializable]
    public class DeviceDropdownComponent : MonoBehaviour
    {
        [SerializeField] private string selectedDevice;
        public string SelectedDevice
        {
            get => selectedDevice;
            set => selectedDevice = value;
        }
    }
}