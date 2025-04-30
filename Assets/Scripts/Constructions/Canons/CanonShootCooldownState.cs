using System;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Constructions.Canons
{
    internal class CanonShootCooldownState : CanonState
    {
        public event EventHandler OnCooldownOver;

        private float _cooldown;
        
        public CanonShootCooldownState(CanonConstruct canon) : base(canon) { }

        public override void EnterState()
        {
            if (_canon.ObjectHeld && _canon.ObjectHeld.TryGetComponent(out PlayerScript player))
            {
                GamepadEvents.CallVibration.Invoke((PlayerNumber)player.Id, VibrationSource.CanonExit);
            }

            _canon.SetLineRendererRation(_canon.lineCooldown);
            _canon.SetCanonFlashVFX(false);
            _canon.canonChargeSfx.Stop();
            _canon.canonShootSfx.PlayWithTryCatch();

            _cooldown = _canon.cooldownAfterShoot;

            if (!_canon.ObjectHeld)
            {
                return;
            }
            _canon.ObjectHeld.transform.SetParent(null);
            _canon.ObjectHeld.ExternalImpulsiveForceAffect(_canon.posInCanon, _canon.cannonForce, true);
            _canon.OnCanonShoot.Invoke();
            _canon.inRange.Remove(_canon.ObjectHeld);
            _canon.ObjectHeld = null;
            CameraEvents.CameraShakeEvent.Invoke(EScreenShakeSource.CanonShoot);

            _canon.ActiveVFX();
        }
        public override void ExitState() { }
        public override void UpdateState()
        {
            _cooldown -= Time.deltaTime;

            if (_cooldown <= 0)
                OnCooldownOver?.Invoke(this, EventArgs.Empty);
        }
    }
}