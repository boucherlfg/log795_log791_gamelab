using System;
using System.Collections.Generic;
using UnityEngine;
using GameLab.Enums;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Gamelab/TutorialConfig")]
    public class TutorialConfig : ScriptableObject
    {
        [Serializable]
        public struct TutorialStep
        {
            public Sprite image;
            [TextArea]
            public string instruction;
        }

        [SerializeField] private List<TutorialStep> tutorialSteps;

        public List<TutorialStep> TutorialSteps
        {
            get => tutorialSteps;
        }
    }
}
