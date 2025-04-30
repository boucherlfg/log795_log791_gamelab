using System.Collections;
using GameLab.Events;
using GameLab.Managers;
using TMPro;
using UnityEngine;
using GameLab.Enums;

namespace GameLab.Utility
{
    // must be executed before Level manager
    [DefaultExecutionOrder(-6)]
    public class CountdownScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        private void Start()
        {
            // RoundEvents.PlayerInitialized.AddListener(StartCountdown);
        }

        private void StartCountdown(PlayerNumber arg0)
        {
            StartCoroutine(CountdownRoutine());
        }

        IEnumerator CountdownRoutine()
        {
            label.text = "1";
            yield return new WaitForSeconds(1);
            label.text = "2";
            yield return new WaitForSeconds(1);
            label.text = "3";
            yield return new WaitForSeconds(1);
            label.text = "Go!";
            RoundEvents.RunStarted.Invoke();
        }
    }
}