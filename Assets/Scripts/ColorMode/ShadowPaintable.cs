using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.Serialization;


namespace GameLab.ColorMode

{
    [DefaultExecutionOrder(10)]
    public class ShadowPaintable : MonoBehaviour, IPaintable
    {
        private static readonly int FresnelColor = Shader.PropertyToID("_Fresnel_Color");

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
            get => spreadConfig.ShadowSpreadConfig.fromCanonSplash;
        }

        public bool CanReceiveBumperSplash
        {
            get => spreadConfig.ShadowSpreadConfig.fromBumperSplash;
        }

        public bool CanReceiveGhostSplash
        {
            get => spreadConfig.ShadowSpreadConfig.fromGhostSplash;
        }

        public bool CanReceiveFallSplash
        {
            get => spreadConfig.ShadowSpreadConfig.fromFallSplash;
        }

        public bool CanCollisionWithPlayer
        {
            get => spreadConfig.ShadowSpreadConfig.fromCollisionWithPlayer;
        }

        public bool CanCollisionWithGhost
        {
            get => spreadConfig.ShadowSpreadConfig.fromCollisionWithGhost;
        }

        public bool CanBeRecolored
        {
            get => spreadConfig.ShadowSpreadConfig.canBeRecolored;
        }

        [SerializeField] private PlayerNumber owner;
        [SerializeField] private int score;
        [SerializeField] private float splashRadius = 1f;
        [SerializeField] private PlayerConfigs playerConfigs;
        [SerializeField] private PlayerColorChangerRevamped playerColorChangerRevamped;
        [SerializeField] private ColorSpreadConfig spreadConfig;
        [SerializeField] private GameObject redSplashPrefab;
        [SerializeField] private GameObject blueSplashPrefab;
        [SerializeField] private StudioEventEmitter splashSfx;

        private bool _hasBeenPainted = false;

        private VehicleData _vehiculeData;


        public void Start()
        {
            _vehiculeData = GetComponentInChildren<VehicleData>();
            owner = _vehiculeData.PlayerNumber;
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


            _hasBeenPainted = true;
            GameEvents.OnPaintableUpdated.Invoke(currentOwner, this);
            _vehiculeData.PlayerNumber = owner;
            UpdateColor();

            if (currentOwner != playerNumber && canSplash)
            {
                Splash();
            }
        }


        private void UpdateColor()
        {
            if (Owner == PlayerNumber.None)
            {
                return;
            }

            playerColorChangerRevamped.SetPlayerHue(this.GetComponentInChildren<MovingObject>(), (int)owner);
            gameObject.GetComponentInChildren<Renderer>().material.SetColor(FresnelColor, playerConfigs.ShadowColors[(int)owner - 1]);
        }


        private void Splash()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));

            List<IPaintable> paintables = ColorSplash.GetPaintablesInRadius(this, floorPosition, splashRadius);
            paintables = paintables.Where(p => p.CanReceiveGhostSplash).ToList();
            foreach (var paintable in paintables)
            {
                paintable.Paint(owner, false, ColorSource.ColorSplash);
            }
            if (_vehiculeData.PlayerNumber == PlayerNumber.One)
            {
                GameObject splash = Instantiate(redSplashPrefab, transform.position, Quaternion.identity);
                splash.transform.localScale = new Vector3(splashRadius, splashRadius, splashRadius);
            }
            else if (_vehiculeData.PlayerNumber == PlayerNumber.Two)
            {
                GameObject splash = Instantiate(blueSplashPrefab, transform.position, Quaternion.identity);
                splash.transform.localScale = new Vector3(splashRadius, splashRadius, splashRadius);
            }
            splashSfx.SetParameter("RADIUS", splashRadius);
            splashSfx.PlayWithTryCatch();
        }


        public void Reset()
        {
            _hasBeenPainted = false;
        }

        private void OnDrawGizmos()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(floorPosition, splashRadius);
        }
    }
}