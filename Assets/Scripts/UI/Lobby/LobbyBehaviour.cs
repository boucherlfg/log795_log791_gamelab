using System.Linq;
using GameLab.Events;
using UnityEngine;

namespace GameLab.UI.Lobby
{
    public class LobbyBehaviour: MonoBehaviour
    {
        [SerializeField]
        private GameObject lobbyPanelPrefab;
        private GameObject _lobbyPanel;

        private void Awake()
        {
            _lobbyPanel = GameObject.FindGameObjectsWithTag("Input").FirstOrDefault();
            
        }

        private void OnPlayerJoined(int playerId)
        {
        }
    }
}