using System;
using System.Collections.Generic;
using GameLab.Events;
using UnityEngine;

namespace GameLab.TestComponents
{
    public class ScoreTester : MonoBehaviour
    {
        public static event EventHandler<Transform> onSpawnedTrigget;
        public static void ClearStaticVariables()
        {
            onSpawnedTrigget = null;
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            onSpawnedTrigget?.Invoke(this, transform);
            
            GameEvents.AccumulatedScoreCalculated.AddListener(ScoreCalculated);
            GameEvents.VehicleScoreCalculated.AddListener(VehicleScoreCalculated);
        }

        private void VehicleScoreCalculated(Dictionary<int, int> arg0)
        {
            foreach (var data in arg0)
            {
                Debug.Log(data.Key + " : " + data.Value);
            }
        }

        private void ScoreCalculated(int arg0, int arg1)
        {
            Debug.Log("player 1 : " + arg0);
            Debug.Log("player 2 : " + arg1);
        }
    }
}
