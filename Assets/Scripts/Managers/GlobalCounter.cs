using System;
using System.Collections.Generic;
using System.Linq;
using GameLab.Core;
using GameLab.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Managers
{
    [DefaultExecutionOrder(-50)]
    public class GlobalCounter : MonoBehaviour
    {
        private bool _running;
        private float _counter;
        private List<ITickable> _tickables = new();
        

        
        private void Awake()
        {
            RoundEvents.RunStarted.AddListener(OnRunStarted);
            RoundEvents.RunEnded.AddListener(OnRunEnded);
        }

        private void FixedUpdate()
        {
            if (!_running) return;

            _counter += Time.fixedDeltaTime;


            _tickables.ForEach(tickable => tickable.Tick(_counter));
        }

        private void OnDestroy()
        {
            RoundEvents.RunStarted.RemoveListener(OnRunStarted);
            RoundEvents.RunEnded.RemoveListener(OnRunEnded);
        }

        private void OnRunEnded()
        {
            _running = false;
            _counter = 0;
            _tickables.ForEach(t => t.Reset());
        }

        private void OnRunStarted()
        {
            _tickables = FindObjectsByType<MonoBehaviour>(
                                                    FindObjectsInactive.Include, 
                                                    FindObjectsSortMode.None)
                                                .OfType<ITickable>().OrderBy(t1 => t1.Priority).ToList();
            _running = true;
        }
    }
}
