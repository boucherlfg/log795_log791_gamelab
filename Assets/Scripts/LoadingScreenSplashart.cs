using System;
using GameLab.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab
{
    public class LoadingScreenSplashart : MonoBehaviour
    {
        [SerializeField] private Sprite defaultSplashArt;
        [SerializeField] private Image image;

        private void Awake()
        {
            if (SceneLoader.loadingScreenSplashart != null)
            {
                image.sprite = SceneLoader.loadingScreenSplashart;
                SceneLoader.loadingScreenSplashart = null;
            }
            else
            {
                image.sprite = defaultSplashArt;
            }
        }
    }
}
