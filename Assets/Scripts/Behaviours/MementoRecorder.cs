using System.Collections.Generic;
using GameLab.Core;
using GameLab.Events;
using GameLab.ScriptableObjects;
using UnityEngine;

namespace GameLab.Behaviours
{
    [DefaultExecutionOrder(-5)]
    public class MementoRecorder : MonoBehaviour, ITickable
    {
        [SerializeField] private InputConfig inputConfig;
        private Vector2 _movement;
        private bool _isAccelerating;
        private int _id;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Vector3 _currentPosition;
        private readonly List<MementoEntry> _records = new();
        private bool _isRecording;

        public int Priority { get => 1; }

        public void SaveMemento()
        {
            _isRecording = false;
            GameEvents.Players[_id].Move.RemoveListener(HandleOnMove);
            GameEvents.Players[_id].Accelerate.RemoveListener(HandleOnAccelerate);
            GameEvents.MementoRequested.Invoke(new MementoPlayerInfo(_initialPosition, _initialRotation, new List<MementoEntry>(_records) ,_id));
        }
        
        public void SetPlayer(int playerId, Vector3 initialPosition, Quaternion initialRotation)
        {
            _id = playerId;
            GameEvents.Players[playerId].Move.AddListener(HandleOnMove);
            GameEvents.Players[playerId].Accelerate.AddListener(HandleOnAccelerate);
            _records.Clear();
            _initialPosition = initialPosition;
            _initialRotation = initialRotation;
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            VehiculeEvent.OnOutOfEnergy.AddListener(HandleOutOfEnergy);
        }
        

        private void HandleOutOfEnergy(bool isPlayer)
        {
            if (!isPlayer) return; 
            VehiculeEvent.OnOutOfEnergy.RemoveListener(HandleOutOfEnergy);
        }

        private void OnRunStarted()
        {
            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
        }

        private void HandleOnAccelerate(bool isAccelerating)
        {
            _isAccelerating = isAccelerating;
        }

        private void HandleOnMove(Vector2 direction)
        {
            _movement = direction;
        }

        public void Tick(float time)
        {
            _records.Add(new MementoEntry(time, _movement, _isAccelerating));
           
        }

        public void Reset() {}
    }
}