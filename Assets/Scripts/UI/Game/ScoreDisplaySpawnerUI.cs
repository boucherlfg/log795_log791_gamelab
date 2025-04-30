using System;
using GameLab.Managers;
using UnityEngine;
using GameLab.Enums;

namespace GameLab.UI.Game
{
    public class ScoreDisplaySpawnerUI: MonoBehaviour
    {
        public static ScoreDisplaySpawnerUI Instance { get; private set; }
        
        [SerializeField] private ScoreDisplayUI player1Score;
        [SerializeField] private ScoreDisplayUI player2Score;
        [SerializeField] private ScoreDisplayUI ghostScore;
        [SerializeField] private Vector3 offset = Vector3.right;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogError($"There is more than one ScoreDisplaySpawnerUI singleton! {transform}");
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public void DisplayPlayerScore(Transform target, int score, PlayerNumber playerNumber, int vehicleId)
        {
            var scoreUI = playerNumber switch
                {
                    PlayerNumber.One => player1Score,
                    PlayerNumber.Two => player2Score,
                    _ => throw new ArgumentOutOfRangeException()
                 };
            
            ScoreDisplayUI display = Instantiate(scoreUI, target.position + offset, Quaternion.identity, transform);
            display.SetScore(score);
            display.SetFollow(target);
        }
    }
}