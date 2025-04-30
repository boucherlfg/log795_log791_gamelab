using System;
using FMODUnity;
using GameLab.Core;
using GameLab.Utility;
using UnityEngine;
using EventHandler = System.EventHandler;

namespace GameLab.Constructions
{
    public class ConstructButton: StateBehavior
    {
        public event EventHandler onPressTrigger; 
        internal event EventHandler<GameObject> onPressAttempt;

        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private float cooldown = 2f;

        private CanonButtonAwaitState _awaitState;
        private CanonButtonActiveState _activeState;

        [Header("SFX")]
        [SerializeField] private StudioEventEmitter buttonPressSfx;

        protected override void Start()
        {
            base.Start();

            _awaitState = new CanonButtonAwaitState(this);
            _activeState = new CanonButtonActiveState(this, boxCollider, cooldown);

            _activeState.onReadyToActive += (_, _) => changeState(_awaitState);
            _awaitState.onPressAcknoledge += (_, _) =>
            {
                changeState(_activeState);
                buttonPressSfx.PlayWithTryCatch();
                onPressTrigger?.Invoke(this, EventArgs.Empty);
            };

            changeState(_awaitState);
        }

        private void TryPressButton(GameObject buttonPresser)
        {
            if (IsCurrentState(_awaitState))
            {
                onPressAttempt?.Invoke(this, buttonPresser);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            TryPressButton(other.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryPressButton(other.gameObject);
        }
    }

    internal class CanonButtonAwaitState : IState
    {
        private const string PlayerTag = "Player";
        private const string ShadowTag = "Shadow";

        internal event EventHandler onPressAcknoledge;

        private ConstructButton _buttonConstruct;

        public CanonButtonAwaitState(ConstructButton buttonConstruct)
        {
            _buttonConstruct = buttonConstruct;
        }

        public void EnterState()
        {
            _buttonConstruct.onPressAttempt += CanonButtonConstruct_OnPressAttempt;
        }

        public void ExitState()
        {
            _buttonConstruct.onPressAttempt -= CanonButtonConstruct_OnPressAttempt;
        }
        public void UpdateState() { }

        private void CanonButtonConstruct_OnPressAttempt(object sender, GameObject gameObject)
        {
            if (gameObject.CompareTag(PlayerTag) || gameObject.CompareTag(ShadowTag))
                onPressAcknoledge?.Invoke(this, EventArgs.Empty);
        }
    }

    internal class CanonButtonActiveState : IState
    {
        internal event EventHandler onReadyToActive;
        private const float SpeedSplit = 8f;
        
        private Transform _movingObject;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        
        private float _downSpeed;
        private float _riseSpeed;
        private bool _isRising;

        public CanonButtonActiveState(ConstructButton buttonConstruct, BoxCollider boxCollider, float cooldown)
        {
            _movingObject = buttonConstruct.transform;
            
            _startPosition = buttonConstruct.transform.position;
            _endPosition = buttonConstruct.transform.position - new Vector3(0, boxCollider.size.y, 0);
            
            _downSpeed = boxCollider.size.y / (cooldown / SpeedSplit);
            _riseSpeed = boxCollider.size.y / (cooldown - cooldown / SpeedSplit);
        }

        public void EnterState()
        {
            _isRising = false;
        }
        public void ExitState() { }

        public void UpdateState()
        {
            float step =  (_isRising ? _riseSpeed : _downSpeed) * Time.deltaTime;
            Vector3 target = _isRising ? _startPosition : _endPosition;
            
            _movingObject.position = Vector3.MoveTowards(_movingObject.position, target, step);

            if (Vector3.Distance(_movingObject.position, target) < 0.001f)
            {
                _movingObject.position = target;
                
                if(_isRising)
                    onReadyToActive?.Invoke(this, EventArgs.Empty);
                else
                    _isRising = true;
            }
        }
    }
}