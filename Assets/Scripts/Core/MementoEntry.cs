using System;
using UnityEngine;

namespace GameLab.Core
{
 
    [Serializable]
    public struct MementoEntry : IEquatable<MementoEntry>
    {
        public readonly float Time;
        public readonly Vector3 Direction;
        public readonly bool IsAccelerating;

        public MementoEntry(float time, Vector3 direction, bool isAccelerating)
        {
            Time = time;
            Direction = direction;
            IsAccelerating = isAccelerating;
        }

        public bool Equals(MementoEntry other)
        {
            return Mathf.Abs(Time - other.Time) < 1E-5 && Vector3.Distance(Direction, other.Direction) < 1E-5 && other.IsAccelerating == IsAccelerating;
        }

        public override bool Equals(object obj)
        {
            return obj is MementoEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Time, Direction);
        }
    }
}