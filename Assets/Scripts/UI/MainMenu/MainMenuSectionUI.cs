using System;
using System.Collections;
using FMODUnity;
using GameLab.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventHandler = System.EventHandler;

namespace GameLab.UI.MainMenu
{
    public class MainMenuSectionUI : MonoBehaviour
    {
        public static MainMenuSectionUI Instance { get; private set; }

        public event EventHandler OnNewGameTrigger;
        public event EventHandler OnOptionTrigger;
        public event EventHandler OnCreditTrigger;
        public event EventHandler OnExitTrigger;

        [Header("Buttons")] 
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button optionGameButton;
        [SerializeField] private Button creditGameButton;
        [SerializeField] private Button exitGameButton;

        private Button _lastSelectedButton;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(MainMenuSectionUI)}! {transform}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private IEnumerator Start()
        {
            MainMenuHandler.Instance.OnOptionsReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnCreditReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnSelectPlayerReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnMainMenuReveal += (_, _) => Show();
            
            newGameButton.onClick.AddListener(() => OnNewGameTrigger?.Invoke(this, EventArgs.Empty));
            optionGameButton.onClick.AddListener(() =>
            {
                _lastSelectedButton = optionGameButton;
                OnOptionTrigger?.Invoke(this, EventArgs.Empty);
            });
            creditGameButton.onClick.AddListener(() =>
            {
                _lastSelectedButton = creditGameButton;
                OnCreditTrigger?.Invoke(this, EventArgs.Empty);
            });
            exitGameButton.onClick.AddListener(() => OnExitTrigger?.Invoke(this, EventArgs.Empty));
            
            _lastSelectedButton = newGameButton;
            // EventSystem.current.SetSelectedGameObject(_lastSelectedButton.gameObject);

            yield return null;

            newGameButton.GetComponent<Animator>().enabled = true;
            optionGameButton.GetComponent<Animator>().enabled = true;
            creditGameButton.GetComponent<Animator>().enabled = true;
            exitGameButton.GetComponent<Animator>().enabled = true;
            _lastSelectedButton.Select();
        }

        private void Show()
        {
            gameObject.SetActive(true);
            // EventSystem.current.SetSelectedGameObject(_lastSelectedButton.gameObject);
            _lastSelectedButton.Select();
        }

        private void Hide() => gameObject.SetActive(false);
    }
}