using FMODUnity;
using GameLab.Behaviours;
using GameLab.Events;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Flag
{
    public class FlagBearerScript : MonoBehaviour
    {
        public Zone.ZoneData ZoneData
        {
            get;
            set;
        }
        public VehicleData VehicleData
        {
            get;
            set;
        }

        [SerializeField] private Renderer _renderer;
        private float _collisionTime = 0;
        [SerializeField] private float collisionTimeout = 1;
        [Header("SFX")]
        [SerializeField] private StudioEventEmitter flagStealSfx;
        private void Start()
        {
            // _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (Time.fixedTime - _collisionTime < collisionTimeout)
            {
                InvincibleEffect();
                return;
            }
            
            NotInvincibleEffect();
        }
        
        private void InvincibleEffect()
        {
            // pour l'instant on va juste changer le renderer de la sphère autour du joueur
            _renderer.enabled = true;
        }
        
        private void NotInvincibleEffect()
        {
            _renderer.enabled = false;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == gameObject) return;
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Shadow")) return;
            
            var vehicleData = other.GetComponent<VehicleData>();
            if (!vehicleData)
            {
                vehicleData = other.attachedRigidbody.GetComponentInChildren<VehicleData>();
                if (!vehicleData)
                {
                    Debug.LogError($"There should be a MovingObject script on {other.name}");
                    return;
                }
            }

            if (Time.fixedTime - _collisionTime < collisionTimeout) return;

            _collisionTime = Time.fixedTime;

            // remove flag from this
            var zoneData = new Zone.ZoneData()
            {
                Entered = false,
                Score = ZoneData.Score,
                VehicleId = ZoneData.VehicleId,
                PlayerId = ZoneData.PlayerId,
                ZoneId = ZoneData.ZoneId,
            };
            VehiculeEvent.OnScoreRegistered.Invoke(zoneData);
            VehicleData = vehicleData;
            transform.parent = VehicleData.transform;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one * 2;
            
            // attach flag to other
            zoneData = new Zone.ZoneData()
            {
                Entered = true,
                Score = ZoneData.Score,
                VehicleId = vehicleData.VehicleId,
                PlayerId = vehicleData.PlayerNumber,
                ZoneId = ZoneData.ZoneId
            };
            flagStealSfx.PlayWithTryCatch();

            ZoneData = zoneData;
            VehiculeEvent.OnScoreRegistered.Invoke(ZoneData);
        }
    }
}
