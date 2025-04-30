using UnityEngine;

namespace GameLab.ScriptableObjects
{
    
    [CreateAssetMenu(fileName = "LobbyConfig", menuName = "Gamelab/Config/Lobby", order = 1)]
    public class LobbyConfig : ScriptableObject
    {
        public GameObject playerPrefab;
    }
}