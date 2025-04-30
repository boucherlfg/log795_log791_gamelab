using System.Collections;
using System.Linq;
using GameLab.ColorMode;
using GameLab.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    [DefaultExecutionOrder(-5)]
    public class ScoreBarUI : MonoBehaviour
    {
        [SerializeField] private DoubleSlider slider;
        private int _totalScore;
        [SerializeField] private bool shouldDisplayTotalScoreArea;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            GameEvents.AccumulatedScoreCalculated.AddListener(OnTotalScoreCalculated);
            GameEvents.GameStarted.AddListener(OnReady);
        }

        private void OnReady()
        {
            slider.Value1 = slider.Value2 = 0.5f;
            _totalScore = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IPaintable>().Sum(paintable => paintable.Score);
        }

        private void OnTotalScoreCalculated(int player1Score, int player2Score)
        {
            var totalScore = player1Score + player2Score;
            if(shouldDisplayTotalScoreArea) totalScore = _totalScore;
            
            var score1 = totalScore == 0 ? 0.5f : player1Score / (float)totalScore;
            var score2 = totalScore == 0 ? 0.5f : player2Score / (float)totalScore;
            slider.Value1 = score1;
            slider.Value2 = score2;
        }
    }
}