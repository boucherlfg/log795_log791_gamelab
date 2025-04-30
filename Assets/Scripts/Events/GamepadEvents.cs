using GameLab.Enums;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GameLab.Events
{
    public class GamepadEvents
    {
        public static readonly UnityEvent<PlayerNumber, VibrationSource> CallVibration = new();
        public static readonly UnityEvent<PlayerNumber, InputDevice> VibrationRegister = new();
    }
}