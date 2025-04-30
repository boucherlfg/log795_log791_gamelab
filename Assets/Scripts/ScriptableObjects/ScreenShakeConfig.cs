using System;
using System.Collections.Generic;
using GameLab.Enums;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ScreenShakeConfig", menuName = "Gamelab/ScreenShakeConfig", order = 0)]
    public class ScreenShakeConfig : ScriptableObject
    {
        [Serializable]
        public struct ScreenShakeTuple
        {
            public EScreenShakeSource source;
            [CanBeNull] public NoiseSettings config;

            public float duration;
        }

        [SerializeField] private List<ScreenShakeTuple> screenShaketuples = new()
        {
            new ScreenShakeTuple() { source = EScreenShakeSource.CanonShoot, config = null, duration = 0.5f },
        };

        public bool TryGetConfig(EScreenShakeSource source, out ScreenShakeTuple tuple)
        {
            tuple = screenShaketuples.Find(x => x.source == source);
            return tuple.config != null;
        }
    }
}