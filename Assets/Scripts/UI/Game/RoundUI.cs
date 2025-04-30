using System;
using GameLab.Events;
using GameLab.Managers;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    // must be executed before round manager
    [DefaultExecutionOrder(-5)]
    public class RoundUI: MonoBehaviour
    {
        private const string ShowScore = "RoundUI_ShowScore";
        private const string HideScore = "RoundUI_HideScore";
        
        [SerializeField] private Text roundText;
        [SerializeField] private Animator anim;

        private void Start()
        {
            roundText.text = $"Round: 1";
            RoundEvents.RoundNumber.AddListener(DisplayRound);
            
            RoundEvents.RunStarted.AddListener(() => anim.Play(HideScore));
        }

        private void DisplayRound(int round, int totalRounds)
        {
            roundText.text = $"Round: {round}";
            anim.Play(ShowScore);
        }
    }
}