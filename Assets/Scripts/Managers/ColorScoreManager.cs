using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Behaviours;
using GameLab.ColorMode;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.UI.Game;
using UnityEngine;
using GameLab.Enums;
using GameLab.Utility;

namespace GameLab.Managers
{
    [DefaultExecutionOrder(-10)]
    public class ColorScoreManager : MonoBehaviour
    {
        private bool _isCalculating = false;
        private int _player1Score;
        private int _player2Score;

        /// <summary>
        /// key is vehicle id and value is counter
        /// </summary>
        private Dictionary<int, float> _vehicleCounters;

        private Dictionary<PlayerNumber, List<IPaintable>> _paintableElements = new();
        private void Start()
        {
            _paintableElements.Add(PlayerNumber.None, new List<IPaintable>());
            _paintableElements.Add(PlayerNumber.One, new List<IPaintable>());
            _paintableElements.Add(PlayerNumber.Two, new List<IPaintable>());

            RoundEvents.RoundNumber.AddListener(OnRunStarted);
            RoundEvents.TimesOut.AddListener(OnTimesOut);
            GameEvents.OnPaintableUpdated.AddListener(OnPaintableUpdated);
        }

        private void OnDestroy()
        {
            RoundEvents.RoundNumber.RemoveListener(OnRunStarted);
            RoundEvents.TimesOut.RemoveListener(OnTimesOut);
            GameEvents.OnPaintableUpdated.RemoveListener(OnPaintableUpdated);
        }

        private void Update()
        {
            if (!_isCalculating) return;
            CalculatePoints();
        }

        private void CalculatePoints()
        {
            _player1Score = _paintableElements[PlayerNumber.One].Sum(paintable => paintable.Score);
            _player2Score = _paintableElements[PlayerNumber.Two].Sum(paintable => paintable.Score);
            GameEvents.AccumulatedScoreCalculated.Invoke(_player1Score, _player2Score);
        }

        private void OnRunStarted(int round, int totalRounds)
        {
            _player1Score = 0;
            _player2Score = 0;

            foreach (var list in _paintableElements)
            {
                list.Value.Clear();
            }

            var paintables = Extensions.FindComponents<IPaintable>().ToList();
            ColorSplash.paintables = paintables;
            foreach (var paintable in paintables)
            {
                paintable.Reset();
                _paintableElements[PlayerNumber.None].Add(paintable);
            }
            GameEvents.AccumulatedScoreCalculated.Invoke(0, 0);
            _isCalculating = true;
        }

        private void OnTimesOut()
        {
            _isCalculating = false;
            CalculatePoints();
        }

        private void OnPaintableUpdated(PlayerNumber previousOwner, IPaintable paintable)
        {
            _paintableElements[previousOwner].Remove(paintable);
            _paintableElements[paintable.Owner].Add(paintable);
        }
    }
}