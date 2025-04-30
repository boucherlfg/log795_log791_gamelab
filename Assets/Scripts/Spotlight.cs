using System;
using GameLab.Behaviours;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;

namespace GameLab
{
    public class Spotlight : MonoBehaviour
    {
        [SerializeField] private PlayerScript playerScript;
        [SerializeField] private PlayerConfigs playerConfigs;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameEvents.GameStarted.AddListener(RaiseLights);
            RoundEvents.RoundIsEnding.AddListener(RaiseLights);
            RoundEvents.RunStarted.AddListener(DimLights);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GameEvents.GameStarted.RemoveListener(RaiseLights);
            RoundEvents.RoundIsEnding.RemoveListener(RaiseLights);
            RoundEvents.RunStarted.RemoveListener(DimLights);
        }

        private void DimLights()
        {
            gameObject.SetActive(false);
        }

        private void RaiseLights()
        {
            if (!playerScript) return;
            if (TryGetComponent(out Light lights))
            {
                lights.color = playerConfigs.PlayerColors[playerScript.Id - 1];
            }

            gameObject.SetActive(true);
        }
    }
}
