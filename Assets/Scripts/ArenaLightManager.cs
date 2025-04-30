using System;
using System.Collections;
using GameLab.Events;
using UnityEngine;

namespace GameLab
{
    public class ArenaLightManager : MonoBehaviour
    {
        private GameObject arenaLight;

        private const string ArenaLightTag = "ArenaLight";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            GameEvents.GameStarted.AddListener(DimLights);
            RoundEvents.RoundIsEnding.AddListener(DimLights);
            RoundEvents.RunStarted.AddListener(RaiseLights);

            yield return null;
            DimLights();
        }

        private void OnDestroy()
        {
            GameEvents.GameStarted.RemoveListener(DimLights);
            RoundEvents.RoundIsEnding.RemoveListener(DimLights);
            RoundEvents.RunStarted.RemoveListener(RaiseLights);
        }

        private void DimLights()
        {
            arenaLight = arenaLight ? arenaLight : GameObject.FindGameObjectWithTag(ArenaLightTag);
            arenaLight.SetActive(false);
        }

        private void RaiseLights()
        {
            arenaLight = arenaLight ? arenaLight : GameObject.FindGameObjectWithTag(ArenaLightTag);
            arenaLight.SetActive(true);
        }
    }
}
