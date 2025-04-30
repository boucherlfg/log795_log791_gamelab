using System;
using System.Collections;
using GameLab.ScriptableObjects;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace GameLab.Utility
{
    enum EHidingState
    {
        Hide,
        Show
    }

    public class Hidable : MonoBehaviour
    {
        [SerializeField] private AnimationCurveObject showAnimationCurve;
        [SerializeField] private AnimationCurveObject hideAnimationCurve;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private AnimationCurveObjectUtility _animationCurveObjectUtility;

        [SerializeField] private EHidingState _hidingState = EHidingState.Show;
        [SerializeField] private float _currentLerpValue = 0f;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            if (GetComponent<AnimationCurveObjectUtility>() == null)
            {
                gameObject.AddComponent<AnimationCurveObjectUtility>();
            }

            if (!TryGetComponent(out _animationCurveObjectUtility))
            {
                _animationCurveObjectUtility = gameObject.AddComponent<AnimationCurveObjectUtility>();
            }

            _animationCurveObjectUtility.OnAnimationTick.AddListener(OnAnimationTick);
            _animationCurveObjectUtility.OnAnimationEnd.AddListener(OnAnimationEnd);
        }

        public void Show()
        {
            if (_hidingState == EHidingState.Show)
            {
                return;
            }

            if (!_animationCurveObjectUtility)
            {
                return;
            }
            _hidingState = EHidingState.Show;


            _animationCurveObjectUtility.StopAnimation();

            // hidingState = EHidingState.Show;
            _animationCurveObjectUtility.animationCurve = showAnimationCurve;

            // IF WE WERE PLAYING AN ANIMATION, START WITH AN OFFSET
            if (_currentLerpValue > 0f + float.Epsilon)
            {
                _animationCurveObjectUtility.StartAnimationWithOffset(1 - _currentLerpValue);
            }
            // START FROM 0
            else
            {
                _animationCurveObjectUtility.StartAnimation();
            }
        }


        public void Hide()
        {
            if (_hidingState == EHidingState.Hide)
            {
                return;
            }

            if (!_animationCurveObjectUtility)
            {
                return;
            }

            _hidingState = EHidingState.Hide;

            _animationCurveObjectUtility.StopAnimation();

            // hidingState = EHidingState.Hide;
            _animationCurveObjectUtility.animationCurve = hideAnimationCurve;

            // IF WE WERE PLAYING AN ANIMATION, START WITH AN OFFSET
            if (_currentLerpValue > 0f + float.Epsilon)
            {
                _animationCurveObjectUtility.StartAnimationWithOffset(1 - _currentLerpValue);
            }
            // START FROM 0
            else
            {
                _animationCurveObjectUtility.StartAnimation();
            }
        }

        private void OnAnimationTick(float alpha)
        {
            _currentLerpValue = alpha;
            Material material = _renderer.material;
            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }

        private void OnAnimationEnd()
        {
            _currentLerpValue = 0f;
        }
    }
}