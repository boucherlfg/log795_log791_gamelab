using System;
using UnityEngine;

namespace GameLab.Core
{
    public abstract class StateBehavior: MonoBehaviour
    {
        private IState _currentState;
        private OnHoldState _onHold;

        protected virtual void Start()
        {
            _onHold = new OnHoldState();
            _currentState = _onHold;
        }

        protected virtual void Update()
        {
            _currentState.UpdateState();
        }
        
        protected bool IsCurrentState(IState state) => state == _currentState;

        protected void changeState() => changeState(_onHold);
        protected void changeState(IState newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }
    }

    class OnHoldState : IState
    {
        public void EnterState() { }
        public void ExitState() { }
        public void UpdateState() { }
    }
}