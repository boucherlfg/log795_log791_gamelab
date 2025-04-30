using System;
using GameLab.Behaviours;
using GameLab.TestComponents;
using UnityEngine;

namespace GameLab.UI.Game
{
    public class ObjectivePointerUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform target;
        [SerializeField] private Transform owner;
        [SerializeField] private RectTransform rect;

        private Camera cam;
        private int _playerID;

        private void Awake()
        {
            for (var i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) != transform) continue;
                _playerID = i + 1;
                Debug.Log(_playerID);
                break;
            }

            transform.SetParent(transform.parent.parent);

            PlayerScript.onSpawnedTrigget += (_, tmp) =>
            {
                if (_playerID != tmp.Id) return;
                owner = tmp.transform;
                if (target)
                    enabled = true;
            };

            ScoreTester.onSpawnedTrigget += (_, tmp) =>
            {
                target = tmp;
                if (owner)
                    enabled = true;
            };
        }

        private void Start()
        {
            cam = Camera.main;
            if (!target || !owner)
                enabled = false;
        }

        private void Update()
        {
            if (!rect || !cam || !owner || !target)
            {
                return;
            }

            rect.anchoredPosition = cam.WorldToScreenPoint(owner.position) / canvas.scaleFactor;

            Vector2 tmp = ((Vector2)cam.WorldToScreenPoint(target.position) / canvas.scaleFactor - rect.anchoredPosition).normalized;
            float angle = Mathf.Atan2(tmp.y, tmp.x) * Mathf.Rad2Deg;
            rect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}