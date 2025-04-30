using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Behaviours;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.UI.Game;
using UnityEngine;
using GameLab.Enums;

namespace GameLab.Managers
{
    public class CTFScoreManager : MonoBehaviour
    {
        private bool _isCalculating = false;
        private int _player1Score;
        private int _player2Score;
        private GameObject _goal;
        private List<Zone.ZoneData> _zoneDatas = new();
        [SerializeField] private ScoreConfig scoreConfig;
        private VehicleData[] _vehicles = Array.Empty<VehicleData>();
        
        /// <summary>
        /// key is vehicle id and value is counter
        /// </summary>
        private Dictionary<int, float> _vehicleCounters;
        private void Start()
        {
            _goal = GameObject.FindGameObjectWithTag("Goal");
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            RoundEvents.TimesOut.AddListener(OnTimesOut);
            VehiculeEvent.OnScoreRegistered.AddListener(OnScoreRegistered);
        }

        private void OnDestroy()
        {
            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
            RoundEvents.TimesOut.RemoveListener(OnTimesOut);
            VehiculeEvent.OnScoreRegistered.RemoveListener(OnScoreRegistered);
        }

        private void Update()
        {
            if (!_isCalculating) return;
            CalculatePoints();
        }

        private void CalculatePoints()
        {
            // update counter if vehicle is in a zone
            _zoneDatas.ForEach(zone =>
            {
                var vehicle = _vehicles.FirstOrDefault(x => x.VehicleId == zone.VehicleId);
                if (!vehicle) return;

                _vehicleCounters[zone.VehicleId] += Time.deltaTime;
            });

            // score points everytime a counter reaches the scoring interval
            var vehiculesPertinents = _vehicleCounters.Where(x => x.Value > scoreConfig.ScoringInterval).ToArray();
            foreach (var vehicleCounter in vehiculesPertinents)
            {
                var vehicle = _vehicles.FirstOrDefault(x => x.VehicleId == vehicleCounter.Key);
                Debug.Assert(vehicle, $"{vehicle} should not be null");

                _vehicleCounters[vehicleCounter.Key] = 0;
                var score = GetVehicleScoreByZone(vehicle);

                switch (vehicle.PlayerNumber)
                {
                    case PlayerNumber.One:
                        _player1Score += score;
                        break;
                    case PlayerNumber.Two:
                        _player2Score += score;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                GameEvents.AccumulatedScoreCalculated.Invoke(_player1Score, _player2Score);
                ScoreDisplaySpawnerUI.Instance.DisplayPlayerScore(vehicle.transform, score, vehicle.PlayerNumber, vehicle.VehicleId);
            }
        }

        private void OnRunStarted()
        {
            _zoneDatas.Clear();
            _vehicles = FindObjectsByType<VehicleData>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            _vehicleCounters = _vehicles.ToDictionary(data => data.VehicleId, _ => 0f);
            _isCalculating = true;
        }
        private void OnTimesOut()
        {
            _isCalculating = false;
            CalculatePoints();
        }

        private void OnScoreRegistered(Zone.ZoneData zoneData)
        {
            if (!zoneData.Entered)
            {
                // remove all zone datas that match
                _zoneDatas.RemoveAll(x => x.Equals(zoneData));
                
                // put back vehicle counter to zero if vehicle aint in any zone anymore
                if (_zoneDatas.Any(x => x.VehicleId == zoneData.VehicleId)) return;
                _vehicleCounters[zoneData.VehicleId] = 0;
            }
            else
            {
                _zoneDatas.Add(zoneData);
            }
        }

        private int GetVehicleScoreByZone(VehicleData vehicle)
        {
            var playerId = vehicle.PlayerNumber;
            var vehicleId = vehicle.VehicleId;
            var zone = _zoneDatas.Where(x => x.PlayerId == playerId && x.VehicleId == vehicleId).OrderBy(x => -x.Score).FirstOrDefault();
            return zone.Score;
        }
    }
}