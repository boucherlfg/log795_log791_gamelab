using System.Collections.Generic;
using GameLab.Core;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;
using GameLab.Utility;
using UnityEngine.Serialization;

namespace GameLab.Enums
{
    public enum GameModeState
    {
        Initialization,
        Ready,
        GameStarted,
        GamePaused,
        GameOver
    }
  
}
