using System;
using GameLab.Core;
using UnityEngine;

namespace GameLab.Constructions.Canons
{
    public class CanonConstructRotate: StateBehavior, ITickable
    {
        [Header("Rotate")]
        [SerializeField] internal float rotationSpeed = 50f;
        [SerializeField] internal float rotateAngle = 45f;
        [SerializeField] internal bool isRotationClockWise;
        [SerializeField] private ConstructButton canonButton;
        
        private CanonRotateState _rotateState;
        private Quaternion _initialRotation;
        private bool _initIsRotationClockWise;

        protected override void Start()
        {
            _initialRotation = transform.rotation;
            _initIsRotationClockWise = isRotationClockWise;

            if(canonButton)
                canonButton.onPressTrigger += (_, _) => {
                    changeState(_rotateState);
                    isRotationClockWise = !isRotationClockWise;
                };
            
            base.Start();

            _rotateState = new CanonRotateState(this, rotationSpeed);
            _rotateState.onRotationComplete += (_, _) => changeState();
        }

        public int Priority { get => 1; }
        public void Tick(float time){ }

        public void Reset()
        {
            transform.rotation = _initialRotation;
            isRotationClockWise = _initIsRotationClockWise;

        }
    }
    
    internal class CanonRotateState : IState
    {
        internal event EventHandler onRotationComplete; 
        
        private readonly CanonConstructRotate _canon;
        private readonly Transform _transform;
        
        private Vector3 _targetEulerAngles;
        
        public CanonRotateState(CanonConstructRotate canon, float rotationSpeed)
        {
            _canon = canon;
            _transform = canon.transform;
        }

        public void EnterState()
        {
            float angle = _canon.isRotationClockWise ? _canon.rotateAngle : -_canon.rotateAngle;
            _targetEulerAngles = _transform.eulerAngles + new Vector3(0, angle, 0);
        }

        public void ExitState() { }

        public void UpdateState()
        {
            Quaternion currentRotation = _transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(_targetEulerAngles);

            _transform.rotation = Quaternion.RotateTowards(
                currentRotation,
                targetRotation,
                _canon.rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(currentRotation, targetRotation) < 0.1f)
            {
                _transform.rotation = targetRotation;
                onRotationComplete?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}