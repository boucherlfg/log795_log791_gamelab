using GameLab.Core;
using GameLab.Enums;
using GameLab.ScriptableObjects;
using GameLab.ScriptableObjects.Bonus;
using UnityEngine;

namespace GameLab.Bonus
{
    public class BonusSpawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnLocation;

        public void SpawnBonus(GameObject bonusPrefab, AbstractBonusConfig bonusConfig)
        {
            // LATER WE CAN ANIMATE THIS
            GameObject createdGameObject = Instantiate(bonusPrefab, spawnLocation);
            createdGameObject.GetComponentInChildren<Bonus>().BonusConfig = bonusConfig;
        }
    }
}