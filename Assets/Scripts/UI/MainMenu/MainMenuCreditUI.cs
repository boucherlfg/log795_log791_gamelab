using System;
using GameLab.Events;
using GameLab.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameLab.UI.MainMenu
{
    public class MainMenuCreditUI: MonoBehaviour
    {
        public static MainMenuCreditUI Instance { get; private set; }

        public event EventHandler OnOpenPage;
        public event EventHandler OnReturnTrigger;
        public event EventHandler OnMidWayAnimation;

        private const string PopUp = "MainMenu_OptionPopUp";
        private const string PopOut = "MainMenu_OptionPopOut";

        [SerializeField] private Animator anim;
        [SerializeField] private Button returnButton;
        
        private CustomUI_InputActions _inputActions;
        
        private void OnEnable()
        {
            _inputActions.UI.Enable();
            OnOpenPage?.Invoke(this, EventArgs.Empty);
        }

        private void OnDisable()
        {
            _inputActions.UI.Disable();
        }
        
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(MainMenuCreditUI)}! {transform}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _inputActions = new CustomUI_InputActions();
        }

        private void Start()
        {
            MainMenuHandler.Instance.OnSelectPlayerReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnCreditReveal += (_, _) => Show();
            MainMenuHandler.Instance.OnCreditPopUp += MainMenuHandler_OnCreditPopUp;
            MainMenuHandler.Instance.OnCreditPopOut += MainMenuHandler_OnCreditPopOut;

            returnButton.onClick.AddListener(() => { OnReturnTrigger?.Invoke(this, EventArgs.Empty); });

            _inputActions.UI.Cancel.performed += _ =>
            {
                MainMenuEvents.OnMainMenuSoundPlayed.Invoke(MainMenuSoundType.Back);
                OnReturnTrigger?.Invoke(this, EventArgs.Empty);
            };

            Hide();
        }

        private void MainMenuHandler_OnCreditPopUp(object sender, EventArgs e)
        {
            Show();
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.MainMenuParam, (float)SoundEvents.MainMenuState.CREDITS);
            anim.Play(PopUp);
        }

        private void MainMenuHandler_OnCreditPopOut(object sender, EventArgs e)
        {
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.MainMenuParam, (float)SoundEvents.MainMenuState.MAIN);
            anim.Play(PopOut);
        }

        private void MidWayAnimationTrigger()
        {
            OnMidWayAnimation?.Invoke(this, EventArgs.Empty);
        }

        private void Show()
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(returnButton.gameObject);
        }

        private void Hide() => gameObject.SetActive(false);
        
        
    }
}