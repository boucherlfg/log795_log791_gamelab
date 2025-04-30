using FMODUnity;
using GameLab.Events;
using GameLab.Managers;
using GameLab.Utility;
using UnityEngine;

namespace GameLab
{
    public class BGMManager : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter menuBGM;
        [SerializeField] private StudioEventEmitter gameBGM;
        [SerializeField] private StudioEventEmitter crowdAmb;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            SoundEvents.OnSceneChanged.AddListener(OnSceneLoaded);
            SoundEvents.OnChangeBGMGlobalParam.AddListener(OnChangeBGMParam);
        }

        private void OnChangeBGMParam(string param, float value)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(param, value);
        }

        private void OnSceneLoaded(SceneLoader.SceneTarget scene)
        {
            switch (scene)
            {
                case SceneLoader.SceneTarget.MainMenu:
                case SceneLoader.SceneTarget.Lobby:
                case SceneLoader.SceneTarget.LevelSelector:
                case SceneLoader.SceneTarget.GameEnd:
                    // START MENU AMBIANCE
                    if (!menuBGM.IsPlaying())
                    {
                        menuBGM.PlayWithTryCatch();
                    }
                    // STOP GAME AND CROWD AMBIANCE
                    crowdAmb.Stop();
                    gameBGM.Stop();
                    break;
                case SceneLoader.SceneTarget.Game:
                case SceneLoader.SceneTarget.GameColor:
                    // START CROWD
                    if (!crowdAmb.IsPlaying())
                    {
                        crowdAmb.PlayWithTryCatch();
                    }

                    // START GAME AMBIANCE
                    if (!gameBGM.IsPlaying())
                    {
                        gameBGM.PlayWithTryCatch();
                    }
                    // STOP MENU MUSIC
                    menuBGM.Stop();
                    break;
                default:
                    break;
            }
        }
    }
}
