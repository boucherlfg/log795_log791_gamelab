using UnityEngine;

namespace GameLab.ScriptableObjects
{
    public enum ScoreMethod
    {
        Zone = 0,
        Distance = 1,
    }
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Gamelab/ScoreConfig")]
    public class ScoreConfig : ScriptableObject
    {
        [SerializeField] private ScoreMethod scoreMethod;
        [SerializeField] private int bonusForFirstPlayer;
        [SerializeField] private float scoringInterval = 2;
        
        public ScoreMethod ScoreMethod => scoreMethod;
        public int BonusForFirstPlayer => bonusForFirstPlayer;
        
        public float ScoringInterval => scoringInterval;
    }
}