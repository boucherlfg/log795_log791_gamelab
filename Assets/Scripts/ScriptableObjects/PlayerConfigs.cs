using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerConfigs", menuName = "Gamelab/PlayerConfigs")]
    public class PlayerConfigs : ScriptableObject
    {
        [Header("Color palette")] 
        public Color brightUI;
        public Color regularUI;
        public Color darkUI;
        public Color player1UI;
        public Color player2UI;
        public Color yellowUI;
        public Color grayUI;
        
        [Header("Model Values")] [SerializeField]
        private Material[] materialsToLookFor;

        [SerializeField] private float[] playerHues;
        [SerializeField] private Color[] playerColors;
        [SerializeField] private Color[] shadowColors;
        [Range(0, 2)] [SerializeField] private int[] playerHeadIndex;

        [Header("Painting Values")]
        [SerializeField] private Color tileEmissiveTintDefault;
        [SerializeField] private Color[] tileEmissiveTints;
        [SerializeField] private Color tileMaskColorTintsDefault;
        [SerializeField] private Color[] tileMaskColorTints;
        [SerializeField] private float tileEmissive;

        [SerializeField] private Material bumperDefaultMaterial;
        [SerializeField] private Material[] bumperMaterials;
        [SerializeField] private float canonDefaultHue;
        [SerializeField] private float[] canonHues;

        [FormerlySerializedAs("baseColorSelects")]
        [Header("Player Select")] 
        [SerializeField] private Material[] decalsSelectMaterials;
        [SerializeField] private float[] emissionIntensities;
        [SerializeField] private Color[] emissionTients;
        
        public Material[] MaterialsToLookFor
        {
            get => materialsToLookFor;
        }

        public float[] PlayerHues
        {
            get => playerHues;
        }

        public Color[] PlayerColors
        {
            get => playerColors;
        }

        public Color[] ShadowColors
        {
            get => shadowColors;
        }

        public int[] PlayerHeadIndex
        {
            get => playerHeadIndex;
        }

        public Color TileEmissiveTintDefault {
            get => tileEmissiveTintDefault;
        }
        public Color[] TileEmissiveTints {
            get => tileEmissiveTints;
        }
        public Color TileMaskColorTintsDefault {
            get => tileMaskColorTintsDefault;
        }
        public Color[] TileMaskColorTints {
            get => tileMaskColorTints;
        }
        public float TileEmissive {
            get => tileEmissive;
        }

        public Material BumperDefaultMaterial {
            get => bumperDefaultMaterial;
        }
        public Material[] BumperMaterials {
            get => bumperMaterials;
        }
        public float CanonDefaultHue {
            get => canonDefaultHue;
        }
        public float[] CanonHues {
            get => canonHues;
        }
        
        public Material[] DecalsSelectMaterials => decalsSelectMaterials;
        public float[] EmissionIntensities => emissionIntensities;
        public Color[] EmissionTients => emissionTients;
    }
}