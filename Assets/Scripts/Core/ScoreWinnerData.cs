using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Enums;
using GameLab.Events;

namespace GameLab.Core
{
    public class ScoreWinnerData : Singleton<ScoreWinnerData>
    {
        public List<PlayerNumber> ScoresPerRound => _scoresPerRound;
        public int Player1 => _scoresPerRound.Count(x => x == PlayerNumber.One);
        public int Player2 => _scoresPerRound.Count(x => x == PlayerNumber.Two);
        
        private List<PlayerNumber> _scoresPerRound = new ();
        public PlayerNumber Winner;

        public ScoreWinnerData()
        {
            GameEvents.TotalPointsUpdated.AddListener(TotalPointsUpdated);
            GameEvents.GameStarted.AddListener(Reset);
        }

        private void Reset()
        {
            _scoresPerRound.Clear();
        }

        private void TotalPointsUpdated(List<PlayerNumber> scores)
        {
            _scoresPerRound = new List<PlayerNumber>(scores);
            
            if(Player1 == Player2) Winner = PlayerNumber.None;
            else if (Player1 > Player2) Winner = PlayerNumber.One;
            else Winner = PlayerNumber.Two;
        }
    }
}
