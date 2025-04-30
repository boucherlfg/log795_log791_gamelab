using GameLab.ScriptableObjects;
using GameLab.Utility;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Cameras
{
    [DefaultExecutionOrder(-5)]
    public class GameCameraManager : MonoBehaviour
    {
        [SerializeField] private CameraConfig cameraConfig;

        [Header("Cameras")]
        [SerializeField] private GameObject dollyCamera;
        [SerializeField] private GameObject playerCamera;
        
        [Header("Targets")]
        [SerializeField] private CinemachineTargetGroup targetGroup;

        [Header("Events")]
        public readonly UnityEvent OnDollyAnimationDone = new();

        private AnimationCurveObjectUtility _animationCurveObjectUtility;
        private Transform _objectiveTransform;
        private Transform _playerTransform;
        private Transform[] _playerTransforms;

        private void Start()
        {
            if (!TryGetComponent(out _animationCurveObjectUtility))
            {
                _animationCurveObjectUtility = gameObject.AddComponent<AnimationCurveObjectUtility>();
            }
            _animationCurveObjectUtility.animationCurve = cameraConfig.DollyPositionOverTime;
            _animationCurveObjectUtility.OnAnimationStart.AddListener(OnDollyAnimationStart);
            _animationCurveObjectUtility.OnAnimationTick.AddListener(OnDollyAnimationTick);
            _animationCurveObjectUtility.OnAnimationEnd.AddListener(OnDollyAnimationEnd);
            playerCamera.GetComponent<CinemachineCamera>().enabled = false;
        }

        public void SetObjectiveTransform(Transform newObjectiveTransform)
        {
            _objectiveTransform = newObjectiveTransform;
        }

        public void SetPlayerTransforms(Transform[] newPlayerTransforms)
        {
            targetGroup.Targets.Clear();
            foreach (Transform newPlayerTransform in newPlayerTransforms)
            {
                targetGroup.Targets.Add(new CinemachineTargetGroup.Target
                {
                    Object =  newPlayerTransform,
                    Radius = cameraConfig.ScreenEdgeGap
                });
                SetPlayerTransform(targetGroup.transform);
            }
        }
        public void SetPlayerTransform(Transform newPlayerTransform)
        {
            _playerTransform = newPlayerTransform;

            switch (cameraConfig.CameraMode)
            {
                case CameraMode.ZommingAndDolly:

                    playerCamera.GetComponent<CinemachineCamera>().Follow = _playerTransform;
                    playerCamera.GetComponent<CinemachineCamera>().ForceCameraPosition(_playerTransform.position + playerCamera.GetComponent<CinemachineFollow>().FollowOffset, playerCamera.transform.rotation);
                    break;
                default:
                    break;
            }
        }

        public void StartAnimation()
        {
            _animationCurveObjectUtility.StartAnimation();
        }

        public void SwitchToDollyCamera()
        {
            Debug.Log("Switching to dolly camera");
            dollyCamera.GetComponent<CinemachineCamera>().enabled = true;
            playerCamera.GetComponent<CinemachineCamera>().enabled = false;
        }

        public void SwitchToPlayerCamera()
        {
            Debug.Log("Switching to player camera");
            playerCamera.GetComponent<CinemachineCamera>().enabled = true;
            dollyCamera.GetComponent<CinemachineCamera>().enabled = false;
        }

        private void OnDollyAnimationStart()
        {
            dollyCamera.GetComponent<CinemachineCamera>().ForceCameraPosition(_objectiveTransform.position + cameraConfig.LocationOffsetDolly, dollyCamera.transform.rotation);

            switch (cameraConfig.CameraMode)
            {
                case CameraMode.FixedDynamic:
                    playerCamera.GetComponent<DynamicOffset>().enabled = false;
                    break;
            }
        }
        private void OnDollyAnimationTick(float lerpVal)
        {
            switch (cameraConfig.CameraMode)
            {
                case CameraMode.ZommingAndDolly:
                    dollyCamera.transform.position = Vector3.Lerp(_objectiveTransform.position, targetGroup.transform.position, lerpVal) + cameraConfig.LocationOffsetDolly;
                    break;
                case CameraMode.FixedDynamic:
                    dollyCamera.transform.position = Vector3.Lerp(_objectiveTransform.position + cameraConfig.LocationOffsetDolly, playerCamera.transform.position, lerpVal);
                    break;
            }

        }
        private void OnDollyAnimationEnd()
        {
            switch (cameraConfig.CameraMode)
            {
                case CameraMode.ZommingAndDolly:
                    dollyCamera.GetComponent<CinemachineCamera>().ForceCameraPosition(targetGroup.transform.position + cameraConfig.LocationOffsetDolly, dollyCamera.transform.rotation);
                    break;
                case CameraMode.FixedDynamic:
                    dollyCamera.GetComponent<CinemachineCamera>().ForceCameraPosition( playerCamera.transform.position, dollyCamera.transform.rotation);
                    playerCamera.GetComponent<DynamicOffset>().enabled = true;
                    break;
            }
            OnDollyAnimationDone.Invoke();
        }
    }
}
