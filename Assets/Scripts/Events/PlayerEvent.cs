using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Events
{
    public class PlayerEvent
    {
        public readonly UnityEvent<Vector2> Move = new ();
        public readonly UnityEvent<bool> Accelerate = new ();
        public readonly UnityEvent Dash = new ();
    }
}