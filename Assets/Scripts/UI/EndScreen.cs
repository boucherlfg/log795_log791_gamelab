using System;
using System.Collections.Generic;
using GameLab.Behaviours;
using GameLab.Core;
using GameLab.Enums;
using GameLab.Player;
using System.Linq;
using FMODUnity;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameLab.UI
{
    public class EndScreen : MonoBehaviour
    {
        private static readonly int Defeat = Animator.StringToHash("Defeat");
        private static readonly int Victory = Animator.StringToHash("Victory");
        [SerializeField] private GameObject[] p1WinnerEffects;
        [SerializeField] private GameObject[] p2WinnerEffects;

        [SerializeField] private Animator p1Animator;
        [SerializeField] private Animator p2Animator;

        [SerializeField] private Button firstButton;
        [SerializeField] private PlayerConfigs playerConfigs;
        [SerializeField] private TMPro.TextMeshProUGUI playerName;
        [SerializeField] private TMPro.TextMeshProUGUI player1Score;
        [SerializeField] private TMPro.TextMeshProUGUI player2Score;

        [Header("SFX")]
        [SerializeField] private EventReference winSoundEvent;
        [SerializeField] private EventReference loseSoundEvent;
        [SerializeField] private StudioEventEmitter p1SoundEmitter;
        [SerializeField] private StudioEventEmitter p2SoundEmitter;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameEvents.OnPodiumReady.Invoke();
            SoundEvents.OnChangeBGMGlobalParam.Invoke(SoundEvents.GameParam, 5.0f);
            SoundEvents.OnChangeBGMGlobalParam.Invoke("SCORE_RATIO", 0);
            firstButton.Select();
            InputSystem.EnableDevice(Mouse.current);
            
            var winner = ScoreWinnerData.Instance.Winner;

            player1Score.text = ScoreWinnerData.Instance.Player1.ToString();
            player2Score.text = ScoreWinnerData.Instance.Player2.ToString();
            switch (winner)
            {
                case PlayerNumber.None:
                    playerName.text = "Nobody wins!";
                    playerName.color = playerConfigs.brightUI;
                    foreach (var p1 in p1WinnerEffects)
                    {
                        p1.gameObject.SetActive(false);
                    }

                    foreach (var p2 in p2WinnerEffects)
                    {
                        p2.gameObject.SetActive(false);
                    }

                    p1Animator.SetBool(Defeat, true);
                    p2Animator.SetBool(Defeat, true);
                    p1SoundEmitter.EventReference = loseSoundEvent;
                    p1SoundEmitter.PlayWithTryCatch();

                    p2SoundEmitter.EventReference = loseSoundEvent;
                    p2SoundEmitter.PlayWithTryCatch();
                    break;
                case PlayerNumber.One:
                    playerName.color = playerConfigs.player1UI;
                    playerName.text = "Red wins!";
                    foreach (var p2 in p2WinnerEffects)
                    {
                        p2.gameObject.SetActive(false);
                    }
                    p1Animator.SetBool(Victory, true);
                    p2Animator.SetBool(Defeat, true);

                    p1SoundEmitter.EventReference = winSoundEvent;
                    p1SoundEmitter.PlayWithTryCatch();

                    p2SoundEmitter.EventReference = loseSoundEvent;
                    p2SoundEmitter.PlayWithTryCatch();

                    break;
                case PlayerNumber.Two:
                    playerName.color = playerConfigs.player2UI;
                    playerName.text = "Blue wins!";
                    foreach (var p1 in p1WinnerEffects)
                    {
                        p1.gameObject.SetActive(false);
                    }

                    p1Animator.SetBool(Defeat, true);
                    p2Animator.SetBool(Victory, true);

                    p1SoundEmitter.EventReference = loseSoundEvent;
                    p1SoundEmitter.PlayWithTryCatch();

                    p2SoundEmitter.EventReference = winSoundEvent;
                    p2SoundEmitter.PlayWithTryCatch();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}