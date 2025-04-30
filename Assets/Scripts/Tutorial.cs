using System;
using GameLab.Events;
using GameLab.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameLab
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI indexText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Slider progressionBar;
        [SerializeField] private TutorialConfig tutorialConfig;
        private int _index = 0;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UpdateForCurrentIndex();
            LobbyEvent.TutorialNext.AddListener(OnTutorialNext);
        }

        private void OnTutorialNext()
        {
            _index = (_index + 1) % tutorialConfig.TutorialSteps.Count;
            UpdateForCurrentIndex();
        }

        private void UpdateForCurrentIndex()
        {
            image.enabled = tutorialConfig.TutorialSteps[_index].image != null;
            image.sprite = tutorialConfig.TutorialSteps[_index].image;
            indexText.text = $"{_index + 1}/{tutorialConfig.TutorialSteps.Count}";
            instructionText.text = tutorialConfig.TutorialSteps[_index].instruction;
            progressionBar.value = (float)(_index + 1)/(float)tutorialConfig.TutorialSteps.Count;
        }

        private void OnDestroy()
        {
            LobbyEvent.TutorialNext.RemoveListener(OnTutorialNext);
        }
    }
}
