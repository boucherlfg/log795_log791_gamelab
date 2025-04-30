using System;
using System.Linq;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects.Bonus;
using UnityEngine;

namespace GameLab.Bonus
{
    public class Bonus : MonoBehaviour
    {
        // TODO - Add Elements that will acts like an interface for the other bonus type.
        private AbstractBonusConfig _bonusConfig;
        public AbstractBonusConfig BonusConfig
        {
            set => _bonusConfig = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody.CompareTag("Player") && !other.attachedRigidbody.CompareTag("Shadow"))
            {
                return;
            }

            // // CHECK IF THEY ALREADY HAVE AN AURA OF THE SAME TYPE
            if(GetComponents<Aura>().Any(comp => comp.BonusConfig.BonusType == _bonusConfig.BonusType))
            {
                return;
            }

            // ADD THE AURA
            GameObject childObject = new GameObject("Aura");
            childObject.transform.parent = other.attachedRigidbody.transform;
            childObject.transform.localPosition = Vector3.zero;

            SphereCollider childCollider = childObject.AddComponent<SphereCollider>();
            childCollider.isTrigger = true;
            childCollider.radius = _bonusConfig.auraRadius;

            childObject.layer = LayerMask.NameToLayer("Bonus");

            Aura aura = childObject.AddComponent<Aura>();
            aura.BonusConfig = _bonusConfig;
            BonusManagerEvents.RegisterAura.Invoke(aura);
            Destroy(gameObject);
        }
    }
}