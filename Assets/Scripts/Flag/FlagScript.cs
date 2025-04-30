using FMODUnity;
using GameLab.Behaviours;
using GameLab.Events;
using GameLab.Utility;
using UnityEngine;

namespace GameLab.Flag
{
    public class FlagScript : MonoBehaviour
    {
        private float _collisionTime = 0;
        private int _flagId;
        
        [SerializeField] private float collisionTimeout = 1;
        [SerializeField] private int scoreValue = 3; 
        [SerializeField] private GameObject flagBearerPrefab;
        [Header("SFX")]
        [SerializeField] private StudioEventEmitter flagGrabSfx;

        private void Start()
        {
            
            _flagId = Extensions.idGenerator++;
            RoundEvents.RunEnded.AddListener(OnRoundEnded);
        }

        private void OnDestroy()
        {
           
            RoundEvents.RunEnded.RemoveListener(OnRoundEnded);
        }

        private void OnRoundEnded()
        {
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            
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
            
            var flagBearerInstance = Instantiate(flagBearerPrefab, other.transform);
            flagBearerInstance.transform.localPosition = Vector3.zero;
            flagBearerInstance.transform.localScale = Vector3.one * 2;
            var flagBearerScript = flagBearerInstance.GetComponent<FlagBearerScript>();
            flagBearerScript.VehicleData = vehicleData;
            flagBearerScript.ZoneData = new Zone.ZoneData()
            {
                Entered = true,
                Score = scoreValue,
                VehicleId = vehicleData.VehicleId,
                PlayerId = vehicleData.PlayerNumber,
                ZoneId = _flagId
            };
            
            VehiculeEvent.OnScoreRegistered.Invoke(flagBearerScript.ZoneData);
            flagGrabSfx.PlayWithTryCatch();
            gameObject.SetActive(false);
        }
    }
}
