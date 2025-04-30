using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLab.Utility
{
    public class VibrationUtility : MonoBehaviour
    {
        [SerializeField] private VibrationConfig vibrationConfig;
        private readonly Dictionary<PlayerNumber, InputDevice> _deviceAllocation = new ();
        private readonly Dictionary<PlayerNumber, Coroutine> _vibrationCoroutines = new();
        private List<InputDevice> _devices = new();

        void Awake()
        {
            InputSystem.onDeviceChange += InputSystemOnDeviceChange;
            GamepadEvents.VibrationRegister.AddListener(RegisterGamepad);
            GamepadEvents.CallVibration.AddListener(Vibrate);
        }

        private void OnDestroy()
        {
            InputSystem.devices.OfType<Gamepad>().ToList().ForEach(x => x.SetMotorSpeeds(0, 0));
        }

        private void Vibrate(PlayerNumber player, VibrationSource source)
        {
            if (!_deviceAllocation.TryGetValue(player, out var device)) return;
            if (!InputSystem.devices.Contains(device)) return;
            if (device is not Gamepad gamepad) return;
            if (!vibrationConfig.TryGetCurve(source, out var curve)) return;
            
            if(_vibrationCoroutines.TryGetValue(player, out var vibrationCoroutine) && vibrationCoroutine != null) StopCoroutine(vibrationCoroutine);
            _vibrationCoroutines[player] = StartCoroutine(Vibrate(gamepad, curve));
        }
        private IEnumerator Vibrate(Gamepad gamepad, AnimationCurve curve)
        {
            var duration = curve.keys[^1].time;

            var startTime = Time.unscaledTime;
            var targetTime = Time.unscaledTime + duration;


            while (Time.unscaledTime <= targetTime)
            {
                var elapsedTime = Mathf.Clamp(Time.unscaledTime - startTime, 0f, duration);
                var value = curve.Evaluate(elapsedTime);
                gamepad.SetMotorSpeeds(value, value);
                yield return null;
            }
            gamepad.SetMotorSpeeds(0, 0);
        }

        private void Reset()
        {
            _devices = InputSystem.devices.ToList();
            _deviceAllocation.Clear();
        }
        private void RegisterGamepad(PlayerNumber player, InputDevice device)
        {
            _deviceAllocation[player] = device;
        }

        private void InputSystemOnDeviceChange(InputDevice device, InputDeviceChange evt)
        {
            switch (evt)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Reconnected:
                case InputDeviceChange.Enabled:
                    if (_devices.Contains(device)) break;
                    _devices.Add(device);
                    break;
                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                case InputDeviceChange.Disabled:
                    if(!_devices.Contains(device)) break;
                    _devices.Remove(device);
                    break;
            }
        }
        
        
    }
}
