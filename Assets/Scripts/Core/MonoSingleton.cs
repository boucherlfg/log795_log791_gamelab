using System;
using UnityEngine;

namespace GameLab.Core
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance
        {
            get; private set;
        }
        protected virtual void Awake()
        {
            if (!Instance)
            {
                Instance = this as T;
            }
            else
            {
                Debug.LogWarning($"There already is an instance of {typeof(T).Name}");
            }
        }
    }
}