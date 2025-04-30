using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Events
{
    public class MovingObjectEvent
    {
        public readonly UnityEvent<Vector3, Quaternion, bool> PositionChanged = new ();
    }
}