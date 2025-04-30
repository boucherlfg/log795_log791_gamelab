using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GameLab.Behaviours;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.ColorMode
{
    [DefaultExecutionOrder(10)]
    public class FallColorSplasher : MonoBehaviour
    {
        [SerializeField] private MovingObject movingObject;
        [SerializeField] private float minHeightToExplosiveLanding = 3.0f;
        [SerializeField] private float explosionSize;
        [SerializeField] private GameObject redSplashPrefab;
        [SerializeField] private GameObject blueSplashPrefab;


        [Header("SFX")]
        [SerializeField]
        private StudioEventEmitter fallOnGroundSfx;
        [SerializeField] private StudioEventEmitter splashSfx;

        private float _maxHeightReached = 0;
        private VehicleData _vehicleData;
        private bool _canPaint = true;
        private bool _colorSplat = false;

        private void Start()
        {
            _vehicleData = GetComponent<VehicleData>();
            RoundEvents.TimesOut.AddListener(OnRunEnded);
        }

        private void OnDestroy()
        {
            RoundEvents.TimesOut.RemoveListener(OnRunEnded);
        }

        private void OnRunEnded()
        {
            _canPaint = false;
        }

        private void Update()
        {
            if (_maxHeightReached > minHeightToExplosiveLanding && movingObject.IsOnGround)
            {
                fallOnGroundSfx.PlayWithTryCatch();
                _colorSplat = true;
                _maxHeightReached = 0;
            }

            if (!_canPaint)
            {
                return;
            }

            if (!movingObject)
            {
                return;
            }

            if (_colorSplat)
            {
                Splash();
                _colorSplat = false;
                return;
            }

            UpdateHeight();
        }

        private void UpdateHeight()
        {
            if (!movingObject.IsOnGround && transform.position.y > _maxHeightReached)
            {
                _maxHeightReached = transform.position.y;
            }
        }

        private void Splash()
        {
            if (_vehicleData.IsPlayer)
            {
                GamepadEvents.CallVibration.Invoke(_vehicleData.PlayerNumber, VibrationSource.Landing);
            }

            List<IPaintable> paintables = ColorSplash.GetPaintablesInRadius(null, Vector3.Scale(transform.position, new Vector3(1, 0, 1)), explosionSize);
            paintables = paintables.Where(p => p.CanReceiveFallSplash).ToList();

            foreach (var paintable in paintables)
            {
                paintable.Paint(_vehicleData.PlayerNumber, false,ColorSource.ColorSplash);
            }

            if (_vehicleData.PlayerNumber == PlayerNumber.One)
            {
                GameObject splash = Instantiate(redSplashPrefab, transform.position, Quaternion.identity);
                splash.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
            }
            else if (_vehicleData.PlayerNumber == PlayerNumber.Two)
            {
                GameObject splash = Instantiate(blueSplashPrefab, transform.position, Quaternion.identity);
                splash.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
            }
            splashSfx.SetParameter("RADIUS", explosionSize);
            splashSfx.PlayWithTryCatch();
        }

        private void OnDrawGizmos()
        {
            Vector3 floorPosition = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(floorPosition, explosionSize);
        }
    }
}