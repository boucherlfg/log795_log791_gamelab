using System;
using FMODUnity;
using GameLab.Core;
using GameLab.Events;
using GameLab.Menu;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI
{
    public class SoundCategoryUI: MonoBehaviour
    {
         public static event EventHandler<SoundCategoryUI> OnAnySoundCategorySpawn;
         public static event EventHandler<AudioHandler.SoundInfo> OnAnySoundCategoryUpdate;
         public static void ClearStaticVariable()
         {
             OnAnySoundCategorySpawn = null;
             OnAnySoundCategoryUpdate = null;
         }

         [SerializeField] private AudioHandler.SoundType soundType;

        [Header("UI Elements")]
        [SerializeField] private Text categoryName;
        [SerializeField] private Text valueNumeric;
        [SerializeField] private Slider volumeSlider;
        
        private AudioHandler.SoundType _currentSoundType;

        private void OnEnable()
        {
            float amount = AudioHandler.Instance.GetSoundVolume(soundType);
            int displayValue = (int)(amount * 100f);
            volumeSlider.value = displayValue;
            valueNumeric.text = displayValue.ToString("0");
        }

        private void Start()
        {
            OnAnySoundCategorySpawn?.Invoke(this, this);
            
            volumeSlider.onValueChanged.AddListener(HandleValueChanged);
        }
        
        private void HandleValueChanged(float percentage)
        {
            valueNumeric.text = percentage.ToString("0");
            OnAnySoundCategoryUpdate?.Invoke(this, new AudioHandler.SoundInfo(soundType, percentage/100f));
            MainMenuEvents.OnMainMenuSoundPlayed.Invoke(MainMenuSoundType.Slider);
        }

        private void OnValidate()
        {
            if (soundType != _currentSoundType)
            {
                _currentSoundType = soundType;
                transform.name = $"{soundType}CategoryUI";
                categoryName.text = soundType.ToString();
            }
        }
    }
}