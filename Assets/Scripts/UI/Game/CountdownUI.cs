using GameLab.Events;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class CountdownUI : MonoBehaviour
    {
        [SerializeField] private CountdownNumberUI countdownPrefab;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameModeTimer.Instance.OnCountdownChangeUI.AddListener(OnGameModeTimerChange);
            RoundEvents.RunStarted.AddListener(OnRoundStarted);
        }

        private void OnRoundStarted()
        {
            SpawnCountdown("Go!");
        }

        private void OnGameModeTimerChange(int arg0)
        {
            SpawnCountdown(Mathf.CeilToInt(arg0).ToString());
        }

        private void SpawnCountdown(string text)
        {
            Instantiate(countdownPrefab, transform).SetText(text);
        }
    }
}