using GameLab.Managers;
using UnityEngine.Events;
using GameLab.Enums;
using GameLab.ScriptableObjects;

namespace GameLab.Events
{
    public static class RoundEvents
    {
        public static readonly UnityEvent<PlayerNumber> PlayerInitialized = new ();
        public static readonly UnityEvent<string> ObjectInitialized = new ();
        public static readonly UnityEvent RoundInitialized = new();
        public static readonly UnityEvent RoundIsStarting = new();
        public static readonly UnityEvent RunStarted = new();
        public static readonly UnityEvent TimesOut = new();
        public static readonly UnityEvent RoundIsEnding = new();
        public static readonly UnityEvent RunEnded = new();
        
        /// <summary>
        /// first is start round, second is total round
        /// </summary>
        public static readonly UnityEvent<int, int> RoundNumber = new ();
        /// <summary>
        /// this corresponds to the amount of point a round will give you if you win it
        /// </summary>
        public static readonly UnityEvent<Round> CurrentRoundUpdated = new();
        public static readonly UnityEvent<float> EnergyRatio = new ();
        public static readonly UnityEvent<float> EnergyRemaining = new();
    }
}