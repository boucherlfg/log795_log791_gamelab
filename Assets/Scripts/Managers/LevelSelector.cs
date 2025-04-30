using System;
using System.Collections;
using FMODUnity;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.UI;
using GameLab.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameLab.Managers
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private Animator arrowLeft;
        [SerializeField] private Animator arrowRight;
        [SerializeField] private TextMeshProUGUI levelName;
        [SerializeField] private DifficultyIndicator levelDifficulty;
        [SerializeField] private Image levelImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private StudioEventEmitter changeLevelSFX;

        private GameObject _levelPrefab;
        public static GameObject Level
        {
            get;
            private set;
        }
        public SceneLoader.SceneTarget nextScene;
        public SelectableLevel[] levels;

        private int _index = 0;
        
        private IEnumerator Start()
        {
            LevelSelectorEvents.Left.AddListener(ChangeLevelLeft);
            LevelSelectorEvents.Right.AddListener(ChangeLevelRight);
            LevelSelectorEvents.Select.AddListener(Ready);
            ChangeLevel(0, false);

            yield return null;

            arrowLeft.enabled = true;
            arrowRight.enabled = true;
        }

        public void ChangeLevelLeft()
        {
            ChangeLevel(-1);    
        }

        public void ChangeLevelRight()
        {
            ChangeLevel(1);
        }
        
        public void ChangeLevel(int move, bool animate = true)
        {
            if(animate) changeLevelSFX.PlayWithTryCatch();
            _index += move;
            if (_index >= levels.Length) _index = 0;
            if (_index < 0) _index = levels.Length - 1;
            
            levelName.text = levels[_index].levelName;
            levelDifficulty.Value = levels[_index].levelDifficulty;
            levelImage.sprite = levels[_index].levelImage;
            backgroundImage.sprite = levels[_index].levelImage;
            _levelPrefab = levels[_index].levelPrefab;
            var anim = move == 1 ? arrowRight : arrowLeft;   
            if(animate) anim.Play("NextAnimation", -1, 0);
        }
        public void Ready()
        {
            Level = _levelPrefab;
            SceneLoader.loadingScreenSplashart = levels[_index].levelImage;
            SceneLoader.Load(nextScene);
        }
    }
}
