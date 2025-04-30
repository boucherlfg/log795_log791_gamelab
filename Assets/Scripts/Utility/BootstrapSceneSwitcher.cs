/**
 * author: Jimmy Tremblay-Bernier
 */

using System;
using GameLab.Events;
using GameLab.Managers;
using UnityEngine;

namespace GameLab.Utility
{

    public class BootstrapSceneSwitcher : MonoBehaviour
    {
        private void Awake()
        {
            BootstrapEvent.OnBootstrapInitialized.AddListener(OnSceneInitialized);
        }

        private void OnSceneInitialized()
        {
            // INIT COMPLETE, MOVE TO MAIN MENU
            SceneLoader.LoadDirect(SceneLoader.SceneTarget.MainMenu);
        }
    }
}