
using UnityEngine;

namespace GameLab.Core
{
    public class MementoPosition
    {
        public readonly Vector3 Position;

        public MementoPosition(Vector3 initialPosition)
        {
            Position = initialPosition;
        }
    }
}