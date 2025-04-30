using System;
using System.Collections.Generic;
using GameLab.Core;
using GameLab.Player;
using GameLab.ScriptableObjects;
using UnityEngine;

namespace GameLab.Constructions
{
    public class RampConstruction : MonoBehaviour, IAuraReceiver
    {
        private bool _hasCanonBeenAffected = false;
        public List<AbstractBonusEffector> BonusEffectors { get; set; } = new();

        public void Receive()
        {
            foreach (var bonusConfig in BonusEffectors)
            {
                bonusConfig.Affect(this);
            }
        }

        public void StartAffecting(AbstractBonusEffector effector)
        {
            if (_hasCanonBeenAffected)
            {
                return;
            }
            _hasCanonBeenAffected = true;
            effector.StartAffecting(this);
        }

        public void StopAffecting(AbstractBonusEffector effector)
        {
            effector.StopAffecting(this);
        }
    }
}