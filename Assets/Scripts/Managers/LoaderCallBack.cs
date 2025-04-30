using System;
using System.Collections;
using UnityEngine;

namespace GameLab.Managers
{
    public class LoaderCallBack: MonoBehaviour
    {
        [SerializeField] private float delayInLoadingScene;
        private bool _isFirstUpdate;

        private void Start()
        {
            StartCoroutine(nameof(LoadWithDelay));
        }

        private IEnumerator LoadWithDelay()
        {
            yield return new WaitForSeconds(delayInLoadingScene);
            SceneLoader.LoaderCallBack();
        }
    }
}