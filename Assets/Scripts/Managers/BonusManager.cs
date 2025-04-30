using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Bonus;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.ScriptableObjects.Bonus;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameLab.Managers
{
    // IF NOT EXECUTED BEFORE GAMEMODE WE'RE NOT RECEIVING THE RUNNUMBER EVENT
    [DefaultExecutionOrder(-10)]
    public class BonusManager : MonoBehaviour, ITickable
    {
        [SerializeField] private BonusManagerConfig bonusManagerConfig;

        private List<BonusSpawner> _bonusSpawners;

        public int Priority { get => 0; }

        private Dictionary<EBonusType, Dictionary<GameObject, List<Aura>>> _auraRegistries = new ()
        {
            { EBonusType.Metal, new ()},
            { EBonusType.Electricity, new ()},
            { EBonusType.Fire, new ()},
        };

        private List<IAuraReceiver> _affectedReceivers = new();
        private List<Aura> _auraEmiters = new();

        public void Tick(float time)
        {
            foreach (var affectedReceiver in _affectedReceivers)
            {
                affectedReceiver.Receive();
            }
        }

        public void Reset(){}

        private void Start()
        {
            // GET THE SPAWNER
            _bonusSpawners = Extensions.FindComponents<BonusSpawner>().ToList();

            if (_bonusSpawners.Count < bonusManagerConfig.RoundBonusAssociationList.Count)
            {
                Debug.LogError($"There is not enough spawner in the level. There is currently {_bonusSpawners.Count} for {bonusManagerConfig.RoundBonusAssociationList.Count} bonus in the roundBonusAssociationList list");
            }

            RoundEvents.RoundNumber.AddListener(OnRoundNumberChanged);
            BonusManagerEvents.SubscribeElement.AddListener(OnReceiveSubscribeRequest);
            BonusManagerEvents.UnsubscribeElement.AddListener(OnReceiveUnsubscribeRequest);
            BonusManagerEvents.RegisterAura.AddListener(OnRegisterAura);
        }

        private void OnRoundNumberChanged(int roundNum, int totalRounds)
        {
            foreach (RoundBonusTypeTuple roundBonusTypeTuple in bonusManagerConfig.RoundBonusAssociationList)
            {
                if (roundBonusTypeTuple.roundNumber == roundNum)
                {
                    SpawnBonus(roundNum, roundBonusTypeTuple.bonusType);
                    return;
                }
            }
        }

        private GameObject GetPrefabFromType(EBonusType bonusType)
        {
            foreach (BonusTypePrefabTuple bonusTypePrefabTuple in bonusManagerConfig.BonusTypePrefabAssociationList)
            {
                if (bonusTypePrefabTuple.bonusType == bonusType)
                {
                    return bonusTypePrefabTuple.prefab;
                }
            }

            return null;
        }

        private AbstractBonusConfig GetConfigOfType(EBonusType bonusType)
        {
            switch (bonusType)
            {
                case EBonusType.Metal:
                    return bonusManagerConfig.MetalBonusConfig;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SpawnBonus(int roundNum, EBonusType bonusType)
        {
            if (_bonusSpawners.Count == 0)
            {
                Debug.LogError($"Error while spawning bonus of type {bonusType.ToString()}: Not enough spawner free");
                return;
            }

            int randomIndex = Random.Range(0, _bonusSpawners.Count);
            BonusSpawner bonusSpawner = _bonusSpawners[randomIndex];
            _bonusSpawners.Remove(bonusSpawner);
            bonusSpawner.SpawnBonus(GetPrefabFromType(bonusType), GetConfigOfType(bonusType));

            BonusManagerEvents.OnSpawnBonus.Invoke(roundNum, bonusSpawner, bonusType);
        }


        private void OnReceiveSubscribeRequest(EBonusType bonusType, GameObject receivingGameobject, Aura emitingAura)
        {
            // GAME OBJECT ALREADY IS RECEIVING AN EFFECT
            if (_auraRegistries[bonusType].ContainsKey(receivingGameobject))
            {
                // AURA ALREADY REGISTERED TO THIS AURA
                if (_auraRegistries[bonusType][receivingGameobject].Contains(emitingAura))
                {
                    return;
                }
                // AURA NOT REGISTERED TO THIS AURA
                _auraRegistries[bonusType][receivingGameobject].Add(emitingAura);
            }
            // GAME OBJECT IS NOT RECEIVING AN EFFECT
            else
            {
                // CREATE A LIST
                _auraRegistries[bonusType].Add(receivingGameobject, new List<Aura>{emitingAura});

                // ASSIGN THE CONFIG TO THE AURA RECEIVER
                IAuraReceiver auraReceiver = receivingGameobject.GetComponent<IAuraReceiver>();
                switch (bonusType)
                {
                    case EBonusType.Metal:
                        AbstractBonusEffector abstractBonusEffector = new MetalBonusEffector();
                        abstractBonusEffector.BonusConfig = bonusManagerConfig.MetalBonusConfig;
                        auraReceiver.BonusEffectors.Add(abstractBonusEffector);
                        auraReceiver.StartAffecting(abstractBonusEffector);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                _affectedReceivers.Add(auraReceiver);
            }
        }

        private void OnReceiveUnsubscribeRequest(EBonusType bonusType, GameObject gameobject, Aura aura)
        {
            // GAME OBJECT ALREADY IS NOT RECEIVING AN EFFECT
            if (!_auraRegistries[bonusType].ContainsKey(gameobject))
            {
                return;
            }

            // REMOVE ITSELF
            _auraRegistries[bonusType][gameobject].Remove(aura);

            // CHECK IF LIST IS EMPTY
            if (_auraRegistries[bonusType][gameobject].Count == 0)
            {
                IAuraReceiver auraReceiver = gameobject.GetComponent<IAuraReceiver>();
                switch (bonusType)
                {
                    case EBonusType.Metal:
                        MetalBonusEffector metalBonusEffector = (MetalBonusEffector)auraReceiver.BonusEffectors.First(m => m.BonusConfig.BonusType == EBonusType.Metal);
                        auraReceiver.BonusEffectors.Remove(metalBonusEffector);
                        auraReceiver.StopAffecting(metalBonusEffector);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                _auraRegistries[bonusType].Remove(gameobject);
                _affectedReceivers.Remove(auraReceiver);
            }
        }

        private void OnRegisterAura(Aura aura)
        {
            if (!_auraEmiters.Contains(aura))
            {
                _auraEmiters.Add(aura);
                aura.BonusConfig.AffectPositive(aura);
            }
        }

    }
}