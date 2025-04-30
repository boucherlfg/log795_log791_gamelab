using System;
using System.Collections.Generic;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class RoundStartMenu : MonoBehaviour
    {
        [Header("Edit menu's text here")]
        [SerializeField] private string roundText;
        
        [Header("Dynamic numbers")]
        [SerializeField] private TMPro.TMP_Text round;
        [SerializeField] private TMPro.TMP_Text message;
        [SerializeField] private ScoreIndicator scoresPerRound;
        [SerializeField] private Animator animator;

        private bool _firstTime = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            RoundEvents.RoundInitialized.AddListener(OpenMenu);
            RoundEvents.RoundIsStarting.AddListener(CloseMenu);
            RoundEvents.RoundNumber.AddListener(UpdateRoundNumber);
            RoundEvents.CurrentRoundUpdated.AddListener(UpdateRoundMessage);
            GameEvents.TotalPointsUpdated.AddListener(UpdateScores);
            UpdateScores(new List<PlayerNumber>());
            gameObject.SetActive(false);
        }

        private void UpdateScores(List<PlayerNumber> scoresPerRound)
        {
            this.scoresPerRound.ScoresPerRound = scoresPerRound;
        }

        private void UpdateRoundMessage(Round round)
        {
            message.text = round.message;
        }

        private void UpdateRoundNumber(int roundNumber, int totalRound)
        {
            round.text = $"{roundText} {roundNumber}";
        }

        private void CloseMenu()
        {
            animator.Play("PauseUI_FadeOut");
        }

        public void Hide() => gameObject.SetActive(false);

        private void OpenMenu()
        {
            gameObject.SetActive(true);
            if (_firstTime)
            {
                animator.Play("PauseUI_Spawn");
            }
            else
            {
                animator.Play("PauseUI_Bump");
            }

            _firstTime = false;
        }
    }
}
