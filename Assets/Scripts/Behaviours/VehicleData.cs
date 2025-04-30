using UnityEngine;
using GameLab.Enums;

namespace GameLab.Behaviours
{
    public class VehicleData : MonoBehaviour
    {
        public PlayerNumber PlayerNumber { get; set; }
        public int VehicleId { get; set; }
        public Color Color { get; set; }
        public float Hue { get; set; }
        public bool IsPlayer => VehicleId < 2; // managers should always assign vehicle id 0 and 1 to players
    }
}