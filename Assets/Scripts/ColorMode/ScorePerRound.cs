using System;
using System.Collections.Generic;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.ColorMode
{
    public class ScorePerRound : MonoBehaviour
    {
        private int _scoreThisRound;

        private int _currentRound;
        private readonly List<PlayerNumber> _playerScores = new();
        private PlayerNumber _winningPlayer = 0;
        private void Awake()
        {
            RoundEvents.CurrentRoundUpdated.AddListener(OnPotentialScore);
            RoundEvents.TimesOut.AddListener(OnRoundIsEnding);
            GameEvents.AccumulatedScoreCalculated.AddListener(OnScoreInRoundCalculated);
        }

        private void OnDestroy()
        {
            RoundEvents.CurrentRoundUpdated.RemoveListener(OnPotentialScore);
            RoundEvents.TimesOut.RemoveListener(OnRoundIsEnding);
            GameEvents.AccumulatedScoreCalculated.RemoveListener(OnScoreInRoundCalculated);
        }

        private void OnScoreInRoundCalculated(int player1, int player2)
        {
            if (player1 > player2)
            {
                _winningPlayer = PlayerNumber.One;
            }
            else if (player1 < player2)
            {
                _winningPlayer = PlayerNumber.Two;
            }
            else
            {
                _winningPlayer = PlayerNumber.None;
            }
        }

        private void OnRoundIsEnding()
        {
            switch (_winningPlayer)
            {
                case PlayerNumber.One:
                    _playerScores.Add(PlayerNumber.One);
                    break;
                case PlayerNumber.Two:
                    _playerScores.Add(PlayerNumber.Two);
                    break;
                default:
                    _playerScores.Add(PlayerNumber.None);
                    break;
            }

            GameEvents.TotalPointsUpdated.Invoke(_playerScores);
        }

        private void OnPotentialScore(Round round)
        {
            _scoreThisRound = round.potetialPoints;
        }
    }
}