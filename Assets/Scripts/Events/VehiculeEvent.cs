using GameLab.Behaviours;
using GameLab.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Events
{
    public static class VehiculeEvent
    {
        public static readonly UnityEvent<bool> OnOutOfEnergy = new();
        public static readonly UnityEvent<Zone.ZoneData> OnScoreRegistered = new();
        public static readonly UnityEvent<Vector3> Move = new();
    }
}