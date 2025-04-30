using GameLab.Enums;
using GameLab.Events;
using GameLab.Managers;
using GameLab.UI.Game;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameLab.TestComponents
{
    public class Testing: MonoBehaviour
    {
        [Range(0f, 1f)]
        public float energy;
        [Range(1, 20)]
        public int round;

        public string playerName = "Dave";
        public int score = 7;
        public PlayerNumber playerNumber;
        [Tooltip("vehicle id 0 and 1 are players")]
        public int vehicleId;
        public void TestPause()
        {
            GameEvents.GamePaused?.Invoke(true);
        }
        
        public void TestEndGame()
        {
            GameEvents.GameEnded?.Invoke();
        }

        public void UpdateEnergy()
        {
            RoundEvents.EnergyRatio.Invoke(energy);
        }

        public void UpdateRound()
        {
            RoundEvents.RoundNumber.Invoke(round, 3);
        }

        public void UpdatePlayerTurn()
        {
            GameEvents.PlayerTurn.Invoke(playerName);
        }

        public void DisplayPlayerScore()
        {
            ScoreDisplaySpawnerUI.Instance.DisplayPlayerScore(transform, score, playerNumber, vehicleId);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Testing))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Testing test = (Testing)target;
            if(GUILayout.Button("Trigger pause"))
                test.TestPause();
            if(GUILayout.Button("Trigger end game"))
                test.TestEndGame();
            if(GUILayout.Button("Update energy"))
                test.UpdateEnergy();
            if(GUILayout.Button("Update Round"))
                test.UpdateRound();
            if(GUILayout.Button("Update Player Turn"))
                test.UpdatePlayerTurn();
            if(GUILayout.Button("Show Player Score"))
                test.DisplayPlayerScore();
        }
    }
#endif
}

