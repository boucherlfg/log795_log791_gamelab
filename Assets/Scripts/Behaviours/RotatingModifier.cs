using System;
using System.Collections.Generic;
using GameLab.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.Behaviours
{
    enum ERotatingDirection
    {
        Forward,
        Reverse
    }

    [RequireComponent(typeof(Rigidbody))]
    public class RotatingModifier : MonoBehaviour, ITickable
    {
        [SerializeField] private float buildingTorqueSpeed = 25;

        [SerializeField] private bool doFullRotation = true;

        [Range(0.0f, 360f)] [SerializeField] private float maxAngle = 360f;
        private ERotatingDirection _rotatingDirection = ERotatingDirection.Forward;
        private Quaternion _initialRot;
        private Rigidbody _rigidBody;
        private float _localRotationTracking;

        public int Priority
        {
            get => 1;
        }

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.isKinematic = true;
            _rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out Rigidbody childRb))
                {
                    childRb = child.gameObject.AddComponent<Rigidbody>();
                }

                childRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                childRb.isKinematic = true;
                _initialRot = transform.rotation;
            }

            _localRotationTracking = transform.rotation.eulerAngles.y;
        }

        public void Reset()
        {
            transform.rotation = _initialRot;
        }

        public void Tick(float time)
        {
            Vector3 savedRotation = transform.rotation.eulerAngles;
            if (doFullRotation)
            {
                var y = time * buildingTorqueSpeed;
                transform.rotation = Quaternion.Euler(savedRotation.x, y, savedRotation.z);
            }
            else
            {
                // CHECK IF WE NEED TO REVERSE
                switch (_rotatingDirection)
                {

                    case ERotatingDirection.Forward:
                        if (_localRotationTracking >= maxAngle)
                        {
                            _rotatingDirection = ERotatingDirection.Reverse;
                        }
                        break;
                    case ERotatingDirection.Reverse:
                        if (_localRotationTracking <= 0.0)
                        {
                            _rotatingDirection = ERotatingDirection.Forward;
                        }
                        break;
                }

                // APPLY ROTATION
                switch (_rotatingDirection)
                {
                    case ERotatingDirection.Forward:
                        _localRotationTracking += buildingTorqueSpeed * 0.1f;
                        transform.rotation = Quaternion.Euler(savedRotation.x, _localRotationTracking, savedRotation.z);
                        break;
                    case ERotatingDirection.Reverse:
                        _localRotationTracking -= buildingTorqueSpeed * 0.1f;
                        transform.rotation = Quaternion.Euler(savedRotation.x, _localRotationTracking, savedRotation.z);
                        break;
                }
            }
        }

        private float ClampedAngle(float angle)
        {
            float clampedAngle = angle;
            if (clampedAngle < 0f)
            {
                clampedAngle += 360f;
            }
            else if (clampedAngle > 360)
            {
                clampedAngle -= 360f;
            }

            return clampedAngle;
        }

        private void OnDrawGizmos()
        {
            const int NUM_POINT = 15;
            const int RADIUS = 5;
            Vector3 VERTICAL_OFFSET = new Vector3(0, 2.5f, 0);

            List<Vector3> points = new();

            if (doFullRotation)
            {
                for (int i = 0; i < NUM_POINT + 1; i++)
                {
                    Quaternion rotation = Quaternion.AngleAxis(360f / NUM_POINT * i, Vector3.up);
                    points.Add(rotation * (transform.forward * RADIUS) + transform.position + VERTICAL_OFFSET);
                }
            }
            else
            {
                for (int i = 0; i < NUM_POINT + 1; i++)
                {
                    Quaternion rotation = Quaternion.AngleAxis(maxAngle / NUM_POINT * i, Vector3.up) * Quaternion.Inverse(transform.rotation);
                    points.Add(rotation * (transform.forward * RADIUS) + transform.position + VERTICAL_OFFSET);
                }
            }


            Gizmos.color = Color.grey;
            Gizmos.DrawLineStrip(points.ToArray(), false);
        }
    }
}