using GameLab.Bonus;
using GameLab.Constructions;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Player;
using UnityEngine;

namespace GameLab.ScriptableObjects.Bonus
{
    [CreateAssetMenu(fileName = "MetalBonus", menuName = "Gamelab/Bonus/MetalBonus", order = 0)]
    public class MetalBonusConfig : AbstractBonusConfig
    {
        public override EBonusType BonusType
        {
            get => EBonusType.Metal;
        }

        [SerializeField] private float metalScaleFactor = 1f;
        [SerializeField] private float metalVitesseMax = 1f;
        [SerializeField] private float metalSlowDuration = 2f;
        [SerializeField] private float metalTimeInCanon = 0.5f;

        public float MetalScaleFactor  {get => metalScaleFactor;}
        public float MetalVitesseMax {get => metalVitesseMax;}
        public float MetalSlowDuration {get => metalSlowDuration;}
        public float MetalTimeInCanon {get => metalTimeInCanon;}


        public override void AffectPositive(Aura aura)
        {
            Transform parent = aura.transform.parent;

            parent.localScale *= MetalScaleFactor;
        }
    }
}