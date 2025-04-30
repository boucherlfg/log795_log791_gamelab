using UnityEngine.Events;

namespace GameLab.Events
{
    public class LevelSelectorEvents
    {
        public static UnityEvent Left = new();
        public static UnityEvent Right = new();
        public static UnityEvent Select = new();
    }
}