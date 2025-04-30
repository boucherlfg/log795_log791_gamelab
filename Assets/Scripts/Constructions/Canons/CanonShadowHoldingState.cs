using System;
using UnityEngine;

namespace GameLab.Constructions.Canons
{
    public class CanonShadowHoldingState: CanonState
    {
        public event EventHandler OnShadowLeft;
        
        public CanonShadowHoldingState(CanonConstruct canon) : base(canon) { }

        public override void EnterState()
        {
            _canon.ObjectHeld.CanonGrab();
            _canon.SetCanonFlashVFX(true);
            _canon.ObjectHeld.transform.position = _canon.posInCanon.position;
            _canon.ObjectHeld.transform.SetParent(_canon.posInCanon);
            _canon.ObjectHeld.DeactivateCollider();
        }

        public override void ExitState() { }

        public override void UpdateState()
        {
            if(_canon.ObjectHeld && Vector3.Distance(_canon.ObjectHeld.transform.position, _canon.posInCanon.position) > 0.2f)
            {
                _canon.ObjectHeld.transform.SetParent(null);
                _canon.OnCanonShoot.Invoke();
                _canon.SetCanonFlashVFX(false);

                _canon.ObjectHeld.ColliderDeactivationTemporary();
                _canon.inRange.Remove(_canon.ObjectHeld);
                _canon.ObjectHeld = null;
                _canon.ActiveVFX();
                OnShadowLeft?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}