using System;
using System.Collections;
using GameLab.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace GameLab.UI.Game

{
    public class EnergyBarUI : MonoBehaviour

    {
        private bool _timeOut = false;

        const float Epsilon = 0.001f;

        private int _lastEnergy;

        [SerializeField] private float fadeOutTime = 0.5f;

        [SerializeField] private float bigCounterTreshold = 3;

        [SerializeField] private Text smallTimerText;

        [SerializeField] private CountdownNumberUI bigTimerText;

        private Coroutine _coroutine;

        private void Start()

        {
            RoundEvents.RoundIsStarting.AddListener(() => ShowSmallTimer(false));
            RoundEvents.RunStarted.AddListener(() => ShowSmallTimer(true));
            RoundEvents.EnergyRemaining.AddListener(HandleEnergyRemaining);
        }


        private void HandleEnergyRemaining(float arg0)

        {
            var currentEnergy = (int)arg0;


            if (currentEnergy > bigCounterTreshold)

            {
                _timeOut = false;

                ShowSmallTimer(true);

                smallTimerText.text = currentEnergy + "";
            }

            else if (currentEnergy > Epsilon)

            {
                _timeOut = false;

                ShowSmallTimer(false);

                if (currentEnergy < _lastEnergy)

                {
                    Instantiate(bigTimerText, transform).SetText(currentEnergy + "");
                }
            }


            _lastEnergy = currentEnergy;
        }
        private void ShowSmallTimer(bool shouldShow)
        {
            smallTimerText.transform.parent.gameObject.SetActive(shouldShow);
        }
    }
    
}