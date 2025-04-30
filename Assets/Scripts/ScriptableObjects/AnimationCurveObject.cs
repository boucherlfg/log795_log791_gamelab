using UnityEngine;

namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AnimationCurveObject", menuName = "Gamelab/AnimationCurveObject", order = 0)]
    public class AnimationCurveObject : ScriptableObject
    {
        [SerializeField] private AnimationCurve animationCurve;

        public AnimationCurve AnimationCurve
        {
            get => animationCurve;
        }

        public float Duration
        {
            get => MaxX - MinX;
        }

        public float MinX
        {
            get => animationCurve[0].time;
        }

        public float MaxX
        {
            get => animationCurve.keys[^1].time;
        }
    }
}