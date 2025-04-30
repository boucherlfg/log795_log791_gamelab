using GameLab.ScriptableObjects;
using Unity.Cinemachine;
using UnityEngine;

namespace GameLab.Cameras
{
    public class CinemachineFollowOffsetApplyer : MonoBehaviour
    {
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private CinemachineFollow cinemachineFollow;

        private void Awake()
        {
            cinemachineFollow.FollowOffset = cameraConfig.LocationOffsetDolly;
        }
    }
}
