using System;
using GameLab.Events;
using GameLab.Managers;
using GameLab.UI.MainMenu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLab.Menu
{
    public class MainMenuHandler: MonoBehaviour
    {
        public static MainMenuHandler Instance { get; private set; }

        public event EventHandler OnTransitionMainMenu;
        public event EventHandler OnTransitionSelectPlayer;
        public event EventHandler OnTransitionReveal;
        public event EventHandler OnOptionsPopUp;
        public event EventHandler OnOptionsPopOut;
        public event EventHandler OnCreditPopUp;
        public event EventHandler OnCreditPopOut;
        
        public event EventHandler OnSelectPlayerReveal;
        public event EventHandler OnMainMenuReveal;
        public event EventHandler OnOptionsReveal;
        public event EventHandler OnCreditReveal;

        private Action _midWayAnimationAction;
        
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(MainMenuHandler)}! {transform}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.MainMenuParam, (float)SoundEvents.MainMenuState.MAIN);
            MainMenuSectionUI.Instance.OnNewGameTrigger += MainMenuSectionUI_OnNewGameTrigger;
            MainMenuSectionUI.Instance.OnOptionTrigger += MainMenuSectionUI_OnOptionTrigger;
            MainMenuSectionUI.Instance.OnCreditTrigger += MainMenuSectionUI_OnCreditTrigger;
            MainMenuSectionUI.Instance.OnExitTrigger += (_, _) => Application.Quit();

            MainMenuOptionsUI.Instance.OnSaveTrigger += MainMenuOptionsUI_OnSaveTrigger;
            MainMenuOptionsUI.Instance.OnCancelTrigger += MainMenuOptionsUI_OnCancelTrigger;
            MainMenuOptionsUI.Instance.OnMidWayAnimation += (_, _) => _midWayAnimationAction();
            
            MainMenuCreditUI.Instance.OnReturnTrigger += MainMenuCreditUI_OnReturnTrigger;
            MainMenuCreditUI.Instance.OnMidWayAnimation += (_, _) => _midWayAnimationAction();

            TransitionUI.Instance.OnMidWayTransition += (_, _) => _midWayAnimationAction();
            
            MainMenuSelectPlayerUI.Instance.OnReturnTrigger += MainMenuSelectPlayer_OnReturnTrigger;
            
            if(!SceneLoader.WasDirectLoaded)
                OnTransitionReveal?.Invoke(this, EventArgs.Empty);
        }

        private void OnEnable()
        {
            InputSystem.EnableDevice(Mouse.current);
        }
        
        private void OnDisable()
        {
            InputSystem.DisableDevice(Mouse.current);
        }

        private void MainMenuSectionUI_OnCreditTrigger(object sender, EventArgs e)
        {
            OnCreditPopUp?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnCreditReveal?.Invoke(this, EventArgs.Empty);
        }
        
        private void MainMenuCreditUI_OnReturnTrigger(object sender, EventArgs e)
        {
            OnCreditPopOut?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnMainMenuReveal?.Invoke(this, EventArgs.Empty);
        }
        
        private void MainMenuSectionUI_OnNewGameTrigger(object sender, EventArgs e)
        {
            OnTransitionSelectPlayer?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnSelectPlayerReveal?.Invoke(this, EventArgs.Empty);
        }
        
        private void MainMenuSectionUI_OnOptionTrigger(object sender, EventArgs e)
        {
            OnOptionsPopUp?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnOptionsReveal?.Invoke(this, EventArgs.Empty);
        }

        private void MainMenuOptionsUI_OnSaveTrigger(object sender, EventArgs e)
        {
            OnOptionsPopOut?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnMainMenuReveal?.Invoke(this, EventArgs.Empty);
        }
        
        private void MainMenuOptionsUI_OnCancelTrigger(object sender, EventArgs e)
        {
            OnOptionsPopOut?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnMainMenuReveal?.Invoke(this, EventArgs.Empty);
        }
        
        private void MainMenuSelectPlayer_OnReturnTrigger(object sender, EventArgs e)
        {
            OnTransitionMainMenu?.Invoke(this, EventArgs.Empty);
            _midWayAnimationAction = () => OnMainMenuReveal?.Invoke(this, EventArgs.Empty);
        }
    }
}