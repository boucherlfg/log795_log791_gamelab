using GameLab.Bonus;
using GameLab.Core;
using GameLab.Enums;
using UnityEngine;

namespace GameLab.ScriptableObjects.Bonus
{
    public abstract class AbstractBonusConfig: ScriptableObject
    {
        public float auraRadius = 0.75f;
        public abstract EBonusType BonusType { get; }

        public GameObject GetAffectedObject(GameObject gameObject)
        {
            // SEEMS LIKE A DIRTY CAST, BUT SHOULD ALWAYS BE APPLIED ON A MONOBEHAVIOUR ANYWAY
            MonoBehaviour auraReceiver = (MonoBehaviour)gameObject.GetComponentInChildren<IAuraReceiver>();
            if (auraReceiver)
            {
                return auraReceiver.gameObject;
            }

            return null;
        }

        public abstract void AffectPositive(Aura aura);
    }
}