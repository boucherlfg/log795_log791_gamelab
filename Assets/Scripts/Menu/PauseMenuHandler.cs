using System;
using GameLab.Events;
using GameLab.Managers;
using GameLab.UI.Game;
using GameLab.UI.MainMenu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLab.Menu
{
    public class PauseMenuHandler: MonoBehaviour
    {
        public static PauseMenuHandler Instance { get; private set; }

        public event EventHandler OnTransitionMainMenu;

        private Action _onMidWayTransition;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(PauseMenuHandler)}! {transform}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            PauseMenuUI.Instance.OnResumeTrigger += PauseMenuUI_OnResumeTrigger; 
            PauseMenuUI.Instance.OnMainMenuTrigger += PauseMenuUI_OnMainMenuTrigger; 
            PauseMenuUI.Instance.OnRestartTrigger += PauseMenuUI_OnRestartTrigger;

            TransitionUI.Instance.OnMidWayTransition += (_, _) => _onMidWayTransition();

            _onMidWayTransition = () => { };
        }

        private void OnEnable()
        {
            InputSystem.EnableDevice(Mouse.current);
        }
        
        private void OnDisable()
        {
            InputSystem.DisableDevice(Mouse.current);
        }
        
        private void PauseMenuUI_OnResumeTrigger(object sender, EventArgs e)
        {
            GameEvents.PauseInputTriggered.Invoke();
        }

        private void PauseMenuUI_OnMainMenuTrigger(object sender, EventArgs e)
        {
            OnTransitionMainMenu?.Invoke(this, EventArgs.Empty);
            
            _onMidWayTransition = () =>
            {
                GameEvents.GameEnded.Invoke();
            };
        }

        private void PauseMenuUI_OnRestartTrigger(object sender, EventArgs e)
        {
            Debug.Log("Game Restarted");
            GameEvents.GameRestarted.Invoke();
        }
    }
}