using GameLab.Cameras;
using UnityEngine;

namespace GameLab.ScriptableObjects
{
    public enum CameraMode
    {
        ZommingAndDolly,
        FixedDynamic,
    }

    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Gamelab/CameraConfig", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        [Header("Manager Setting")]
        [SerializeField] private CameraMode cameraMode = CameraMode.ZommingAndDolly;
        [SerializeField] private Vector3 locationOffsetDolly;
        [SerializeField] private AnimationCurveObject dollyPositionOverTime;

        [Header("ZommingAndDolly Camera Settings")]
        [SerializeField] private float screenEdgeGap = 12f;

        [Header("Fixed Dynamic Camera Settings")]
        [SerializeField] private float maxOffsetRadius = 10;
        [SerializeField] private float lerpStrength = 0.5f;

        // Manager Setting
        public CameraMode CameraMode { get => cameraMode; }
        public Vector3 LocationOffsetDolly { get => locationOffsetDolly; }
        public AnimationCurveObject DollyPositionOverTime { get => dollyPositionOverTime; }

        // ZommingAndDolly Camera Settings
        public float ScreenEdgeGap { get => screenEdgeGap; }

        // Fixed Dynamic Camera Settings

        public float MaxOffsetRadius { get => maxOffsetRadius; }
        public float LerpStrength { get => lerpStrength; }
    }
}