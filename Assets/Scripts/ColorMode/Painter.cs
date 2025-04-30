using System;
using GameLab.Behaviours;
using GameLab.Events;
using UnityEngine;


namespace GameLab.ColorMode

{
    [DefaultExecutionOrder(10)]
    public class Painter : MonoBehaviour

    {
        [SerializeField] private VehicleData vehicleData;

        [SerializeField] private bool isShadow = false;

        private bool _canPaint = true;


        private void Start()
        {
            vehicleData = GetComponentInChildren<VehicleData>();
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

        private void Paint(IPaintable paintable)
        {
            // SHADOW DON'T PAINT SHADOWS
            if (isShadow && !paintable.CanCollisionWithGhost)
            {
                return;
            }

            if (!isShadow && !paintable.CanCollisionWithPlayer)
            {
                return;
            }

            paintable.Paint(vehicleData.PlayerNumber, true, (isShadow) ? ColorSource.ShadowPainter : ColorSource.PlayerPainter);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_canPaint)
            {
                return;
            }
            IPaintable paintable = other.transform.GetComponentInChildren<IPaintable>();
            if (paintable == null)

            {
                return;
            }
            Paint(paintable);
        }


        private void OnCollisionEnter(Collision other)
        {
            if (!_canPaint)
            {
                return;
            }
            IPaintable paintable = other.transform.GetComponentInChildren<IPaintable>();
            if (paintable == null)

            {
                return;
            }
            Paint(paintable);
        }
    }
}