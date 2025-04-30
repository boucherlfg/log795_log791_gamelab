using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.ScriptableObjects
{
    public enum InputType
    {
        Directional = 0,
        Tank = 1
    }

    [CreateAssetMenu(fileName = "InputConfig", menuName = "Gamelab/InputConfig", order = 0)]
    public class InputConfig : ScriptableObject
    {
        private const float epsilon = 0.1f;
        [Header("Both")]
        [SerializeField] private InputType inputType;
        [SerializeField] private float rampVelBonus = 1.2f;    
        [SerializeField] private bool useSharpRotation = true;
        [SerializeField] private float sharpRotationAngle = 15f;

        [Header("Ground Value")]
        [SerializeField] private float acceleration;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float decceleration;
        [SerializeField] private float deccelerationOverMaxSpeed;
        [SerializeField] private float deccelerationOverMaxSpeedMultiplier = 2f;

        [Header("Air Value")]
        [SerializeField] private float accelerationInAir;
        [SerializeField] private float deccelerationAir;
        [SerializeField] private float gravityAcceleration;
        [SerializeField] private float gravityMaxSpeed;
        


        [Header("Dash")]
        [Tooltip("Acceleration that's added to normal acceleration")][SerializeField] private float dashAddedAcceleration = 10f;
        [Tooltip("Acceleration that replaces normal input acceleration")][SerializeField] private float dashInputAcceleration = 5f;
        [Tooltip("Max speed added to normal max speed")][SerializeField] private float dashAddedMaxSpeed = 10f;
        [Tooltip("Duration of the dash")][SerializeField] private float dashDuration = 0.5f;
        [Tooltip("Cooldown between dashes")][SerializeField] private float dashCooldown = 0.5f;
        [Tooltip("Impulse that's given to the player when the dash starts")][SerializeField] private float dashInitialImpulse = 20;
        [Tooltip("0 is no gravity, 1 is all gravity. 2 is double gravity etc")][SerializeField] private float dashGravityFactor = 0.1f;
        [Range(0, 90)] [Tooltip("0 is foward, 90 is up")]
        [SerializeField] private float dashDirection;
        [Tooltip("0 is no dash, infinite dash would be a big number, say, 1000000")][SerializeField] private int dashCharges = 3;
        [Tooltip("when false, player will not be able to dash mid air")][SerializeField] private bool canDashInAir = false;

        [Header("Rotation")]
        [SerializeField] private float ballRotationSpeed;
        [SerializeField] private float torqueRobot = 10f;
        [SerializeField] private AnimationCurveObject torqueBallGround;
        [SerializeField] private AnimationCurveObject torqueBallAir;



        public InputType InputType
        {
            get => inputType;
        }

        public float RampVelBonus
        {
            get => rampVelBonus;
        }

        public bool UseSharpRotation
        {
            get => useSharpRotation;
        }

        public float SharpRotationAngle
        {
            get => sharpRotationAngle;
        }

        public float Acceleration
        {
            get => acceleration;
        }

        public float Decceleration
        {
            get => decceleration;
        }

        public float DeccelerationOverMaxSpeed
        {
            get => deccelerationOverMaxSpeed;
        }

        public float DeccelerationOverMaxSpeedMultiplier
        {
            get => deccelerationOverMaxSpeedMultiplier;
        }

        public float MaxSpeed
        {
            get => maxSpeed;
        }

        public float TorqueRobot
        {
            get => torqueRobot;
        }

        public float BallRotationSpeed
        {
            get => ballRotationSpeed;
        }

        public AnimationCurveObject TorqueBallGround {
            get => torqueBallGround;
        }
        public AnimationCurveObject TorqueBallAir {
            get => torqueBallAir;
        }

        public float AccelerationInAir {
            get => accelerationInAir;
        }
        public float DeccelerationAir {
            get => deccelerationAir;
        }
        public float GravityAcceleration {
            get => gravityAcceleration;
        }
        public float GravityMaxSpeed {
            get => gravityMaxSpeed;
        }

        public float DashAddedAcceleration => dashAddedAcceleration;

        public float DashAddedMaxSpeed => dashAddedMaxSpeed;
        
        public float DashInputAcceleration => dashInputAcceleration;

        public float DashDuration => dashDuration;
        
        public float DashCooldown => dashCooldown;
        
        public float DashInitialImpulse => dashInitialImpulse;
        
        public float DashGravityFactor => dashGravityFactor;
        
        public float DashDirection => dashDirection;
        
        public int DashCharges => dashCharges;
        
        public bool CanDashInAir => canDashInAir;
    }
}