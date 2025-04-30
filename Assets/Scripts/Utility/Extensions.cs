using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLab.Utility
{
    public static class Extensions
    {
        public static int idGenerator = 0;

        public static string ToHexString(this Color color)
        {
            var r = (int)(255 * color.r);
            var g = (int)(255 * color.g);
            var b = (int)(255 * color.b);
            var a = (int)(255 * color.a);
            var rStr = r.ToString("X2");
            var gStr = g.ToString("X2");
            var bStr = b.ToString("X2");
            var aStr = a.ToString("X2");
            return $"#{rStr}{gStr}{bStr}{aStr}";
        }
        
        public static IEnumerable<T> FindComponents<T>() where T : class
        {
            IEnumerable<MonoBehaviour> result = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            result = result.Where(x => x is T);
            return result.Select(x => x as T);
        }

        public static void ExecuteWithTryCatch<T>(Action action) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T ex)
            {
                Debug.LogError(ex);
            }
        }
        
        public static void PlayWithTryCatch(this StudioEventEmitter studioEventEmitter) 
        {
            ExecuteWithTryCatch<Exception>(studioEventEmitter.Play);
        }
    }
}