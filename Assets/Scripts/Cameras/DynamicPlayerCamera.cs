using System;
using GameLab.Behaviours;
using GameLab.ScriptableObjects;
using Unity.Cinemachine;
using UnityEngine;

namespace GameLab.Cameras
{
    public class DynamicPlayerCamera : CinemachineExtension
    {
        [Header("Settings")] [SerializeField] private AnimationCurveObject zoomModifierAnimationCurve;
        [SerializeField] private AnimationCurveObject fovModifierAnimationCurve;

        [SerializeField] private AnimationCurveObject offsetModifierDistanceAnimationCurve;

        private CinemachineCamera _camera;
        private CinemachineFollow _follow;
        private float _initialFOV;
        private Vector3 _initialOffset;

        private Vector3 _lastOffset;

        private void Start()
        {
            _camera = gameObject.GetComponent<CinemachineCamera>();
            _follow = gameObject.GetComponent<CinemachineFollow>();

            _initialFOV = _camera.Lens.FieldOfView;
            _initialOffset = _follow.FollowOffset;

            _lastOffset = _follow.FollowOffset;
        }

        // UPDATE CAMERA STATE HERE
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state,
            float deltaTime)
        {
            // CHECK IF WE'RE ACTIVE
            if (!vcam.IsLive)
            {
                return;
            }

            if (stage != CinemachineCore.Stage.Body)
            {
                return;
            }

            // ONLY PROCESS CAMERA WHEN WE HAVE A PLAYER
            if (_camera == null)
            {
                return;
            }

            GameObject player = _camera.Follow.gameObject;
            if (player == null)
            {
                return;
            }

            if (player.TryGetComponent(out PlayerScript playerScript))
            {
                // APPLY EFFECT TO THE CAMERA
                float lerpVal = Mathf.Clamp(playerScript.MovementSpeed.magnitude / playerScript.MaxSpeed, 0f, 1f);

                // APPLY FOV
                float fovEffect = fovModifierAnimationCurve.AnimationCurve.Evaluate(lerpVal);
                _camera.Lens.FieldOfView = _initialFOV + fovEffect;

                // APPLY ZOOM
                float zoomEffect = zoomModifierAnimationCurve.AnimationCurve.Evaluate(lerpVal);
                Vector3 direction = _camera.transform.forward;
                Vector3 zoomOffset = direction * zoomEffect;

                // APPLY OFFSET
                float offsetDistance = offsetModifierDistanceAnimationCurve.AnimationCurve.Evaluate(lerpVal);
                Vector3 playerDirectionWorldSpace = player.transform.forward;
                Vector3 playerDirectionInCameraSpace = new Vector3(playerDirectionWorldSpace.x, playerDirectionWorldSpace.z, 0).normalized;
                Vector3 cameraDirectionInWorldSpace = _camera.transform.rotation * playerDirectionInCameraSpace;
                Vector3 movementOffset = cameraDirectionInWorldSpace * offsetDistance;

                Vector3 lerpedMovementOffset = Vector3.Lerp(_lastOffset, movementOffset, deltaTime);
                _lastOffset = lerpedMovementOffset;

                // UPDATE OFFSET
                _follow.FollowOffset = _initialOffset + zoomOffset + lerpedMovementOffset;
            }
        }
    }
}