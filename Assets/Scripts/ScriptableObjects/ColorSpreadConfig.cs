using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    [Serializable]
    public class SpreadConfig
    {
        public bool fromCanonSplash = true;
        public bool fromBumperSplash = true;
        public bool fromGhostSplash = true;
        public bool fromFallSplash = true;
        public bool fromCollisionWithPlayer = true;
        public bool fromCollisionWithGhost = true;
        public bool canBeRecolored = true;
    }

    [CreateAssetMenu(fileName = "ColorSpreadConfig", menuName = "Gamelab/ColorSpreadConfig", order = 0)]
    public class ColorSpreadConfig : ScriptableObject
    {
        [Header("Tiles & Walls")] [SerializeField] private SpreadConfig tileAndWallsSpreadConfig;
        [Header("Canon")] [SerializeField] private SpreadConfig canonSpreadConfig;
        [Header("Bumper")] [SerializeField] private SpreadConfig bumperSpreadConfig;
        [Header("Shadow")] [SerializeField] private SpreadConfig shadowSpreadConfig;

        public SpreadConfig TileAndWallsSpreadConfig
        {
            get => tileAndWallsSpreadConfig;
        }

        public SpreadConfig CanonSpreadConfig
        {
            get => canonSpreadConfig;
        }

        public SpreadConfig BumperSpreadConfig
        {
            get => bumperSpreadConfig;
        }

        public SpreadConfig ShadowSpreadConfig
        {
            get => shadowSpreadConfig;
        }
    }
}