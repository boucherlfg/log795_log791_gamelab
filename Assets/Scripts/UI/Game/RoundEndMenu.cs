using System.Collections.Generic;
using System.Linq;
using GameLab.ColorMode;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class RoundEndMenu : MonoBehaviour
    {
        [Header("Text that can be changed in the UI")] 
        [SerializeField] private string winnerText = "won this round!";
        [SerializeField] private string player1Text = "Red";
        [SerializeField] private string player2Text = "Blue";
        [SerializeField] private string neutralText = "Nobody";
        [Header("Dynamic values")]
        [SerializeField] private ScoreIndicator roundScores;
        [SerializeField] private TMPro.TMP_Text winningPlayer;
        [SerializeField] private DoubleSlider scores;
        [SerializeField] PlayerConfigs playerConfigs;
        [SerializeField] private Animator animator;
        [SerializeField] private bool shouldDisplayTotalScoreArea;
        private Color _drawColor;
        private int _totalPossibleScore;
        private void Awake()
        {
            RoundEvents.RoundIsEnding.AddListener(OpenMenu);
            RoundEvents.RunEnded.AddListener(CloseMenu);
            GameEvents.TotalPointsUpdated.AddListener(UpdateScore);
            GameEvents.AccumulatedScoreCalculated.AddListener(ShowRoundResult);
            
            _drawColor = winningPlayer.color;
            UpdateScore(new List<PlayerNumber>());
            gameObject.SetActive(false);
        }

        private void ShowRoundResult(int player1, int player2)
        {
            _totalPossibleScore = _totalPossibleScore == 0 ? Extensions.FindComponents<IPaintable>().Aggregate(0, (i, paintable) => i + paintable.Score) : _totalPossibleScore;
            var totalScore = shouldDisplayTotalScoreArea ? _totalPossibleScore : player1 + player2;
            
            scores.Value1 = totalScore == 0 ? 0.5f : (float)player1 / totalScore;
            scores.Value2 = totalScore == 0 ? 0.5f : (float)player2 / totalScore;
            if (player1 > player2)
            {
                var html = playerConfigs.player1UI.ToHexString();
                winningPlayer.text = $"<color={html}>{player1Text}</color> {winnerText}";
            }
            else if (player1 < player2)
            {
                var html = playerConfigs.player2UI.ToHexString();
                winningPlayer.text = $"<color={html}>{player2Text}</color> {winnerText}";
            }
            else
            {
                var html = playerConfigs.darkUI.ToHexString();
                winningPlayer.text = $"<color={html}>{neutralText}</color> {winnerText}";
            }
            
        }

        private void UpdateScore(List<PlayerNumber> scorePerRound)
        {
            roundScores.ScoresPerRound = scorePerRound;
        }

        private void CloseMenu()
        {
            gameObject.SetActive(false);
        }

        private void OpenMenu()
        {
            gameObject.SetActive(true);
            animator.Play("PauseUI_Spawn");
        }
    }
}
