using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GameLab.Enums;
using GameLab.Events;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.ColorMode
{
    public class BumperPaintable : MonoBehaviour, IPaintable
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
            get => spreadConfig.BumperSpreadConfig.fromCanonSplash;
        }

        public bool CanReceiveBumperSplash
        {
            get => spreadConfig.BumperSpreadConfig.fromBumperSplash;
        }

        public bool CanReceiveGhostSplash
        {
            get => spreadConfig.BumperSpreadConfig.fromGhostSplash;
        }

        public bool CanReceiveFallSplash
        {
            get => spreadConfig.BumperSpreadConfig.fromFallSplash;
        }

        public bool CanCollisionWithPlayer
        {
            get => spreadConfig.BumperSpreadConfig.fromCollisionWithPlayer;
        }

        public bool CanCollisionWithGhost
        {
            get => spreadConfig.BumperSpreadConfig.fromCollisionWithGhost;
        }

        public bool CanBeRecolored
        {
            get => spreadConfig.BumperSpreadConfig.canBeRecolored;
        }

        [SerializeField] private Renderer renderer;
        [SerializeField] private int materialIdToSwitch = 0;
        [SerializeField] private PlayerNumber owner;
        [SerializeField] private int score;
        [SerializeField] private float splashRadius = 1f;
        [SerializeField] private PlayerConfigs playerConfigs;
        [SerializeField] private ColorSpreadConfig spreadConfig;
        [SerializeField] private GameObject redSplashPrefab;
        [SerializeField] private GameObject blueSplashPrefab;

        private bool _hasBeenPainted = false;

        private void Start()
        {
            Material[] materials = renderer.materials;
            materials[materialIdToSwitch] = playerConfigs.BumperDefaultMaterial;
            renderer.materials = materials;
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
            UpdateColor();
            if (canSplash)
            {
                Splash();
            }
        }


        private void UpdateColor()
        {
            if (Owner == PlayerNumber.None)
            {
                Material[] materials = renderer.materials;
                materials[materialIdToSwitch] = playerConfigs.BumperDefaultMaterial;
                renderer.materials = materials;
            }
            else
            {
                Material[] materials = renderer.materials;
                materials[materialIdToSwitch] = playerConfigs.BumperMaterials[(int)Owner - 1];
                renderer.materials = materials;
            }
        }


        private void Splash()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));

            List<IPaintable> paintables = ColorSplash.GetPaintablesInRadius(this, floorPosition, splashRadius);
            paintables = paintables.Where(p => p.CanReceiveBumperSplash).ToList();

            foreach (var paintable in paintables)
            {
                paintable.Paint(owner, false, ColorSource.ColorSplash);
            }

            if (owner == PlayerNumber.One)
            {
                GameObject splash = Instantiate(redSplashPrefab, transform.position, Quaternion.identity);
                splash.transform.localScale = new Vector3(splashRadius, splashRadius, splashRadius);
            }
            else if (owner == PlayerNumber.Two)
            {
                GameObject splash = Instantiate(blueSplashPrefab, transform.position, Quaternion.identity);
                splash.transform.localScale = new Vector3(splashRadius, splashRadius, splashRadius);
            }
        }


        public void Reset()
        {
            Owner = PlayerNumber.None;
            _hasBeenPainted = false;
            UpdateColor();
        }

        private void OnDrawGizmos()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(floorPosition, splashRadius);
        }
    }
}