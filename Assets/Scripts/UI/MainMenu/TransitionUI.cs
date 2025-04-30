using System;
using GameLab.Menu;
using GameLab.UI.Game;
using UnityEngine;

namespace GameLab.UI.MainMenu
{
    // Need this to be executed before the MainMenuHandler script
    [DefaultExecutionOrder(-20)]
    public class TransitionUI: MonoBehaviour
    {
        public static TransitionUI Instance { get; private set; }

        public event EventHandler OnMidWayTransition;

        private const string TransitionPrevious = "TransitionUI_Previous";
        private const string TransitionNext = "TransitionUI_Next";
        private const string TransitionReveal = "TransitionUI_Reveal";
        
        [SerializeField] private Animator transition;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(TransitionUI)}! {transform}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            if(MainMenuHandler.Instance)
            {
                MainMenuHandler.Instance.OnTransitionMainMenu += (_, _) => transition.Play(TransitionPrevious);
                MainMenuHandler.Instance.OnTransitionSelectPlayer += (_, _) => transition.Play(TransitionNext);
                MainMenuHandler.Instance.OnTransitionReveal += (_, _) => transition.Play(TransitionReveal);
            }
            if (PauseMenuHandler.Instance)
            {
                PauseMenuHandler.Instance.OnTransitionMainMenu += (_, _) => transition.Play(TransitionPrevious);
            }

            if (EndGameUI.Instance)
            {
                EndGameUI.Instance.OnMainMenuTrigger += (_, _) => transition.Play(TransitionPrevious); 
            }
        }

        private void MidWayTransition()
        {
            OnMidWayTransition?.Invoke(this, EventArgs.Empty);
        }
    }
}