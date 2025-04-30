using System;
using System.Collections;
using System.Collections.Generic;
using GameLab.Enums;
using UnityEngine;

namespace GameLab.Constructions
{
    public class PlayerSpawner : MonoBehaviour
    {
        private static readonly int MaskHueOffset = Shader.PropertyToID("_Mask_Hue_Offset");
        private static readonly int EmissiveIntensity = Shader.PropertyToID("_Emissive_Intensity");
        private static readonly int MaskColorTInt = Shader.PropertyToID("_Mask_Color_TInt");

        [Header("Configuration")]
        [SerializeField] private Renderer baseRenderer;
        [SerializeField] private Collider triggerArea;
        [SerializeField] private PlayerNumber player = PlayerNumber.One;
        [SerializeField] private int spawnIndex;
        [SerializeField] private bool isSelected = false;
        [SerializeField] private Transform spawnLocation;

        [Header("Colors")]
        [SerializeField] private float activeEmissiveValue;
        [SerializeField] private float inactiveEmissiveValue;
        [SerializeField] private Color activeMaskColorTint;
        [SerializeField] private Color inactiveMaskColorTint;
        [SerializeField] private float playerOneHue;
        [SerializeField] private float playerTwoHue;

        [Header("Animation")]
        [SerializeField] private float animationDuration;

        private void Start()
        {
            Reset();
        }

        public PlayerNumber Player
        {
            get => player;
        }

        public int SpawnIndex
        {
            get => spawnIndex;
        }

        public Transform SpawnLocation
        {
            get => spawnLocation;
        }

        public void Reset()
        {
            UpdateRendering();
            triggerArea.isTrigger = true;
        }

        public void UpdateRendering()
        {
            UpdateEmissive();
            UpdateColor();
        }

        public void SelectSpawner(bool newIsSelected)
        {
            isSelected = newIsSelected;
        }

        private void UpdateEmissive()
        {
            baseRenderer.material.SetFloat(EmissiveIntensity, (isSelected) ? activeEmissiveValue : inactiveEmissiveValue );
            baseRenderer.material.SetColor(MaskColorTInt, (isSelected) ? activeMaskColorTint : inactiveMaskColorTint );
        }

        private void UpdateColor()
        {
            if (player == PlayerNumber.One)
            {
                baseRenderer.material.SetFloat(MaskHueOffset, playerOneHue);
            }
            else if (player == PlayerNumber.Two)
            {
                baseRenderer.material.SetFloat(MaskHueOffset, playerTwoHue);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // triggerArea.isTrigger = false;
            if (isSelected)
            {
                StartCoroutine(nameof(FadeOut));
            }
        }

        private IEnumerator FadeOut()
        {
           var startTime = Time.time;
            var targetTime = Time.time + animationDuration;


            while (Time.time <= targetTime)
            {
                var currentValue = Mathf.Clamp((Time.time - startTime)/animationDuration, 0f, 1);

                baseRenderer.material.SetFloat(EmissiveIntensity, Mathf.Lerp(activeEmissiveValue, inactiveEmissiveValue, currentValue));
                baseRenderer.material.SetColor(MaskColorTInt, Color.Lerp(activeMaskColorTint, inactiveMaskColorTint, currentValue));

                yield return null;
            }
            baseRenderer.material.SetFloat(EmissiveIntensity, inactiveEmissiveValue);
            baseRenderer.material.SetColor(MaskColorTInt, inactiveMaskColorTint);

        }
    }
}
