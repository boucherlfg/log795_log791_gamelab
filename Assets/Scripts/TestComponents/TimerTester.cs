using GameLab.Utility;
using UnityEngine;

namespace GameLab.TestComponents
{
    public class TimerTester : MonoBehaviour
    {
        [SerializeField] private GameModeTimer timer;

        [SerializeField] private float timerTime;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            timer = GetComponent<GameModeTimer>();
            timer.OnTimerTick.AddListener(OnTimerTick);
            timer.OnTimerDone.AddListener(OnTimerDone);

            // Display the initial time left
            Debug.Log($"Time left: {timer.TimeLeft}");

            // Should display a warning since timer is already stopped
            Debug.Log("Stopping Timer, expecting a warning");
            timer.StopTimer();

            // Should change the time with no warning
            Debug.Log($"Setting time with no warning");
            timer.SetTime(timerTime, false);
            Debug.Log($"Time left: {timer.TimeLeft}");

            // Should start the timer with no warnings
            Debug.Log("Starting Timer with no warning");
            timer.StartTimer();


            // Should set the time with a warning
            Debug.Log($"Setting time - 5 with a warning");

            Debug.Log($"Time left: {timer.TimeLeft}");
            timer.SetTime(timerTime - 5, false);
            Debug.Log($"Time left: {timer.TimeLeft}");

            // Should display a warning
            Debug.Log("Starting Timer with a warning");
            timer.StartTimer();

            // Should stop the timer with no warning
            Debug.Log("Starting Timer with no warning");
            timer.StopTimer();

            // Should change the time with no warning
            Debug.Log($"Setting time with no warning");
            timer.SetTime(timerTime, false);
            Debug.Log($"Time left: {timer.TimeLeft}");

            // Should display a warning
            Debug.Log("Stopping Timer with a warning");
            timer.StopTimer();

            // Should start the timer with no issue
            Debug.Log("Starting Timer with no warning");
            timer.StartTimer();
        }


        void OnTimerTick(float timeElapsed)
        {
            Debug.Log($"Timer time elasped: {timeElapsed}");
        }

        void OnTimerDone()
        {
            Debug.Log("Timer done !");
        }
    }
}
