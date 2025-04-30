using System.Collections.Generic;
using System.Linq;
using GameLab.Core;
using GameLab.Events;
using UnityEngine;

namespace GameLab.Behaviours
{
    public class MementoRoundRecorder : MonoBehaviour, ITickable
    {
        private readonly Dictionary<int, MementoVehicleInfo> _records = new();
        private readonly Dictionary<int, Vector3> _positions = new();
        private readonly Dictionary<int, bool> _dashes = new();
        private readonly Dictionary<int, Quaternion> _rotations = new();
        private readonly Dictionary<int, Color> _colors = new();
        private bool _isRecording;
        private int _frame;


        public void Awake()
        {
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            RoundEvents.RunEnded.AddListener(OnRunFinished);
            TrailEvent.Track.AddListener(SetPlayer);
            _isRecording = false;
            _frame = 0;
        }

        private void SetPlayer(int playerId, Color color)
        {
            _records.TryAdd(playerId, new MementoVehicleInfo());
            _colors.TryAdd(playerId, color);
            GameEvents.MovingObjects.TryAdd(playerId, new MovingObjectEvent());
            GameEvents.MovingObjects[playerId].PositionChanged.AddListener((position, rotation, dash) =>
            {
                HandleOnPositionChanged(playerId, position, rotation, dash);
            });
        }

        private void HandleOnPositionChanged(int playerId, Vector3 position, Quaternion rotation, bool dash)
        {
            if (_positions.ContainsKey(playerId) && _isRecording)
                _positions[playerId] = position;
            else if (_isRecording)
            {
                _positions.TryAdd(playerId, position);
            }
            
            if (_rotations.ContainsKey(playerId) && _isRecording)
                _rotations[playerId] = rotation;
            else if (_isRecording)
            {
                _rotations.TryAdd(playerId, rotation);
            }

            if (_dashes.ContainsKey(playerId) && _isRecording)
            {
                _dashes[playerId] = dash;
            }
            else if (_isRecording)
            {
                _dashes.TryAdd(playerId, dash);
            }
        }
        
        private void OnRunStarted()
        {
            _frame = 0;

            _isRecording = true;
        }

        private void OnRunFinished()
        {
            
            
            _records[_records.Count] = _records[0];
            _records[_records.Count] = _records[1];
            _records[0] = new MementoVehicleInfo();
            _records[1] = new MementoVehicleInfo();
            
            TrailEvent.Clear.Invoke();
            _isRecording = false;
        }

        public int Priority { get; }

        public void Tick(float time)
        {
            _frame++;
            if (!_isRecording) return;

            if (_positions.Count >= 2)
            {
                _records[0].AddPosition(_frame, _positions[0]);
                _records[1].AddPosition(_frame, _positions[1]);
                _records[0].AddRotation(_frame, _rotations[0]);
                _records[1].AddRotation(_frame, _rotations[1]);
                _records[0].AddDash(_frame, _dashes[0]);
                _records[1].AddDash(_frame, _dashes[1]);
            }

            if (_frame <= 1) return;
            foreach (var record in _records.Where(record => GameEvents.Shadows.ContainsKey(record.Key)))
            {
                if(record.Value.PositionExists(_frame))
                    GameEvents.Shadows[record.Key].Move.Invoke(record.Value.GetPosition(_frame));
                if(record.Value.RotationExists(_frame))
                    GameEvents.Shadows[record.Key].Rotate.Invoke(record.Value.GetRotation(_frame));
                if(record.Value.DashExists(_frame))
                    GameEvents.Shadows[record.Key].Dash.Invoke(record.Value.GetDash(_frame));
            }
        }

        public void Reset()
        {
        }
    }
}