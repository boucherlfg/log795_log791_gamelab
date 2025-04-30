using GameLab.Managers;
using UnityEngine.Events;

namespace GameLab.Events
{
    public static class SoundEvents
    {
        public enum MainMenuState
        {
            MAIN = 0,
            SETTING = 1,
            LOBBY = 2,
            CREDITS = 3
        }

        public static readonly string MainMenuParam = "MenuState";
        public static readonly string GameParam = "GameState";


        /// <summary>
        /// Event used to change the BGM depending of the map
        /// </summary>
        public static readonly UnityEvent<SceneLoader.SceneTarget> OnSceneChanged = new();

        /// <summary>
        /// Event used to request to update a param for a specific bgm with a value of type float
        /// <param>BGMType: Element that will be updated</param>
        /// <param>string: The param name</param>
        /// <param>float: The value</param>
        /// </summary>
        public static readonly UnityEvent<string, float> OnChangeBGMGlobalParam = new();
    }
}