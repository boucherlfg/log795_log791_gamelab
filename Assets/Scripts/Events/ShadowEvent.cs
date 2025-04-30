using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace GameLab.Events
{
    public class ShadowEvent
    {
        public readonly UnityEvent<Vector3> Move = new ();
        public readonly UnityEvent<Quaternion> Rotate = new ();
        public readonly UnityEvent<bool> Dash = new (); 
    }
}