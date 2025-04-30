using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GameLab.Constructions;
using GameLab.Constructions.Canons;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameLab.ColorMode
{
    public class CanonPaintable : MonoBehaviour, IPaintable
    {
        private static readonly int MaskHueOffset = Shader.PropertyToID("_Mask_Hue_Offset");

        public bool CanReceiveCanonSplash
        {
            get => spreadConfig.CanonSpreadConfig.fromCanonSplash;
        }

        public bool CanReceiveBumperSplash
        {
            get => spreadConfig.CanonSpreadConfig.fromBumperSplash;
        }

        public bool CanReceiveGhostSplash
        {
            get => spreadConfig.CanonSpreadConfig.fromGhostSplash;
        }

        public bool CanReceiveFallSplash
        {
            get => spreadConfig.CanonSpreadConfig.fromFallSplash;
        }

        public bool CanCollisionWithPlayer
        {
            get => spreadConfig.CanonSpreadConfig.fromCollisionWithPlayer;
        }

        public bool CanCollisionWithGhost
        {
            get => spreadConfig.CanonSpreadConfig.fromCollisionWithGhost;
        }

        public bool CanBeRecolored
        {
            get => spreadConfig.CanonSpreadConfig.canBeRecolored;
        }

        [SerializeField] private Renderer renderer;
        [SerializeField] private int materialIdToSwitch = 0;
        [SerializeField] private CanonConstruct canonConstruct;
        [SerializeField] private PlayerNumber owner;
        [SerializeField] private int score;
        [SerializeField] private float splashRadius = 1f;
        [SerializeField] private PlayerConfigs playerConfigs;
        [SerializeField] private ColorSpreadConfig spreadConfig;

        private bool _hasBeenPainted = false;

        public PlayerNumber Owner
        {
            get => owner;

            private set => owner = value;
        }

        public int Score
        {
            get => score;
        }

        private bool _isTaken = false;

        private void Start()
        {
            canonConstruct.OnCanonShoot.AddListener(OnShoot);
            canonConstruct.OnCanonTaken.AddListener(OnCanonTaken);
            renderer.materials[materialIdToSwitch].SetFloat(MaskHueOffset ,playerConfigs.CanonDefaultHue);
        }

        private void OnCanonTaken(PlayerNumber playerNumber)
        {
            PlayerNumber currentOwner = owner;
            if (_hasBeenPainted && !CanBeRecolored)
            {
                return;
            }

            owner = playerNumber;
            GameEvents.OnPaintableUpdated.Invoke(currentOwner, this);
            _hasBeenPainted = true;
            UpdateColor();
        }

        private void OnDestroy()
        {
            canonConstruct.OnCanonShoot.RemoveListener(OnShoot);
            canonConstruct.OnCanonTaken.RemoveListener(OnCanonTaken);
        }

        private void OnShoot()
        {
            if (owner == PlayerNumber.None)
            {
                return;
            }

            Splash();
            _isTaken = false;
        }

        private void Splash()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            List<IPaintable> paintables = ColorSplash.GetPaintablesInRadius(null, floorPosition, splashRadius);
            paintables = paintables.Where(p => p.CanReceiveCanonSplash).ToList();

            foreach (var paintable in paintables)
            {
                paintable.Paint(owner, false, ColorSource.ColorSplash);
            }
        }

        private void UpdateColor()
        {
            if (Owner == PlayerNumber.None)
            {
                // Material[] materials = renderer.materials;
                // materials[materialIdToSwitch] = playerConfigs.CanonDefaultMaterial;
                renderer.materials[materialIdToSwitch].SetFloat(MaskHueOffset ,playerConfigs.CanonDefaultHue);
            }
            else
            {
                renderer.materials[materialIdToSwitch].SetFloat(MaskHueOffset ,playerConfigs.CanonHues[(int)Owner - 1]);
            }
        }

        public void Paint(PlayerNumber playerNumber, bool canSplash, ColorSource colorSource)
        {
            return;

        }

        public void Reset()
        {
            owner = PlayerNumber.None;
            UpdateColor();
            _hasBeenPainted = false;
            _isTaken = false;
        }

        private void OnDrawGizmos()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(floorPosition, splashRadius);
        }
    }
}