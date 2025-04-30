using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLab.Behaviours;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLab.Core
{
    public class InputManagerBase: MonoBehaviour
    {
        [SerializeField]
        protected List<PlayerInput> playerInputs = new();

        private bool canJoin = false;

        private void Awake()
        {
            Reset();
        }

        protected virtual void Reset()
        {
            canJoin = false;
            InputSystem.DisableDevice(Mouse.current);
            foreach (var inputBehaviour in Extensions.FindComponents<InputBehaviour>())
            {
                Destroy(inputBehaviour.gameObject);
            }
            
            GameEvents.Players.Clear();
            playerInputs.Clear();
            canJoin = true;
        }
        protected void BindEvents(int playerId, PlayerInput playerInput)
        {
            if (!canJoin) return;
            
            var device = playerInput.devices.First(x => x is not Mouse); 
            GamepadEvents.VibrationRegister.Invoke((PlayerNumber)playerId, device);
            
            playerInput.name += playerInputs.Count;

            playerInputs.Add(playerInput);

            GameEvents.Players.Add(playerId, new PlayerEvent());

            playerInput.actions.FindAction("LevelLeft").performed += context =>
            {
                if (!context.performed) return;
                LevelSelectorEvents.Left.Invoke();
            };
            playerInput.actions.FindAction("LevelRight").performed += context =>
            {
                if (!context.performed) return;
                LevelSelectorEvents.Right.Invoke();
            };
            playerInput.actions.FindAction("LevelSelect").performed += context =>
            {
                if (!context.performed) return;
                LevelSelectorEvents.Select.Invoke();
            };
            playerInput.actions.FindAction("Pause").performed += context =>
            {
                GameEvents.PauseInputTriggered.Invoke();
            };
            playerInput.actions.FindAction("Move").performed += context =>
            {
                var moveVector = context.ReadValue<Vector2>().normalized;
                GameEvents.Players[playerId].Move.Invoke(moveVector);
            };
            playerInput.actions.FindAction("Move").canceled += context =>
            {
                GameEvents.Players[playerId].Move.Invoke(new Vector2(0,0));
            };

            playerInput.actions.FindAction("ButtonSouth").performed += context =>
            {
                GameEvents.ButtonSouthTriggered.Invoke();
            };

            playerInput.actions.FindAction("Accelerate").performed += context =>
            {
                GameEvents.Players[playerId].Accelerate.Invoke(true);
            };

            playerInput.actions.FindAction("Accelerate").canceled += context =>
            {
                GameEvents.Players[playerId].Accelerate.Invoke(false);
            };
            
            playerInput.actions.FindAction("Dash").performed += context =>
            {
                GameEvents.Players[playerId].Dash.Invoke();
            };
        }
    }
}