using System;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using UnityEngine;

namespace GameLab.Constructions.Canons
{
    internal class CanonPlayerLoadingState : CanonState
    {
        public event EventHandler OnReadyToShootCooldown;
        public event EventHandler<MovingObject> OnReadyToShootReplace;

        private float _timeInCannon;

        public CanonPlayerLoadingState(CanonConstruct canon) : base(canon) { }

        public override void EnterState()
        {
            _canon.OnObjectCollideWithCatchRadius += CanonConstruct_OnObjectCollideWithCatchRadius;
            if (_canon.ObjectHeld.TryGetComponent(out PlayerScript player))
                GamepadEvents.CallVibration.Invoke((PlayerNumber)player.Id, VibrationSource.CanonEnter);

            _timeInCannon = 0;

            _canon.ObjectHeld.CanonGrab();
            _canon.SetCanonFlashVFX(true);
            _canon.ObjectHeld.transform.position = _canon.posInCanon.position;
            _canon.ObjectHeld.transform.SetParent(_canon.posInCanon);
            _canon.ObjectHeld.DeactivateCollider();
        }

        public override void ExitState()
        {
            _canon.OnObjectCollideWithCatchRadius -= CanonConstruct_OnObjectCollideWithCatchRadius;
        }

        public override void UpdateState()
        {
            _timeInCannon += Time.deltaTime;
            _canon.SetLineRendererRation(Mathf.Lerp(_canon.lineCooldown, _canon.lineLoading, _timeInCannon / _canon.TimeInCanon));

            if (_timeInCannon >= _canon.TimeInCanon)
                OnReadyToShootCooldown?.Invoke(this, EventArgs.Empty);
        }
        
        private void CanonConstruct_OnObjectCollideWithCatchRadius(object sender, MovingObject movingObject)
        {
            if (movingObject.CompareTag(CanonConstruct.PlayerTag) && _canon.ObjectHeld) 
                return;
            
            OnReadyToShootReplace?.Invoke(this, movingObject);

            if (movingObject.TryGetComponent(out VehicleData vehicleData))
                _canon.OnCanonTaken.Invoke(vehicleData.PlayerNumber);
        }
    }
}