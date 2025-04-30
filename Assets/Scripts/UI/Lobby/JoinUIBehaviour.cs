using System;
using TMPro;
using UnityEngine;

namespace GameLab.UI.Lobby
{
    public class JoinUIBehaviour : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI readyInstructionText;
        [SerializeField] private GameObject joinPanel;
        [SerializeField] private GameObject readyPanel;

        public int PlayerId => playerId;
        [SerializeField] private int playerId;

        private void Start()
        {
            readyInstructionText.SetText("Press any key to join");
        }

        public void MakeTextRequestReady()
        {
            joinPanel.SetActive(false);
            readyPanel.SetActive(true);
        }

        public void Close()
        {
            readyPanel.SetActive(false);
        }
    }
}