/**
 * author: Jimmy Tremblay-Bernier
 */

using UnityEngine.Events;

namespace GameLab.Events
{
    public static class BootstrapEvent
    {
        public static readonly UnityEvent OnBootstrapInitialized = new();
    }
}