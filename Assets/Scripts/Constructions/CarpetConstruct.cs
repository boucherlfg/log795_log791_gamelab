using System;
using FMODUnity;
using GameLab.Player;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Constructions
{
    public class CarpetConstruct: MonoBehaviour
    {
        private const string PlayerTag = "Player";
        private const string ShadowTag = "Shadow";
        
        [SerializeField] private float impulsiveForce = 100f;
        [Header("SFX")]
        [SerializeField] internal StudioEventEmitter boostSfx;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag) || other.CompareTag(ShadowTag))
            {
                if (other.gameObject.TryGetComponent(out MovingObject movingObject))
                {
                    Debug.Log("Boy");
                    movingObject.ExternalImpulsiveForceAffect(transform, impulsiveForce);
                    boostSfx.PlayWithTryCatch();
                }
            }
        }
    }
}
