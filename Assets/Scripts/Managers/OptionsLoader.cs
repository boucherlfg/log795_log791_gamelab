using System;
using UnityEngine;

namespace GameLab.Managers
{
    //Need this to be executed before the BootstrapLoader in the bootstrap scene
    [DefaultExecutionOrder(-40)]
    public class OptionsLoader: MonoBehaviour
    {
        public static OptionsLoader Instance { get; private set; }

        private const string MasterVolumeAcronym = "MasterVolume";
        private const string MusicVolumeAcronym = "MusicVolume";
        private const string SfxVolumeAcronym = "SoundEffectsVolume";
        private const float AudioDefaultValue = 0.5f;
        
        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"{nameof(OptionsLoader)} already exists. {transform}");
                Destroy(gameObject);
                return;
            }    
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void InitManager()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeAcronym, AudioDefaultValue);
            if (MasterVolume > 1f)
            {
                MasterVolume /= 100f;
                PlayerPrefs.SetFloat(MasterVolumeAcronym, MasterVolume);
            }
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeAcronym, AudioDefaultValue);
            if (MusicVolume > 1f)
            {
                MusicVolume /= 100f;
                PlayerPrefs.SetFloat(MusicVolumeAcronym, MusicVolume);
            }
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeAcronym, AudioDefaultValue);
            if (SfxVolume > 1f)
            {
                SfxVolume /= 100f;
                PlayerPrefs.SetFloat(SfxVolumeAcronym, SfxVolume);
            }
        }

        public void SetMasterVolume(float value) 
        {
            MasterVolume = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(MasterVolumeAcronym, MasterVolume);
        }
        public void SetMusicVolume(float value) 
        {
            MusicVolume = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(MusicVolumeAcronym, MusicVolume);
        }
        public void SetSfxVolume(float value) 
        {
            SfxVolume = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(SfxVolumeAcronym, SfxVolume);
        }
    }
}