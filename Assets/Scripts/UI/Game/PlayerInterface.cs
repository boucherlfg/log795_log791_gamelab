using GameLab.Events;
using UnityEngine;

namespace GameLab.UI.Game
{
    public class PlayerInterface : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameEvents.GamePaused.AddListener(OnGamePaused);
            RoundEvents.RoundInitialized.AddListener(HideHUD);
            RoundEvents.RoundIsStarting.AddListener(ShowHUD);
            RoundEvents.RoundIsEnding.AddListener(HideHUD);
            GameEvents.GameEnded.AddListener(HideHUD);
        }

        private void OnGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                HideHUD();
            }
            else
            {
                ShowHUD();
            }
        }

        private void OnDestroy()
        {
            RoundEvents.RoundInitialized.RemoveListener(HideHUD);
            RoundEvents.RoundIsStarting.RemoveListener(ShowHUD);
            RoundEvents.RoundIsEnding.RemoveListener(HideHUD);
        }

        private void ShowHUD()
        {
            canvasGroup.alpha = 1;
        }

        private void HideHUD()
        {
            canvasGroup.alpha = 0;
        }
    }
}
