using System;
using GameLab.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Instance { get; private set; }

        public event EventHandler OnResumeTrigger;
        public event EventHandler OnMainMenuTrigger;
        public event EventHandler OnRestartTrigger;

        private const string Spawn = "PauseUI_Spawn";
        private const string FadeOut = "PauseUI_FadeOut";

        [SerializeField] private Animator anim;
        [SerializeField] private GameObject background;
        
        [Header("Button")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button restartButton;
        
        private CustomUI_InputActions _inputActions;
        
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"{nameof(PauseMenuUI)} already exists. {transform}");
                Destroy(gameObject);
                return;
            }    
            
            Instance = this;
            _inputActions = new CustomUI_InputActions();
        }
        
        private void OnEnable()
        {
            _inputActions.UI.Enable();
            InputSystem.EnableDevice(Mouse.current);
        }

        private void OnDisable()
        {
            _inputActions.UI.Disable();
            InputSystem.DisableDevice(Mouse.current);
        }
        
        private void Start()
        {
            GameEvents.GamePaused.AddListener(OnGamePaused);
            
            resumeButton.onClick.AddListener(() => OnResumeTrigger?.Invoke(this, EventArgs.Empty));
            mainMenuButton.onClick.AddListener(() => OnMainMenuTrigger?.Invoke(this, EventArgs.Empty));
            restartButton.onClick.AddListener(() => OnRestartTrigger?.Invoke(this, EventArgs.Empty));
            
            _inputActions.UI.Cancel.performed += _ => OnResumeTrigger?.Invoke(this, EventArgs.Empty);
            
            Hide();
        }

        private void OnGamePaused(bool paused)
        {
            if (paused)
            {
                Show();
                anim.Play(Spawn);
            }
            else
                anim.Play(FadeOut);
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        private void Hide() => gameObject.SetActive(false);
    }
}
