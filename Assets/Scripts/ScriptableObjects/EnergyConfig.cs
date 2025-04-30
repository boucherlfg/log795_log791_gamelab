using UnityEngine;

namespace GameLab.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnergyConfig", menuName = "Gamelab/EnergyConfig")]
    public class EnergyConfig : ScriptableObject
    {
        [SerializeField] private float maxEnergy;
        [SerializeField] private float baseEnergyConsumption;
        [SerializeField] private float accelerationExtraConsumption;
        [SerializeField] private float bounceEnergyConsumption;

        public float MaxEnergy
        {
            get => maxEnergy;
        }

        public float BaseEnergyConsumption
        {
            get => baseEnergyConsumption;
        }

        public float AccelerationExtraConsumption
        {
            get => accelerationExtraConsumption;
        }

        public float BounceEnergyConsumption
        {
            get => bounceEnergyConsumption;
        }
    }
}
