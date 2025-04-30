using System.Collections.Generic;
using System.Linq;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameLab.Behaviours
{
    public class ShadowScript : MovingObject
    {
        private int _vehicleId;
        private List<MementoEntry> _mementos = new();
        private bool _isAccelerating;
        private EnergyComponent _energyComponent;
        [SerializeField] private GameObject shadowDashVFX;
        private void Start()
        {
            dashVFXBlue = shadowDashVFX;
            dashVFXRed = shadowDashVFX;
            base.Start();
        }
        
        public void SetShadow(List<MementoEntry> positions, int playerId, int vehicleId)
        {
            _vehicleId = vehicleId;
            _mementos = new List<MementoEntry>(positions);
            _rigidbody = GetComponent<Rigidbody>();
            _energyComponent = GetComponent<EnergyComponent>();
            GameEvents.Shadows.TryAdd(vehicleId, new ShadowEvent());
            GameEvents.Shadows[vehicleId].Move.AddListener(Move);
            GameEvents.Shadows[vehicleId].Rotate.AddListener(Rotate);
            GameEvents.Shadows[vehicleId].Dash.AddListener(Dash);
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            RoundEvents.RoundIsEnding.AddListener(OnRoundIsEnding);
            IsRunNotStarted = true;
            InvokeOnSet(playerId);
            
            PrepareDashVFX((PlayerNumber)playerId);
        }

        private void OnRoundIsEnding()
        {
            Destroy(gameObject);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (GameEvents.Shadows.ContainsKey(_vehicleId))
            {
                GameEvents.Shadows[_vehicleId].Move.RemoveListener(Move);
                GameEvents.Shadows[_vehicleId].Rotate.RemoveListener(Rotate);
                GameEvents.Shadows[_vehicleId].Dash.RemoveListener(Dash);
            }

            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
            RoundEvents.RoundIsEnding.RemoveListener(OnRoundIsEnding);
        }

        private void Dash(bool dash)
        {
            if (dash == isDashing) return;
            
            isDashing = dash;
            ToggleDashVfx(isDashing);
        }

        private void Move(Vector3 destination)
        {
            transform.position = destination;

            // USED BY SHADOW PAINTABLE
            RaycastHit raycastHit;
            bool isDetecting = Physics.Raycast(transform.position, Vector3.down, out raycastHit,  0.9f * transform.localScale.y);
            IsOnGround = isDetecting && (raycastHit.transform.CompareTag("Floor") || raycastHit.transform.CompareTag("Rampe") || raycastHit.transform
                .CompareTag("Wall"));
        }
        
        private void Rotate(Quaternion rotation)
        {
            ballEffect.transform.rotation = rotation;
        }

        private void OnRunStarted()
        {
            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
            IsRunNotStarted = false;
        }

       

        bool HasMementos(float time)
        {
            return _mementos.Any(x => time < x.Time);
        }
    }
}