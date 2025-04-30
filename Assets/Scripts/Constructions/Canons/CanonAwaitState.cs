using System;
using System.Linq;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Player;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Constructions.Canons
{
    internal class CanonAwaitState : CanonState
    {
        public event EventHandler OnObjectEntered;

        public CanonAwaitState(CanonConstruct canon) : base(canon) { }

        public override void EnterState()
        {
            _canon.OnObjectCollideWithCatchRadius += CanonConstruct_OnObjectCollideWithCatchRadius;
            _canon.SetLineRendererRation(_canon.lineAwait);

            if(_canon.inRange.Count() > 0)
            {
                foreach (MovingObject movingObject in _canon.inRange)
                {
                    if (movingObject is not PlayerScript)
                    {
                        CanonConstruct_OnObjectCollideWithCatchRadius(null, movingObject);
                        return;
                    }
                }
                CanonConstruct_OnObjectCollideWithCatchRadius(null, _canon.inRange[0]);   
            }
        }

        public override void ExitState()
        {
            _canon.OnObjectCollideWithCatchRadius -= CanonConstruct_OnObjectCollideWithCatchRadius;
        }

        public override void UpdateState() { }

        private void CanonConstruct_OnObjectCollideWithCatchRadius(object sender, MovingObject movingObject)
        {
            if (!movingObject)
                return;
            
            if (_canon.ObjectHeld || (!movingObject.CompareTag(CanonConstruct.PlayerTag) && !movingObject.CompareTag(CanonConstruct.ShadowTag))) return;
            if (_canon.ObjectHeld != movingObject)
            {
                _canon.ObjectHeld = movingObject;
                _canon.canonChargeSfx.SetParameter("CANON_POWER", Mathf.Clamp(_canon.cannonForce, 10, 56));
                _canon.canonChargeSfx.PlayWithTryCatch();

                OnObjectEntered?.Invoke(this, EventArgs.Empty);
            }

            if (movingObject.TryGetComponent(out VehicleData vehicleData))
            {
                _canon.OnCanonTaken.Invoke(vehicleData.PlayerNumber);
            }
        }
    }
}