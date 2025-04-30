using System;
using FMODUnity;
using GameLab.Events;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Menu
{
    [Serializable]
    public enum MainMenuSoundType
    {
        Hover,
        Click,
        Back,
        Slider
    }

    public class MainMenuSoundPlayer : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter hoverSound;
        [SerializeField] private StudioEventEmitter clickSound;
        [SerializeField] private StudioEventEmitter backSound;
        [SerializeField] private StudioEventEmitter sliderSound;

        private void Start()
        {
            MainMenuEvents.OnMainMenuSoundPlayed.AddListener(OnSoundPlayed);
        }

        private void OnDestroy()
        {
            MainMenuEvents.OnMainMenuSoundPlayed.RemoveListener(OnSoundPlayed);
        }

        /// <summary>
        /// Used for unity events in the editor only
        /// </summary>
        /// <param name="soundType">The type of sound to play</param>
        public void PlaySound(MainMenuSoundType soundType)
        {
            OnSoundPlayed(soundType);
        }

        private void OnSoundPlayed(MainMenuSoundType soundType)
        {
            switch (soundType)
            {
                case MainMenuSoundType.Back:
                    backSound.PlayWithTryCatch();
                    break;
                case MainMenuSoundType.Click:
                    clickSound.PlayWithTryCatch();
                    break;
                case MainMenuSoundType.Hover:
                    hoverSound.PlayWithTryCatch();
                    break;
                case MainMenuSoundType.Slider:
                    sliderSound.PlayWithTryCatch();
                    break;
            }
        }
    }
}
