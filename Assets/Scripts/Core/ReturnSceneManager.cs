using System;
using GameLab.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLab.Core
{
    public class ReturnSceneManager: MonoBehaviour
    {
        [SerializeField] private SceneLoader.SceneTarget targetScene = SceneLoader.SceneTarget.MainMenu;
        
        private CustomUI_InputActions _inputActions;

        private void Awake()
        {
            _inputActions = new CustomUI_InputActions();
        }

        private void OnEnable()
        {
            _inputActions.UI.Enable();
        }

        private void OnDisable()
        {
            _inputActions.UI.Disable();
        }

        private void Start()
        {
            _inputActions.UI.Cancel.performed += _ =>
            {
                SceneLoader.Load(targetScene);
            };
        }
    }
}