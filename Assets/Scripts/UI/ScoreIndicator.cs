using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Enums;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI
{
    public class ScoreIndicator : MonoBehaviour
    {
        [SerializeField] private bool updateWhenNotPlaying = false;
        public List<PlayerNumber> ScoresPerRound
        {
            get => new(scoresPerRound);
            set
            {
                scoresPerRound = new List<PlayerNumber>(value);
            } 
        }
        [SerializeField] private List<PlayerNumber> scoresPerRound = new ();
        [SerializeField] private PlayerConfigs playerConfigs;
        [SerializeField] private List<Image> scoreElements;
        private const string ScoreAnimationTag = "AddedScoreAnimation";

        private void OnEnable()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int i = 0; i < scoreElements.Count; i++)
            {
                var element = scoreElements[i];
                
                // we only want to animate the last point activated
                var shouldAnimate = i == scoreElements.Count - 1;
                if (i >= scoresPerRound.Count)
                {
                    scoreElements[i].color = playerConfigs.brightUI;
                    scoreElements[i].transform.GetChild(0).gameObject.SetActive(false);
                    continue;
                }
                
                var score = scoresPerRound[i];
                switch (score)
                {
                    case PlayerNumber.One:
                        scoreElements[i].color = playerConfigs.player1UI;
                        scoreElements[i].transform.GetChild(0).gameObject.SetActive(false);
                        break;
                    case PlayerNumber.Two:
                        scoreElements[i].color = playerConfigs.player2UI;
                        scoreElements[i].transform.GetChild(0).gameObject.SetActive(false);
                        break;
                    case PlayerNumber.None:
                        scoreElements[i].color = playerConfigs.brightUI;
                        scoreElements[i].transform.GetChild(0).gameObject.SetActive(true);
                        scoreElements[i].transform.GetChild(0).GetComponent<Image>().color = playerConfigs.grayUI;
                        break;
                }
                if(shouldAnimate) element.transform.parent.GetComponent<Animator>().Play(ScoreAnimationTag);
            }
        }
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            UpdateUI();
        }
    }
}
