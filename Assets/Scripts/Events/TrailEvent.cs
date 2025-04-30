using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Events
{
    public static class TrailEvent
    {
        public static readonly UnityEvent Clear = new();

        public static readonly UnityEvent<List<Vector3>, Color> Draw = new();
        public static readonly UnityEvent<int, Color> Track = new();
        
    }
}