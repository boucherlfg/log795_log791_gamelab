using System;
using System.Collections;
using System.Collections.Generic;
using GameLab.Core;
using UnityEngine;

namespace GameLab.Behaviours
{
    enum EMovableModifierState {
        Init,
        Moving,
        Waiting
    }
    public class MovableModifier : MonoBehaviour, ITickable
    {
        private const float DISTANCE_EPSILON = 0.001f;

        [SerializeField] private List<Transform> destinationPointList;
        [SerializeField] private bool isLoopMovement;
        [SerializeField] private float buildingMoveSpeed = 20f;
        [SerializeField] private float pauseTime = 1f;

        private Vector3 _startPosition;
        private float _waitTimer = 0;
        private int _index;
        private bool _isForward = true;
        private Transform _nextPoint;
        private EMovableModifierState _currentState = EMovableModifierState.Init;
        private Rigidbody _rb;

        public int Priority { get => 1; }

        private void Start()
        {
            _startPosition = transform.position;
            if (destinationPointList.Count < 2)
            {
                Debug.LogError($"Game object {gameObject.name} - A Movable modifier needs at least 2 points to work");
                this.enabled = false;
                return;
            }

            // GET RIGID BODY OF CURRENT OBJECT. We don't want to affect the parents or children
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
            {
                _rb = gameObject.AddComponent<Rigidbody>();
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
                _rb.useGravity = false;
            }
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.isKinematic = true;
        }

        private void SelectNextPointLoop()
        {
            _index = (_index + 1) % destinationPointList.Count;
            _nextPoint = destinationPointList[_index];
        }

        private void SelectNextPointReverse()
        {
            // UPDATE INDEX
            // HANDLE FORWARD
            if (_isForward)
            {
                _index++;
                if (_index >= destinationPointList.Count - 1)
                {
                    _index = destinationPointList.Count - 1;
                    _isForward = false;
                }
            }
            // HANDLE REVERSE
            else
            {
                _index--;
                if (_index <= 0)
                {
                    _index = 0;
                    _isForward = true;
                }
            }

            // SELECT NEXT POINTS
            if (_isForward)
            {
                _nextPoint = destinationPointList[_index + 1];
            }
            else
            {
                _nextPoint = destinationPointList[_index - 1];
            }
        }
        

        private void Init()
        {
            Debug.Log("init");
            transform.position = _startPosition;
            _nextPoint = destinationPointList[0];
            _index = 0;
            gameObject.isStatic = false;
            _currentState = EMovableModifierState.Moving;
        }
        
        private void Wait(float time)
        {
            if (_waitTimer + pauseTime > time) return;
            
            // LOOP
            if (isLoopMovement)
            {
                SelectNextPointLoop();
            }
            // NOT LOOP
            else
            {
                SelectNextPointReverse();
            }
            _currentState = EMovableModifierState.Moving;
        }
        
        private void Move(float time)
        {
            var distanceLeft = Vector3.Distance(transform.position, _nextPoint.position);

            // CHECK IF WE'RE AT THE POSITION
            if (distanceLeft < 0.1f)
            {
                this.transform.position = _nextPoint.position;
                _currentState = EMovableModifierState.Waiting;
                _waitTimer = time;
                return;
            }

            // CALCULATE THE MOVEMENT
            var direction = (_nextPoint.position - this.transform.position).normalized;
            var distance = MathF.Min(distanceLeft, buildingMoveSpeed * Time.fixedDeltaTime);
            _rb.Move( transform.position + direction * distance, transform.rotation);
        }


        public void Tick(float time)
        {
            if (time < DISTANCE_EPSILON)
            {
                _currentState = EMovableModifierState.Init;
            }

            switch (_currentState)
            {
                case EMovableModifierState.Init:
                    Init();
                    break;
                case EMovableModifierState.Moving:
                    Move(time);
                    break;
                case EMovableModifierState.Waiting:
                    Wait(time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Reset()
        {
            Init();
        }
    }
}