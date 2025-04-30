using System;
using GameLab.ScriptableObjects;
using Unity.Cinemachine;
using UnityEngine;

namespace GameLab.Cameras
{
    public class DynamicOffset : CinemachineExtension
    {
        [SerializeField]
        private CinemachineTargetGroup targetGroup;

        [SerializeField] private CameraConfig cameraConfig;

        private Vector3 _targetPosition = Vector3.zero;
        private Vector3? _originPosition = null;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Body)
            {
                return;
            }

            if (!Camera.main)
            {
                return;
            }

            // SEEMED TO NO WORK PROPERLY IN START
            if (!_originPosition.HasValue)
            {
                _originPosition = vcam.transform.position;
            }

            // GET THE POSITION OF THE TARGET GROUP IN CAMERA SPACE AND CHANGED THE CAMERA SPACE TO BE BETWEEN -1,-1 to 1,1
            Vector3 positionOnScreen = Camera.main.WorldToViewportPoint(targetGroup.Sphere.position) * 2f - new Vector3(1f, 1f, 1f);

            // PROJECT TO 2D VECTOR
            positionOnScreen.z = 0;

            // CALCULATE NEW OFFSET
            Vector3 offset = Vector3.ClampMagnitude(positionOnScreen, 1f) * cameraConfig.MaxOffsetRadius;
            _targetPosition = (vcam.transform.rotation * offset) +_originPosition.Value;

            // MOVE
            vcam.ForceCameraPosition(Vector3.Lerp(transform.position, _targetPosition, cameraConfig.LerpStrength), vcam.transform.rotation);
        }
    }
}
