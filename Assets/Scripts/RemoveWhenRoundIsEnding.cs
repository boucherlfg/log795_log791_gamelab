using System;
using GameLab.Events;
using UnityEngine;

namespace GameLab
{
    public class RemoveWhenRoundIsEnding : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            RoundEvents.RoundIsEnding.AddListener(OnRoundIsEnding);
        }

        private void OnDestroy()
        {
            RoundEvents.RoundIsEnding.RemoveListener(OnRoundIsEnding);
        }

        private void OnRoundIsEnding()
        {
            Destroy(gameObject);
        }
    }
}
