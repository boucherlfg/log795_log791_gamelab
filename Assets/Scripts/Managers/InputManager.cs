using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLab.Managers
{
    public class InputManager : InputManagerBase
    {
        private Coroutine _lobbyCoroutine;
        [SerializeField] private PlayerInput lobbyInput;
        private void Start()
        {
            GetComponent<PlayerInputManager>().playerJoinedEvent.AddListener(OnPlayerJoined);
            playerInputs.Clear();
            for (int i = 0; i < playerInputs.Count; i++)
            {
                LobbyEvent.PlayerJoined.Invoke(i+1);
            }
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            if (playerInput.defaultActionMap == "Lobby") return;
            
            var playerId = playerInputs.Count + 1;

            BindEvents(playerId, playerInput);
            
            // Enable The Ready action when Space/Start is not pressed

            _lobbyCoroutine ??= StartCoroutine(EnableReadyAction(playerInput, playerId));

            LobbyEvent.PlayerJoined.Invoke(playerId);
        }
        
        IEnumerator EnableReadyAction(PlayerInput playerInput, int playerId)
        {
            // yield return new WaitUntil(() => playerInput.actions.FindAction("Pause").inProgress == false);

            if (!playerInput) yield break;

            playerInput.SwitchCurrentActionMap("Lobby");
            playerInput.actions.FindAction("ReadyUp").performed += _ =>
            {
                if (playerInputs.Count <= 1) return;
                LobbyEvent.ReadyUp.Invoke(playerId);
                var input = playerInputs[playerId - 1];
                if (!input) return;

                playerInputs.ForEach(i => i.SwitchCurrentActionMap("Player"));
            };
            
            playerInput.actions.FindAction("Move").performed += context =>
            {
                var moveVector = context.ReadValue<Vector2>().normalized;
                GameEvents.Players[playerId].Move.Invoke(moveVector);
            };
            playerInput.actions.FindAction("Move").canceled += _ =>
            {
                GameEvents.Players[playerId].Move.Invoke(new Vector2(0,0));
            };


            playerInput.actions.FindAction("Accelerate").performed += _ =>
            {
                GameEvents.Players[playerId].Accelerate.Invoke(true);
            };

            playerInput.actions.FindAction("Accelerate").canceled += _ =>
            {
                GameEvents.Players[playerId].Accelerate.Invoke(false);
            };
            playerInput.actions.FindAction("Dash").performed += _ =>
            {
                GameEvents.Players[playerId].Dash.Invoke();
            };
            playerInput.actions.FindAction("TutorialNext").performed += context =>
            {
                LobbyEvent.TutorialNext.Invoke();
            };

            _lobbyCoroutine = null;
        }
    }
}
