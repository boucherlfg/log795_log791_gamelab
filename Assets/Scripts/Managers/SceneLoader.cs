using GameLab.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GameLab.Managers
{
    public static class SceneLoader
    {
        // The name of the enum must be the same as the scene's name
        public enum SceneTarget
        {
            MainMenu,
            LoadingScene,
            Lobby, 
            SamirTest,
            Game, 
            GameColor, 
            LevelSelector,
            GameEnd,
        }

        public static bool WasDirectLoaded {get; private set;}
        public static Sprite loadingScreenSplashart = null;
        private static SceneTarget _target;

        //Pass before on the LoadingScene before going to the select scene
        public static void Load(SceneTarget target)
        {
            WasDirectLoaded = false;
            _target = target;

            SceneManager.LoadScene(SceneTarget.LoadingScene.ToString());
        }
        
        //Load the select scene directly
        public static void LoadDirect(SceneTarget target)
        {
            WasDirectLoaded = true;
            SceneManager.LoadScene(target.ToString());
            SceneManager.sceneLoaded += SceneLoaded;
        }

        //Callback used in the LoadingScene to load the selected scene
        public static void LoaderCallBack()
        {
            SceneManager.LoadScene(_target.ToString());
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            SoundEvents.OnSceneChanged.Invoke(_target);
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}