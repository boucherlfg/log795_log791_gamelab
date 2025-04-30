using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    [Serializable]
    public struct Round
    {
        public int potetialPoints;
        public string message;
    }
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Gamelab/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private int roundCount = 6;
        [SerializeField] private int bonusForFirstPlayer = 5;
        [SerializeField] private int startCountDown = 3;
        [SerializeField] private int endCountDown = 2;
        [SerializeField] private GameObject level;

        [SerializeField] private int startRoundUITime = 3;
        [SerializeField] private int endRoundUITime = 3;
        [SerializeField] private List<Round> potentialScorePerRound;
        
        public int RoundCount => roundCount;
        
        public int StartCountDown => startCountDown;
        
        public int EndCountDown => endCountDown;
        
        public GameObject Level => level;

        public int StartRoundUITime => startRoundUITime;
        public int EndRoundUITime => endRoundUITime;
        public List<Round> PotentialScorePerRound => potentialScorePerRound;

        private void OnValidate()
        {
            while(PotentialScorePerRound.Count < roundCount) PotentialScorePerRound.Add(new Round());
            while(PotentialScorePerRound.Count > roundCount) PotentialScorePerRound.RemoveAt(roundCount - 1);
        }
    }
}