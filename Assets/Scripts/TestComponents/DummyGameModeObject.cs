using GameLab.Core;
using GameLab.Managers;
using UnityEngine;

namespace GameLab.TestComponents
{
    public class DummyGameModeObject : MonoBehaviour, IInitializable, IStartable
    {
        public void OnInitialize()
        {
            // nothing
        }

        public void OnStart()
        {
            // nothing
        }
    }
}