using System;
using System.Collections.Generic;
using GameLab.Events;
using UnityEngine;

namespace GameLab
{
    public class TempPlayerMovement : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private bool _isInputBlocked = true;
        private void Start()
        {
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = 3;
        }

        private void OnRunStarted()
        {
            _isInputBlocked = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isInputBlocked) return;
            
            var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            _rigidbody.AddForce(move);
            
        }
    }
}
