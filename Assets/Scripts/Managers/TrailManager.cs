using System;
using System.Collections.Generic;
using GameLab.Events;
using UnityEngine;

namespace GameLab.Managers
{
    public class TrailManager : MonoBehaviour
    {
        private readonly List<GameObject> _trails = new();
        private void Start()
        {
            TrailEvent.Draw.AddListener(OnDrawEvent);
            TrailEvent.Clear.AddListener(OnClear);
        }

        private void OnDestroy()
        {
            TrailEvent.Draw.RemoveListener(OnDrawEvent);
            TrailEvent.Clear.RemoveListener(OnClear);
        }

        private void OnDrawEvent(List<Vector3> lineToDraw, Color lineColor)
        {
            var trailGameObject = new GameObject(); 
            var lineRenderer = trailGameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = lineToDraw.Count;
            lineRenderer.SetPositions(lineToDraw.ToArray());
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = new Color(0, 1, 1, 0);
            
            _trails.Add(trailGameObject);
        }

        private void OnClear()
        {
            _trails.ForEach(Destroy);
            _trails.Clear();
        }
    }
}