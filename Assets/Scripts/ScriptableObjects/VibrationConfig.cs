using System;
using System.Collections.Generic;
using UnityEngine;
using GameLab.Enums;
namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "VibrationConfig", menuName = "Gamelab/VibrationConfig")]
    public class VibrationConfig : ScriptableObject
    {
        [Serializable]
        public struct VibrationMessage
        {
            public VibrationSource source;
            public AnimationCurve curve;
        }

        [SerializeField] private List<VibrationMessage> vibrationMessages;

        public bool TryGetCurve(VibrationSource source, out AnimationCurve curve)
        {
            curve = vibrationMessages.Find(x => x.source == source).curve;
            return curve != null;
        }
    }
}
