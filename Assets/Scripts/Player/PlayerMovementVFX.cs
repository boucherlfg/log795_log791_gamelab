using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLab.Player
{
    public class PlayerMovementVFX: MonoBehaviour
    {
        [Header("Movement VFX")]
        [SerializeField] private float minSpeedThreshold = 0.1f;
        [SerializeField] private GameObject objectVFX;
        [SerializeField] private Rigidbody rb;

        [Header("Damping")] 
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private Transform windForce;

        private Vector3 _targetEulerAngles;
        private Vector3 _lastPosition;
        private float _speed;
        private void Start()
        {
            if (!objectVFX)
            {
                Debug.LogWarning("Object VFX is not set");
                enabled = false;
            }
            _lastPosition = transform.position;
            _targetEulerAngles = transform.eulerAngles;
        }

        private void FixedUpdate()
        {
            var position = transform.position;
            _speed = (position - _lastPosition).magnitude / Time.deltaTime;
            _lastPosition = position;
        }

        private void Update()
        {
            objectVFX.SetActive(_speed >= minSpeedThreshold);
            
            Quaternion currentRotation = windForce.rotation;
            Quaternion targetRotation = Quaternion.Euler(_targetEulerAngles);

            windForce.rotation = Quaternion.RotateTowards(
                currentRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(currentRotation, targetRotation) < 0.1f)
            {
                windForce.rotation = targetRotation;
                
            }
        }
    }
}
