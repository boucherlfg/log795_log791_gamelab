using System;
using System.Collections.Generic;
using FMODUnity;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.Serialization;


namespace GameLab.ColorMode
{
    public class GenericPaintable : MonoBehaviour, IPaintable
    {
        public PlayerNumber Owner
        {
            get => owner;
            private set => owner = value;
        }


        public int Score
        {
            get => score;
        }

        public bool CanReceiveCanonSplash
        {
            get => spreadConfig.TileAndWallsSpreadConfig.fromCanonSplash;
        }

        public bool CanReceiveBumperSplash
        {
            get => spreadConfig.TileAndWallsSpreadConfig.fromBumperSplash;
        }

        public bool CanReceiveGhostSplash
        {
            get => spreadConfig.TileAndWallsSpreadConfig.fromGhostSplash;
        }

        public bool CanReceiveFallSplash
        {
            get => spreadConfig.TileAndWallsSpreadConfig.fromFallSplash;
        }

        public bool CanCollisionWithPlayer
        {
            get => spreadConfig.TileAndWallsSpreadConfig.fromCollisionWithPlayer;
        }

        public bool CanCollisionWithGhost
        {
            get => spreadConfig.TileAndWallsSpreadConfig.fromCollisionWithGhost;
        }

        public bool CanBeRecolored
        {
            get => spreadConfig.TileAndWallsSpreadConfig.canBeRecolored;
        }

        [SerializeField] private Renderer renderer;
        [SerializeField] private PlayerNumber owner;
        [SerializeField] private int score;
        [SerializeField] private PlayerConfigs playerConfigs;
        [SerializeField] private ColorSpreadConfig spreadConfig;

        [Header("SFX")]
        [SerializeField]
        private StudioEventEmitter tileSoundPlayer1;
        [SerializeField]
        private StudioEventEmitter tileSoundPlayer2;


        private static readonly int MaskColorTeintId = Shader.PropertyToID("_Mask_Color_TInt");
        private static readonly int EmissiveId = Shader.PropertyToID("_Emissive_Intensity");
        private static readonly int EmissiveTint = Shader.PropertyToID("_EmissiveTint");
        private List<Color> _defaultColors = new List<Color>();
        private bool _hasBeenPainted = false;


        private void Start()
        {
            foreach (Material material in renderer.materials)
            {
                _defaultColors.Add(material.GetColor(MaskColorTeintId));
            }
        }


        public void Paint(PlayerNumber playerNumber, bool canSplash, ColorSource colorSource)
        {
            if (_hasBeenPainted && !CanBeRecolored)
            {
                return;
            }

            PlayerNumber currentOwner = owner;
            if (playerNumber != PlayerNumber.None)
            {
                owner = playerNumber;
            }

            GameEvents.OnPaintableUpdated.Invoke(currentOwner, this);
            _hasBeenPainted = true;
            if (colorSource == ColorSource.PlayerPainter && currentOwner != playerNumber)
            {
                GamepadEvents.CallVibration.Invoke(owner, VibrationSource.TileColor);
                if (playerNumber == PlayerNumber.One)
                {
                    tileSoundPlayer2.Stop();
                    tileSoundPlayer1.PlayWithTryCatch();
                }
                else if (playerNumber == PlayerNumber.Two)
                {
                    tileSoundPlayer1.Stop();
                    tileSoundPlayer2.PlayWithTryCatch();
                }
            }
            UpdateColor();
        }


        private void UpdateColor()
        {
            if (Owner == PlayerNumber.None)
            {
                if (_defaultColors.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].SetColor(MaskColorTeintId, playerConfigs.TileMaskColorTintsDefault);
                    renderer.materials[i].SetColor(EmissiveTint, playerConfigs.TileEmissiveTintDefault);
                    renderer.materials[i].SetFloat(EmissiveId, 0.0f);
                }
            }

            else
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetColor(MaskColorTeintId, playerConfigs.TileMaskColorTints[(int)owner-1]);
                    material.SetColor(EmissiveTint, playerConfigs.TileEmissiveTints[(int)owner-1]);
                    material.SetFloat(EmissiveId, playerConfigs.TileEmissive);
                }
            }
        }


        public void Reset()
        {
            Owner = PlayerNumber.None;
            _hasBeenPainted = false;
            UpdateColor();
        }
    }
}