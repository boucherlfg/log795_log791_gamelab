using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameLab.Crowd
{
    public class CharacterGradinRandom : MonoBehaviour
    {
        [Serializable]
        public struct PlayerWeightTuple
        {
            public PlayerNumber player;
            public float weight;
        }
        [SerializeField] private Animator animator;
        [SerializeField] private Material[] materialsToChange;
        [SerializeField] private GameObject[] heads;
        [SerializeField] private Vector2 waitTime;
        [SerializeField] private Vector2 loseDisableWaitTime;
        [SerializeField] private Vector2 winDisableWaitTime;
        [SerializeField] private Vector2 drawDisableWaitTime;
        [FormerlySerializedAs("changeOfBeingPlayer")] [SerializeField] private List<PlayerWeightTuple> chanceOfBeingPlayer;

        private static readonly int Hue = Shader.PropertyToID("_Hue_Offset");
        private static readonly int IsPlaying = Animator.StringToHash("IsPlaying");
        private static readonly int AnimDraw = Animator.StringToHash("AnimDraw");
        private static readonly int AnimWin = Animator.StringToHash("AnimWin");
        private static readonly int AnimLose = Animator.StringToHash("AnimLose");
        private static readonly int IdleClip = Animator.StringToHash("IdleClip");
        private string[] _materialNames;
        private PlayerNumber _aFanOf;

        private void Awake()
        {
            GameEvents.TotalPointsUpdated.AddListener(OnRoundWinnerSelected);

            Random.InitState(this.GetInstanceID() + Random.Range(0, 1));
            float hue = Random.Range(0f, 10.0f);
            int randomHeadIndex = Random.Range(0, heads.Length);
            for (int i = 0; i < heads.Length; i++)
            {
                heads[i].SetActive(i == randomHeadIndex);
            }

            _materialNames = materialsToChange.Select(m => m.name).Distinct().ToArray();
            List<Material> materials = transform.GetComponentsInChildren<Renderer>()
                .SelectMany(r => r.materials)
                .Where(m => _materialNames.Any(matName => m.name.StartsWith(matName)))
                .ToList();

            foreach (var material in materials)
            {
                material.SetFloat(Hue, hue);
            }

            ChooseWhoToSupport();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(nameof(StartIdleWithDelay));
        }

        private void OnDestroy()
        {
            GameEvents.TotalPointsUpdated.RemoveListener(OnRoundWinnerSelected);
        }

        private void OnRoundWinnerSelected(List<PlayerNumber> winners)
        {
            PlayerNumber winnerOfLastRound = winners[^1];

            Random.InitState(this.GetInstanceID() + Random.Range(0, 1));
            if (winnerOfLastRound == PlayerNumber.None)
            {
                StartCoroutine(StartAnimAfterDelay(AnimDraw, Random.Range(0f, 0.5f)));
                StartCoroutine(StopAnimAfterDelay(AnimDraw, Random.Range(drawDisableWaitTime.x, drawDisableWaitTime.y)));
            }
            else if (winnerOfLastRound == _aFanOf)
            {
                StartCoroutine(StartAnimAfterDelay(AnimWin, Random.Range(0f, 0.5f)));
                StartCoroutine(StopAnimAfterDelay(AnimWin, Random.Range(winDisableWaitTime.x, winDisableWaitTime.y)));
            }
            else
            {
                StartCoroutine(StartAnimAfterDelay(AnimLose, Random.Range(0f, 0.5f)));
                StartCoroutine(StopAnimAfterDelay(AnimLose, Random.Range(loseDisableWaitTime.x, loseDisableWaitTime.y)));
            }
        }


        private static bool IsInRange(float min, float max, float value)
        {
            return min <= value && value <= max;
        }

        private void ChooseWhoToSupport()
        {
            if (chanceOfBeingPlayer.Count <= 0)
            {
                Debug.LogError("Not Enough entry for chanceOfBeingPlayer. Will support None");
                _aFanOf = PlayerNumber.None;
                return;
            }
            
            float currentMin = 0f;
            float currentMax = 0;
            Random.InitState(this.GetInstanceID() + Random.Range(0, 1));
            float randomWeight = Random.Range(0, chanceOfBeingPlayer.Sum(t => t.weight));
            for(int i = 0; i < chanceOfBeingPlayer.Count; i++)
            {
                currentMin = currentMax;
                currentMax = chanceOfBeingPlayer[i].weight;
                if (IsInRange(currentMin, currentMax, randomWeight))
                {
                    _aFanOf = chanceOfBeingPlayer[i].player;
                    return;
                }

                i++;
            };
            _aFanOf = PlayerNumber.None;
        }

        private IEnumerator StartIdleWithDelay()
        {
            animator.SetInteger(IdleClip, Random.Range(0, 3));
            yield return new WaitForSeconds(Random.Range(waitTime.x, waitTime.y));
            animator.SetTrigger(IsPlaying);
        }

        private IEnumerator StartAnimAfterDelay(int id, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetBool(id, true);
        }

        private IEnumerator StopAnimAfterDelay(int id, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetBool(id, false);
        }

    }
}
