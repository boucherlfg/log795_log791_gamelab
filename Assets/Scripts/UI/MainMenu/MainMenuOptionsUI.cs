using System;
using FMODUnity;
using GameLab.Events;
using GameLab.Menu;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventHandler = System.EventHandler;

namespace GameLab.UI.MainMenu
{
    public class MainMenuOptionsUI : MonoBehaviour
    {
        public static MainMenuOptionsUI Instance { get; private set; }

        public event EventHandler OnOpenPage;
        public event EventHandler OnSaveTrigger;
        public event EventHandler OnCancelTrigger;
        public event EventHandler OnMidWayAnimation;

        private const string PopUp = "MainMenu_OptionPopUp";
        private const string PopOut = "MainMenu_OptionPopOut";

        [SerializeField] private Animator anim;

        [Header("Buttons")] [SerializeField] private Button okButton;

        private CustomUI_InputActions _inputActions;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(MainMenuOptionsUI)}! {transform}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _inputActions = new CustomUI_InputActions();
        }

        private void OnEnable()
        {
            _inputActions.UI.Enable();
            OnOpenPage?.Invoke(this, EventArgs.Empty);
            EventSystem.current.SetSelectedGameObject(okButton.gameObject);
        }

        private void OnDisable()
        {
            _inputActions.UI.Disable();
        }

        private void Start()
        {
            MainMenuHandler.Instance.OnSelectPlayerReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnOptionsReveal += (_, _) => Show();
            MainMenuHandler.Instance.OnOptionsPopUp += MainMenuHandler_OnOptionsPopUp;
            MainMenuHandler.Instance.OnOptionsPopOut += MainMenuHandler_OnOptionsPopOut;

            okButton.onClick.AddListener(() => OnSaveTrigger?.Invoke(this, EventArgs.Empty));

            _inputActions.UI.Cancel.performed += _ =>
            {
                MainMenuEvents.OnMainMenuSoundPlayed.Invoke(MainMenuSoundType.Back);
                OnCancelTrigger?.Invoke(this, EventArgs.Empty);
            };
            Hide();
        }

        private void MainMenuHandler_OnOptionsPopUp(object sender, EventArgs e)
        {
            Show();
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.MainMenuParam, (float)SoundEvents.MainMenuState.SETTING);
            anim.Play(PopUp);
        }

        private void MainMenuHandler_OnOptionsPopOut(object sender, EventArgs e)
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
            EventSystem.current.SetSelectedGameObject(okButton.gameObject);
        }

        private void Hide() => gameObject.SetActive(false);
    }
}