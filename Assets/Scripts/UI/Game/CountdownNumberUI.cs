using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class CountdownNumberUI: MonoBehaviour
    {
        [SerializeField] private Text countdownText;

        private void Start()
        {
            Destroy(gameObject, 2f);
        }

        public void SetText(string text)
        {
            countdownText.text = text;
        }
    }
}