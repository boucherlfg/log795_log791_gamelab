using UnityEngine.Events;

namespace GameLab.Events
{
    public static class LobbyEvent
    {
        public static readonly UnityEvent<int> PlayerJoined = new ();
        public static readonly UnityEvent<int> ReadyUp = new ();
        public static readonly UnityEvent TutorialNext = new ();

    }
}