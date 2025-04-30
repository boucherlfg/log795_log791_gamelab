using System;
using GameLab.Events;
using UnityEngine;

namespace GameLab
{
    public class CrowdSound : MonoBehaviour
    {
        [SerializeField] private int maxScoreDifference;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameEvents.AccumulatedScoreCalculated.AddListener(OnScoreUpdated);
        }

        private void OnDestroy()
        {
            GameEvents.AccumulatedScoreCalculated.RemoveListener(OnScoreUpdated);
        }

        private void OnScoreUpdated(int player1Score, int player2Score)
        {
            float ratio = Mathf.Clamp((float)(player2Score - player1Score) / (float)maxScoreDifference, -1, 1);
            SoundEvents.OnChangeBGMGlobalParam.Invoke("SCORE_RATIO", ratio);
        }
    }
}
