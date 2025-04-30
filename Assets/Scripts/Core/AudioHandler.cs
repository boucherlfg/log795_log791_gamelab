using System;
using System.Collections.Generic;
using GameLab.Managers;
using GameLab.UI;
using GameLab.UI.MainMenu;
using UnityEngine;

namespace GameLab.Core
{
    //Need this to be executed before the SoundCategoryUI
    [DefaultExecutionOrder(-20)]
    public class AudioHandler: MonoBehaviour
    {
        public static AudioHandler Instance { get; private set; }
        
        public enum SoundType
        {
            Master,
            Musique,
            SFX
        }

        [FMODUnity.ParamRef][SerializeField] private string masterParamName;
        [FMODUnity.ParamRef][SerializeField] private string musicParamName;
        [FMODUnity.ParamRef][SerializeField] private string sfxParamName;

        private float _masterTmpValue;
        private float _musicTmpValue;
        private float _sfxTmpValue;

        
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"There is more than one instance of {typeof(AudioHandler)}! {transform}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            SoundCategoryUI.OnAnySoundCategoryUpdate += SoundCategoryUI_OnAnySoundCategoryUpdate;
            
            MainMenuOptionsUI.Instance.OnOpenPage += MainMenuOptionsUI_OnOpenPage;
            MainMenuOptionsUI.Instance.OnSaveTrigger += MainMenuOptionsUI_OnSaveTrigger;
            MainMenuOptionsUI.Instance.OnCancelTrigger += (_, _) => UpdateValueToAudioSources();

            UpdateValueToAudioSources();
        }

        #region Events
        private void SoundCategoryUI_OnAnySoundCategoryUpdate(object sender, SoundInfo e)
        {
            switch (e.Type)
            {
                case SoundType.Master:
                    _masterTmpValue = e.Volume;
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName(masterParamName, e.Volume * 100f);
                    break;
                
                case SoundType.Musique:
                    _musicTmpValue = e.Volume;
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName(musicParamName, e.Volume * 100f);
                    break;
                
                case SoundType.SFX:
                    _sfxTmpValue = e.Volume;
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName(sfxParamName, e.Volume * 100f);
                    break;
                
                default:
                    Debug.LogWarning($"Type {e.Type} is not supported!");
                break;
            }
        }
        
        private void MainMenuOptionsUI_OnOpenPage(object sender, EventArgs e)
        {
            _masterTmpValue = OptionsLoader.Instance.MasterVolume;
            _musicTmpValue = OptionsLoader.Instance.MusicVolume;
            _sfxTmpValue = OptionsLoader.Instance.SfxVolume;
        }
        
        private void MainMenuOptionsUI_OnSaveTrigger(object sender, EventArgs e)
        {
            OptionsLoader.Instance.SetMasterVolume(_masterTmpValue);
            OptionsLoader.Instance.SetMusicVolume(_musicTmpValue);
            OptionsLoader.Instance.SetSfxVolume(_sfxTmpValue);
            
            UpdateValueToAudioSources();
        }
        #endregion

        private void UpdateValueToAudioSources()
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(masterParamName, OptionsLoader.Instance.MasterVolume * 100f);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(musicParamName, OptionsLoader.Instance.MusicVolume * 100f);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(sfxParamName, OptionsLoader.Instance.SfxVolume * 100f);
        }
        
        public float GetSoundVolume(SoundType soundType)
        {
            return soundType switch
            {
                SoundType.Master => OptionsLoader.Instance.MasterVolume,
                SoundType.Musique => OptionsLoader.Instance.MusicVolume,
                SoundType.SFX => OptionsLoader.Instance.SfxVolume,
                _ => -1
            };
        }

        public struct SoundInfo
        {
            public SoundType Type { get; private set; }
            public float Volume { get; private set; }

            public SoundInfo(SoundType type, float volume)
            {
                Type = type;
                Volume = volume;
            }
        }
    }
}