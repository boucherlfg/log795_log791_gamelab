using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLab.Core;
using GameLab.Events;
using GameLab.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace GameLab.Managers
{
    [DefaultExecutionOrder(-20)]
    public class DebugInputManager : InputManagerBase
    {
        [SerializeField] private DeviceDropdownComponent player1Device;
        [SerializeField] private DeviceDropdownComponent player2Device;
        [SerializeField] private GameObject playerInputPrefab;

        private PlayerInput _player1Input;
        private PlayerInput _player2Input;

        private void Start()
        {
            InputSystem.DisableDevice(Mouse.current);
            GameEvents.Players.Clear();
            GameEvents.PauseInputTriggered.RemoveAllListeners();
            SpawnPlayer1();
            SpawnPlayer2();
            _ = ScoreWinnerData.Instance;
        }

        private void SpawnPlayer1()
        {
            InputDevice device = FindDeviceByName(player1Device.SelectedDevice);
            _player1Input = PlayerInput.Instantiate(playerInputPrefab, 1, FindControlSchemeForDevice(device), -1, device);
            BindEvents(1, _player1Input);
        }

        private void SpawnPlayer2()
        {
            InputDevice device = FindDeviceByName(player2Device.SelectedDevice);
            _player2Input = PlayerInput.Instantiate(playerInputPrefab, 2, FindControlSchemeForDevice(device), -1, device);
            BindEvents(2, _player2Input);
        }

        private InputDevice FindDeviceByName(string deviceName)
        {
            foreach (var device in InputSystem.devices)
            {
                if (device.name == deviceName)
                {
                    return device;
                }
            }

            return null;
        }


        private string FindControlSchemeForDevice(InputDevice device)
        {
            // Loop through all control schemes in the PlayerInput component
            var playerInput = playerInputPrefab.GetComponent<PlayerInput>();
            var actionAsset = playerInput.actions;

            foreach (var scheme in actionAsset.controlSchemes)
            {
                if (scheme.SupportsDevice(device))
                {
                    return scheme.name;
                }
            }

            return null; // No scheme matched
        }

    }
}