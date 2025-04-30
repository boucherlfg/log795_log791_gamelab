using UnityEngine;

namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level", menuName = "GameLab/Level", order = 1)]
    public class SelectableLevel : ScriptableObject
    {
        public string levelName;
        [Range(1, 3)] public int levelDifficulty = 1;
        [Tooltip("must be 680 x 300")] public Sprite levelImage;
        public GameObject levelPrefab;
    }
}