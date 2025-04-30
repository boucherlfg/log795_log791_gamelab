using GameLab.Menu;
using UnityEngine.Events;

namespace GameLab.Events
{
    public static class MainMenuEvents
    {
        public static UnityEvent<MainMenuSoundType> OnMainMenuSoundPlayed = new();
    }
}