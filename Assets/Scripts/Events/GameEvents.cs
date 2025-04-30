using System.Collections.Generic;
using GameLab.ColorMode;
using GameLab.Core;
using GameLab.Enums;
using UnityEngine.Events;

namespace GameLab.Events
{
    public static class GameEvents
    {
        public static readonly UnityEvent GameRestarted = new ();
        public static readonly UnityEvent GameStarted = new ();
        public static readonly UnityEvent GameReady = new ();
        public static readonly UnityEvent GameEnded = new ();
        public static readonly UnityEvent<bool> GamePaused = new ();

        public static readonly UnityEvent RoundStarted = new ();
        public static readonly UnityEvent RoundEnded = new();
        public static readonly UnityEvent GoalReached = new ();
        public static readonly UnityEvent OnPodiumReady = new ();

        public static readonly UnityEvent<MementoPlayerInfo> MementoRequested = new ();

        public static readonly UnityEvent ButtonSouthTriggered = new();
        public static readonly UnityEvent PauseInputTriggered = new();
        public static readonly Dictionary<int, PlayerEvent> Players = new();
        public static readonly Dictionary<int, ShadowEvent> Shadows = new();
        public static readonly Dictionary<int, MovingObjectEvent> MovingObjects = new();
        public static readonly UnityEvent<string> PlayerTurn = new ();

        /// <summary>
        /// first int is the accumulated score of player one this round. Second int is for player 2
        /// </summary>
        public static readonly UnityEvent<int, int> AccumulatedScoreCalculated = new();

        /// <summary>
        /// first int corresponds to the car id located in the ScoreKeeper script. Second int corresponds to the score of that car
        /// </summary>
        public static readonly UnityEvent<Dictionary<int, int>> VehicleScoreCalculated = new();

        /// <summary>
        /// first int corresponds to player 1 amount of rounds won, second int corresponds to player 2
        /// </summary>
        public static readonly UnityEvent<List<PlayerNumber>> TotalPointsUpdated = new();

        //////// COLOR MODE /////////
        /// PREVIOUS OWNER, OBJECT
        public static readonly UnityEvent<PlayerNumber, IPaintable> OnPaintableUpdated = new();
        
    }
}