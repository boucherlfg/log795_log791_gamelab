using System;
using GameLab.Managers;
using GameLab.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameLab.UI.MainMenu
{
    public class MainMenuSelectPlayerUI: MonoBehaviour
    {
        public static MainMenuSelectPlayerUI Instance { get; private set; }

        public event EventHandler OnReturnTrigger;
        
        [SerializeField] private Button returnButton;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(MainMenuSelectPlayerUI)}! {transform}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            MainMenuHandler.Instance.OnMainMenuReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnOptionsReveal += (_, _) => Hide();
            MainMenuHandler.Instance.OnSelectPlayerReveal += (_, _) => Show();
            
            returnButton.onClick.AddListener(() => OnReturnTrigger?.Invoke(this, EventArgs.Empty));
            
            Hide();
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(returnButton.gameObject);
        }
        private void Hide() => gameObject.SetActive(false);
    }
}