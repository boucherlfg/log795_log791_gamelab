using System;
using GameLab.Events;
using UnityEngine;

namespace GameLab
{
    public class FinishPointUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            RoundEvents.RunEnded.AddListener(OnRoundEnded);
            RoundEvents.RunStarted.AddListener(OnRoundStarted);
        }


        private void OnRoundStarted()
        {
            Debug.Log("Round Started");
            panel.SetActive(false);
        }

        private void OnRoundEnded()
        {
            Debug.Log("Round Ended");
            panel.SetActive(true);
        }

        private void OnDestroy()
        {
            GameEvents.RoundEnded.RemoveListener(OnRoundEnded);
            GameEvents.RoundStarted.RemoveListener(OnRoundStarted);
        }
    }
}
