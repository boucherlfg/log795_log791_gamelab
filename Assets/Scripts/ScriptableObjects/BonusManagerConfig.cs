using System;
using System.Collections.Generic;
using GameLab.Core;
using GameLab.Enums;
using GameLab.ScriptableObjects.Bonus;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    [Serializable]
    public struct RoundBonusTypeTuple
    {
        public int roundNumber;
        public EBonusType bonusType;
    }

    [Serializable]
    public struct BonusTypePrefabTuple
    {
        public EBonusType bonusType;
        public GameObject prefab;
    }

    [CreateAssetMenu(fileName = "BonusConfig", menuName = "Gamelab/BonusConfig")]
    public class BonusManagerConfig : ScriptableObject
    {
        [SerializeField] private List<RoundBonusTypeTuple> roundBonusAssociationList = new();

        [SerializeField] private List<BonusTypePrefabTuple> bonusTypePrefabAssociationList = new();

        [SerializeField] private MetalBonusConfig _metalBonusConfig;
        // [SerializeField] private AbstractBonusConfig electricityBonusConfig;
        // [SerializeField] private AbstractBonusConfig fireBonusConfig;

        public List<RoundBonusTypeTuple> RoundBonusAssociationList
        {
            get => roundBonusAssociationList;
        }

        public List<BonusTypePrefabTuple> BonusTypePrefabAssociationList
        {
            get => bonusTypePrefabAssociationList;
        }

        public AbstractBonusConfig MetalBonusConfig
        {
            get => _metalBonusConfig;
        }
    }
}