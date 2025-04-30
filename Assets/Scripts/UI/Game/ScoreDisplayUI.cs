using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class ScoreDisplayUI: MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private float liveTime = 2f;
        private Transform _target;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            Destroy(gameObject, liveTime);
        }

        private void Update()
        {
            if (_camera)
                transform.forward = (transform.position - _camera.transform.position).normalized;
            transform.position += _target.position - transform.position;
        }

        public void SetScore(int score)
        {
            scoreText.text = $"+{score}";
        }

        public void SetFollow(Transform target)
        {
            _target = target;
        }
    }
}