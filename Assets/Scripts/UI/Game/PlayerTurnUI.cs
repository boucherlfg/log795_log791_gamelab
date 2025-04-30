using System;
using GameLab.Events;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    [DefaultExecutionOrder(-5)]
    public class PlayerTurnUI: MonoBehaviour
    {
        [SerializeField] private Text playerNameText;

        private void Start()
        {
            GameEvents.PlayerTurn.AddListener( player => playerNameText.text = $"Player {player}");
        }
    }
}