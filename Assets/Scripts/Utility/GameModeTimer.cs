using System;
using GameLab.Core;
using GameLab.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab.Utility
{
    public class GameModeTimer : MonoSingleton<GameModeTimer>

    {
    public UnityEvent<int> OnCountdownChangeUI = new UnityEvent<int>();

    /**
     * Event called every tick with the amount of time elapsed
     */
    public UnityEvent<float> OnTimerTick { get; private set; } = new();

    /**
     * Event triggered when the timer reaches 0
     */
    public UnityEvent OnTimerDone { get; private set; } = new();

    /**
     * Event triggered when the timer is started
     */
    public UnityEvent OnTimerStart { get; private set; } = new();

    /**
     * Event triggered when the timer is paused
     */
    public UnityEvent<bool> OnTimerPause { get; private set; } = new();

    private float timeLeft;
    private bool isTicking;

    private bool _isStartUp;

    // Make TimeLeft accessible as a property, while still allowing timeLeft to be set in the inspector
    public float TimeLeft
    {
        get => timeLeft;
    }

    public void PauseTimer(bool paused)
    {
        isTicking = !paused;
        OnTimerPause.Invoke(paused);
    }

    private void Update()
    {
        if (!isTicking)
            return;

        float tmp = timeLeft % 1;
        timeLeft -= Time.deltaTime;
        OnTimerTick.Invoke(Time.deltaTime);

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            StopTimer();

            OnTimerDone.Invoke();
        }
        else if (_isStartUp && tmp < timeLeft % 1)
            OnCountdownChangeUI.Invoke((int)(timeLeft + 1f));
    }


    public void StartTimer()
    {
        if (isTicking || timeLeft <= 0)
        {
            Debug.LogWarning("Timer is already started or time is 0. Please ensure you set a time");
            return;
        }

        isTicking = true;
        OnTimerStart.Invoke();
    }

    public void StopTimer()
    {
        if (!isTicking)
        {
            Debug.LogWarning("Timer is already stopped.");
            return;
        }

        isTicking = false;
    }

    public void SetTime(float newTime, bool isStartUp)
    {
        if (isTicking)
            Debug.LogWarning(
                "You're trying to set the time on a running timer. It is recommended to stop the timer first, update the value and then start it back again");

        timeLeft = newTime;
        _isStartUp = isStartUp;
    }
    }
}
