using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLab.ScriptableObjects
{
    [Serializable]
    public struct TagKnockbackTuple
    {
        public string tag;
        public float knockbackStrength;
    }

    [CreateAssetMenu(fileName = "KnockbacksConfig", menuName = "Gamelab/KnockbacksConfig", order = 0)]
    public class KnockbacksConfig : ScriptableObject
    {
        [SerializeField] private List<TagKnockbackTuple> knockbackByTagMap;
        [SerializeField] private float minBounceStrength;
        [SerializeField] private float maxVelocityHorizontalAfterKnockback = 20;
        [SerializeField] private float maxVelocityVerticalAfterKnockback = 10;
        [SerializeField] AnimationCurveObject disableDelayCurve;

        public List<TagKnockbackTuple> KnockbackByTagMap
        {
            get => knockbackByTagMap;
        }

        public float MinBounceStrength
        {
            get => minBounceStrength;
        }

        public AnimationCurveObject DisableDelayCurve
        {
            get => disableDelayCurve;
        }

        public float MaxVelocityHorizontalAfterKnockback
        {
            get => maxVelocityHorizontalAfterKnockback;
        }

        public float MaxVelocityVerticalAfterKnockback
        {
            get => maxVelocityVerticalAfterKnockback;
        }

        public bool TryGetKnockbackValue(string tag, out float knockbackStrength)
        {
            foreach (TagKnockbackTuple entry in knockbackByTagMap)
            {
                if (entry.tag == tag)
                {
                    knockbackStrength = entry.knockbackStrength;
                    return true;
                }
            }

            knockbackStrength = 0f;
            return false;
        }
    }
}