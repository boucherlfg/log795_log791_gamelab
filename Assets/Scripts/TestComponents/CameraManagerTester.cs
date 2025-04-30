using GameLab.Cameras;
using GameLab.Managers;
using UnityEngine;

namespace GameLab.TestComponents
{
    [DefaultExecutionOrder(10)]
    public class CameraManagerTester : MonoBehaviour
    {
        [SerializeField] private GameCameraManager gameCameraManager;

        private void Start()
        {
            gameCameraManager.SwitchToDollyCamera();
            gameCameraManager.StartAnimation();

            Debug.Log("Starting Dolly Animation");
            gameCameraManager.OnDollyAnimationDone.AddListener(() =>
            {
                Debug.Log("Dolly Animation done");
                gameCameraManager.SwitchToPlayerCamera();
            });
        }
    }
}
