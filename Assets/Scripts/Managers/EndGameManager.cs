using System;
using GameLab.Events;
using GameLab.Managers;
using UnityEngine;

namespace GameLab
{
    public class EndGameManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameEvents.GameEnded.AddListener(GameEnded);
        }

        private void OnDestroy()
        {
            GameEvents.GameEnded.RemoveListener(GameEnded);
        }

        private void GameEnded()
        {
            SceneLoader.Load(SceneLoader.SceneTarget.GameEnd);
        }
    }
}
