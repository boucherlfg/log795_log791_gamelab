using System;
using GameLab.Events;
using GameLab.Managers;
using GameLab.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLab.Behaviours
{
    public class EnergyComponent : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private EnergyConfig energyConfig;


        [Header("Debug")]
        [SerializeField] private float energyLeft = 0f;
        [SerializeField] private bool isAccelerating = false;
        [SerializeField] private bool canConsume = false;
        private bool _isPlayer;
        private PlayerScript player;
        public float EnergyLeft => energyLeft;
        public void SetIsAccelerating(bool newIsAccelerating)
        {
            this.isAccelerating = newIsAccelerating;
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _isPlayer = TryGetComponent<PlayerScript>(out player);

            // true if we're attach to the main player
            
            energyLeft = energyConfig.MaxEnergy;
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            
            

            if (_isPlayer)
            {
                RoundEvents.EnergyRemaining.Invoke(Mathf.RoundToInt(energyLeft));
                RoundEvents.EnergyRatio.Invoke(energyLeft/energyConfig.MaxEnergy);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!canConsume)
            {
                return;
            }

            // UPDATE ENERGY
            energyLeft -= energyConfig.BaseEnergyConsumption * Time.deltaTime;
            if (isAccelerating)
            {
                energyLeft -= energyConfig.AccelerationExtraConsumption * Time.deltaTime;
            }


            // CHECK IF OUT OF ENERGY
            CheckIfOutOfEnergy();
            UpdateUI();
        }

        private void OnRunStarted()
        {
            canConsume = true;
        }

        private void OnDestroy()
        {
            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
        }

        private void UpdateUI()
        {
            // UPDATE UI
            if (_isPlayer)
            {
                // CLAMP ENERGY
                energyLeft = Mathf.Clamp(energyLeft, 0, energyConfig.MaxEnergy);
                RoundEvents.EnergyRemaining.Invoke(Mathf.RoundToInt(energyLeft));
                RoundEvents.EnergyRatio.Invoke(energyLeft/energyConfig.MaxEnergy);
            }
        }

        private void CheckIfOutOfEnergy()
        {
            if (energyLeft <= 0 && canConsume)
            {
                canConsume = false;
                // DISABLE PLAYER INPUTS
                if (_isPlayer)
                {
                   player.IsOutOfEnergy = true;
                }
                VehiculeEvent.OnOutOfEnergy.Invoke(_isPlayer);
            }
        }

        public void OnBounce()
        {
            energyLeft -= energyConfig.BounceEnergyConsumption;
            CheckIfOutOfEnergy();
            UpdateUI();
        }
    }
}
