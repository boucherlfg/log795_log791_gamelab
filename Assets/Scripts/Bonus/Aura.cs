using System;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects.Bonus;
using UnityEngine;

namespace GameLab.Bonus
{
    public class Aura : MonoBehaviour
    {
        private AbstractBonusConfig _bonusConfig;
        private Vector3 _startScale;


        public AbstractBonusConfig BonusConfig
        {
            set => _bonusConfig = value;
            get => _bonusConfig;
        }

        private void Start() {}

        private void OnTriggerEnter(Collider other)
        {
            GameObject parent = other.transform.parent.gameObject;
            GameObject affectedObject = BonusConfig.GetAffectedObject(parent);
            // START AFFECTING
            if (affectedObject)
            {
                BonusManagerEvents.SubscribeElement.Invoke(BonusConfig.BonusType, affectedObject, this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            GameObject parent = other.transform.parent.gameObject;
            GameObject affectedObject = BonusConfig.GetAffectedObject(parent);
            // STOP AFFECTING
            if (affectedObject)
            {
                BonusManagerEvents.UnsubscribeElement.Invoke(BonusConfig.BonusType, affectedObject, this);
            }
        }
    }
}