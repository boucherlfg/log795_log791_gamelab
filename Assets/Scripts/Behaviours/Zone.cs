using System;
using GameLab.Events;
using GameLab.Managers;
using GameLab.Utility;
using UnityEngine;
using GameLab.Enums;

namespace GameLab.Behaviours
{
    public class Zone : MonoBehaviour
    {
        public struct ZoneData : IEquatable<ZoneData>
        {
            public int ZoneId;
            public int Score;
            public int VehicleId;
            public PlayerNumber PlayerId;
            public bool Entered;

            /// <summary>
            /// check if two zones are equal (minus Entered value)
            /// </summary>
            public bool Equals(ZoneData other)
            {
                return ZoneId == other.ZoneId && Score == other.Score && VehicleId == other.VehicleId && PlayerId == other.PlayerId;
            }

            public override bool Equals(object obj)
            {
                return obj is ZoneData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ZoneId, Score, VehicleId, (int)PlayerId, Entered);
            }
        }
        private int _id;
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _id = Extensions.idGenerator++;
        }

        [SerializeField] private int score;
        private void OnTriggerEnter(Collider other)
        {
            if (!IsVehicle(other, out VehicleData data)) return;

            VehiculeEvent.OnScoreRegistered.Invoke(new ZoneData
            {
                ZoneId = _id,
                Score = score,
                VehicleId = data.VehicleId,
                PlayerId = data.PlayerNumber,
                Entered = true
            });
        }
        private void OnTriggerExit(Collider other)
        {
            if (!IsVehicle(other, out VehicleData data)) return;

            VehiculeEvent.OnScoreRegistered.Invoke(new ZoneData
            {
                ZoneId = _id,
                Score = score,
                VehicleId = data.VehicleId,
                PlayerId = data.PlayerNumber,
                Entered = false
            });
        }

        bool IsVehicle(Collider collider, out VehicleData vehicleData)
        {
            return collider.TryGetComponent(out vehicleData);
        }
    }
}
