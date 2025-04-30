using System;
using System.Linq;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Managers;
using GameLab.Menu;
using GameLab.Player;
using GameLab.UI.MainMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class EndGameUI: MonoBehaviour
    {
        public static EndGameUI Instance { get; private set; }

        public event EventHandler OnMainMenuTrigger;
        
        [SerializeField] private Button replayButton;
        [SerializeField] private Button mainMenuButton;
        
        [Header("Player 1")] 
        [SerializeField] private Text firstPlayerName;
        [SerializeField] private Text firstPlayerScore;

        [Header("Player 2")] 
        [SerializeField] private Text secondPlayerName;
        [SerializeField] private Text secondPlayerScore;
        
        [Header("Win")]
        [SerializeField] private Text winnerText;
        
        private Action _onMidWayTransition;
        private Color[] _colors;
        private Color _baseColor;
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(EndGameUI)}! {transform}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            var colorChanger = FindFirstObjectByType<PlayerColorChangerRevamped>();
            _colors = colorChanger.playerConfigs.PlayerColors;
            _baseColor = winnerText.color;
            
            firstPlayerName.color = _colors[0];
            secondPlayerName.color = _colors[1];
            
            GameEvents.GameEnded.AddListener(Show);
            TransitionUI.Instance.OnMidWayTransition += (_, _) => _onMidWayTransition();
            
            replayButton.onClick.AddListener(() => GameEvents.GameRestarted.Invoke());
            mainMenuButton.onClick.AddListener(() =>
            {
                _onMidWayTransition = () => SceneLoader.Load(SceneLoader.SceneTarget.MainMenu);
                OnMainMenuTrigger?.Invoke(this, EventArgs.Empty);
            });
            
            _onMidWayTransition = () => { };

            GameEvents.TotalPointsUpdated.AddListener((playerNumbers) =>
            {
                var score1 = ScoreWinnerData.Instance.Player1;
                var score2 = ScoreWinnerData.Instance.Player2;
                firstPlayerScore.text = score1.ToString();
                secondPlayerScore.text = score2.ToString();

                if (score1 > score2)
                {
                    winnerText.text = "Player 1";
                    winnerText.color = _colors[0];
                }
                else if (score2 > score1)
                {
                    winnerText.text = "Player 2";
                    winnerText.color = _colors[1];
                }
                else
                {
                    winnerText.text = "Draw";
                    winnerText.color = _baseColor;
                }

            });

            Hide();
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(replayButton.gameObject);
            InputSystem.EnableDevice(Mouse.current);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            InputSystem.DisableDevice(Mouse.current);
        }
    }
}