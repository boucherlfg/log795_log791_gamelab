using System;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Core;
using UnityEngine;
using EventHandler = System.EventHandler;

namespace GameLab.Player
{
    public class MovingObjectRampChecker: StateBehavior
    {
        private const string Ramp = "Rampe";

        internal event EventHandler<Collision> OnEnterContact;

        [SerializeField] internal float gapAllow = 0.2f;
        [SerializeField] internal LayerMask rampLayer;
        [SerializeField] internal Transform body;

        internal float Height;
        
        private MovingObject _movingObject;
        private MovingObject_OffRampState _offRampState;
        private MovingObject_OnRampState _onRampState;

        private void Awake()
        {
            _movingObject = GetComponent<MovingObject>();
        }

        protected override void Start()
        {
            Height = body.GetComponent<SphereCollider>().radius;
            Height *= body.transform.localScale.y;
            
            base.Start();

            _offRampState = new MovingObject_OffRampState(this);
            _offRampState.onEnterRampAtBase += OffRampState_OnEnterRampAtBase;
            _onRampState = new MovingObject_OnRampState(this);
            _onRampState.OnExitRamp += OnRampState_OnExitRamp;
            changeState(_offRampState);
            
            void OffRampState_OnEnterRampAtBase(object sender, EventArgs e)
            {
                _movingObject.isGettingRampBoost = true;
                changeState(_onRampState);
            }
            
            void OnRampState_OnExitRamp(object sender, EventArgs e)
            {
                _movingObject.isGettingRampBoost = false;
                changeState(_offRampState);                
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Ramp))
                OnEnterContact?.Invoke(this, other);
        }

        internal void GetLowestPointMesh(MeshCollider mesh, out float lowestPoint)
        {
            Bounds bounds = mesh.bounds;
            lowestPoint = bounds.min.y;
        }
    }

    class MovingObject_OffRampState: IState
    {
        private readonly MovingObjectRampChecker _rampChecker;

        internal event EventHandler onEnterRampAtBase;
        
        public MovingObject_OffRampState(MovingObjectRampChecker rampChecker)
        {
            _rampChecker = rampChecker;
        }

        public void EnterState()
        {
            _rampChecker.OnEnterContact += RampChecker_OnEnterContact;
        }
        public void ExitState()
        {
            _rampChecker.OnEnterContact -= RampChecker_OnEnterContact;
        }
        public void UpdateState() { }
        
        private void RampChecker_OnEnterContact(object sender, Collision ramp)
        {
            if (ramp.gameObject.TryGetComponent(out MeshCollider mesh))
            {
                _rampChecker.GetLowestPointMesh(mesh, out float lowestPoint);
                
                float gap = _rampChecker.transform.position.y - _rampChecker.Height - lowestPoint;
                if(gap <= _rampChecker.gapAllow)
                    onEnterRampAtBase?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    class MovingObject_OnRampState : IState
    {
        internal event EventHandler OnExitRamp;
        
        private readonly MovingObjectRampChecker _rampChecker;

        public MovingObject_OnRampState(MovingObjectRampChecker rampChecker)
        {
            _rampChecker = rampChecker;
        }

        public void EnterState() { }
        public void ExitState() { }
        public void UpdateState()
        {
            bool isOff= !Physics.CheckSphere(_rampChecker.transform.position - new Vector3(0f, _rampChecker.Height, 0f), _rampChecker.gapAllow, _rampChecker.rampLayer);
            if(isOff)
                OnExitRamp?.Invoke(this, EventArgs.Empty);
        }
    }
}