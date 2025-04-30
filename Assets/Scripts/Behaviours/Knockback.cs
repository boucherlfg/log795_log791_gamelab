// #define DEBUG

using System;
using System.Collections;
using FMODUnity;
using GameLab.Enums;
using GameLab.Events;
using GameLab.Player;
using GameLab.ScriptableObjects;
using GameLab.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace GameLab.Behaviours
{
    public class Knockback : MonoBehaviour
    {
        [SerializeField] private KnockbacksConfig knockbacksConfig;
        [SerializeField] private MovingObject movingObject;
        [SerializeField] Rigidbody rb;

        [Header("SFX")]
        [SerializeField] private StudioEventEmitter wallKnockbackSfx;
        [SerializeField] private StudioEventEmitter playerKnockbackSfx;
        [SerializeField] private StudioEventEmitter shadowKnockbackSfx;
        
        [Header("VFX")]
        [SerializeField] private VisualEffect sparkVFXPrefab;
        [SerializeField] private float sparkLifetime = 1;

        private bool _canKnockback = true;
        private int _numOfFrameToDisable = 5;

        private (Vector3, float) CollideWithStaticObject(Collision other, ContactPoint contactPoint, Vector3 normal, float strength)
        {
            // HANDLE COLLISION WITH STATIC ELEMENT
            // Get Original Veclocity
            Vector3 originalVelocity = rb.linearVelocity + other.relativeVelocity;
            Vector3 reflection = Vector3.Reflect((-originalVelocity).normalized, normal);

#if DEBUG
            if (gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(contactPoint.point, originalVelocity, Color.blue, 2f);
                Debug.DrawRay(contactPoint.point, reflection * (originalVelocity.magnitude * strength), Color.cyan, 2f);
                Debug.DrawRay(contactPoint.point, normal, Color.black, 2f);
            }
#endif
            float calculatedStrength = (originalVelocity.magnitude * strength);
            if (calculatedStrength < knockbacksConfig.MinBounceStrength)
            {
                calculatedStrength = knockbacksConfig.MinBounceStrength;
            }

            Vector3 newVelocity = reflection * calculatedStrength;
            return (newVelocity, knockbacksConfig.DisableDelayCurve.AnimationCurve.Evaluate(originalVelocity.magnitude));
        }

        private (Vector3, float) CollideWithMovingObject(Collision other, ContactPoint contactPoint, Vector3 normal, float strength)
        {
            TrySpawnVFX(other.gameObject);

            // HANDLE COLLISION WITH DYNAMIC ELEMENT
            // Get Original Veclocity
            Vector3 originalVelocity = other.rigidbody.linearVelocity - (rb.mass / (rb.mass + other.rigidbody.mass)) * other.relativeVelocity;
            Vector3 originalVelocityOther = rb.linearVelocity + (other.rigidbody.mass / (rb.mass + other.rigidbody.mass)) * other.relativeVelocity;

            Vector3 originalDirection = (originalVelocity).normalized;

            Vector3 reflection = Vector3.Reflect(originalDirection, normal);

#if DEBUG
            if (gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(contactPoint.point, originalDirection, Color.blue, 2f);
                Debug.DrawRay(contactPoint.point, (-originalDirection), new(255, 255, 0), 2f);
                Debug.DrawRay(contactPoint.point, reflection, Color.cyan, 2f);
                Debug.DrawRay(contactPoint.point, normal, Color.black, 2f);
                Debug.DrawRay(contactPoint.point, originalVelocityOther, Color.red, 2f);
            }
#endif

            Vector3 combinedDirection = ((reflection + originalVelocityOther.normalized) / 2).normalized;
            float combinedMagnitude = ((originalVelocity.magnitude + originalVelocityOther.magnitude) / 2);

            float calculatedStrength = (combinedMagnitude * strength);
            if (calculatedStrength < knockbacksConfig.MinBounceStrength)
            {
                calculatedStrength = knockbacksConfig.MinBounceStrength;
            }

            Vector3 newVelocity = combinedDirection * calculatedStrength;

            return (newVelocity,
                knockbacksConfig.DisableDelayCurve.AnimationCurve.Evaluate((originalVelocity.magnitude + originalVelocityOther.magnitude) / 2));
        }

        private void Start()
        {
            sparkVFXPrefab.Stop();
            sparkVFXPrefab.Reinit();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_canKnockback)
            {
                return;
            }

            float strength;
            if (!knockbacksConfig.TryGetKnockbackValue(other.gameObject.tag, out strength))
            {
                return;
            }

            // Get Point of impact
            ContactPoint contactPoint = other.GetContact(0);

            // Calculate reflection direction
            Vector3 normal = contactPoint.normal;

            Vector3 newVelocity = Vector3.zero;
            float disableDelay;


            // NO RIGIDBODY, MOST LIKELY A STATIC OBJECT LIKE A WALL
            if (other.rigidbody == null)
            {
                if (other.collider.CompareTag("Rampe") && Vector3.Angle(contactPoint.normal, Vector3.up) < 90.0f - Double.Epsilon)
                {
                    return;
                }

                if (other.collider.CompareTag("Wall") && Vector3.Angle(contactPoint.normal, Vector3.up) < Double.Epsilon)
                {
                    return;
                }

                var collisionResult = CollideWithStaticObject(other, contactPoint, normal, strength);
                newVelocity = collisionResult.Item1;
                disableDelay = collisionResult.Item2;
            }
            // HAS A RIGIDBODY, MOST LIKELY A MOVING OBJECT LIKE A PLAYER OR AN ELEMENT WITH A MOVING MODIFIER
            else
            {
                var collisionResult = CollideWithMovingObject(other, contactPoint, normal, strength);
                newVelocity = collisionResult.Item1;
                disableDelay = collisionResult.Item2;
            }


            if (movingObject is PlayerScript player)
            {
                switch (other.gameObject.tag)
                {
                    case "Player":
                        GamepadEvents.CallVibration.Invoke((PlayerNumber)player.Id, VibrationSource.Player);
                        break;
                    case "Shadow":
                        GamepadEvents.CallVibration.Invoke((PlayerNumber)player.Id, VibrationSource.Shadow);
                        break;
                    case "GameWall":
                        GamepadEvents.CallVibration.Invoke((PlayerNumber)player.Id, VibrationSource.Wall);
                        break;
                }
            }

            if (movingObject)
            {
                movingObject.KnockBackWithDelay(disableDelay);
            }

            newVelocity = Vector3.ClampMagnitude(Vector3.Scale(newVelocity, new Vector3(1, 0, 1)), knockbacksConfig.MaxVelocityHorizontalAfterKnockback) +
                          Vector3.ClampMagnitude(Vector3.Scale(newVelocity, new Vector3(0, 1, 0)), knockbacksConfig.MaxVelocityVerticalAfterKnockback);
            rb.AddForce(newVelocity, ForceMode.VelocityChange);

            if (IsResponsibleToPlaySound(other))
            {
                if (other.transform.CompareTag("Player") )
                {
                    playerKnockbackSfx.SetParameter("IMPACT_POWER", Mathf.Clamp(newVelocity.magnitude, 0, 30));
                    playerKnockbackSfx.PlayWithTryCatch();
                }
                else if (other.transform.CompareTag("Shadow"))
                {
                    if (other.transform.GetComponent<VehicleData>().PlayerNumber != transform.GetComponent<VehicleData>().PlayerNumber)
                    {
                        shadowKnockbackSfx.SetParameter("IMPACT_POWER", Mathf.Clamp(newVelocity.magnitude, 0, 30));
                        shadowKnockbackSfx.PlayWithTryCatch();
                    }
                    else
                    {
                        playerKnockbackSfx.SetParameter("IMPACT_POWER", Mathf.Clamp(newVelocity.magnitude, 0, 30));
                        playerKnockbackSfx.PlayWithTryCatch();
                    }

                }
                else
                {
                    wallKnockbackSfx.PlayWithTryCatch();
                }
            }

            if (other.transform.CompareTag("Player") && gameObject.CompareTag("Player"))
            {
                CameraEvents.CameraShakeEvent.Invoke(EScreenShakeSource.ColisionBetweenPlayers);
            }

#if DEBUG
            if (gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(contactPoint.point, rb.linearVelocity, Color.yellow, 2f);
            }
#endif

            if (gameObject.TryGetComponent(out EnergyComponent energyComponent))
            {
                energyComponent.OnBounce();
            }

            _canKnockback = false;
            StartCoroutine(nameof(DisableForSomeFrames));
        }

        private bool IsResponsibleToPlaySound(Collision other)
        {
            bool otherIsAWall = other.rigidbody == null;
            bool otherIsShadow = other.transform.CompareTag("Shadow");
            bool otherIsSmallerPlayer = transform.TryGetComponent(out VehicleData vehicleData) && // WE HAVE VALID VEHICULE DATA
                                        other.gameObject.TryGetComponent(out VehicleData otherVehiculeData) && // AND THE OTHER VEHICULE HAS VALID VEHICULE DATA
                                        (vehicleData.IsPlayer && otherVehiculeData.IsPlayer) && // AND ONE OF THE VEHICULE IS A PLAYER
                                        vehicleData.VehicleId < otherVehiculeData.VehicleId; // AND WE HAVE THE SMALLEST ID;

            return otherIsAWall || otherIsShadow || otherIsSmallerPlayer;
        }

        private IEnumerator DisableForSomeFrames()
        {
            for (int i = 0; i < _numOfFrameToDisable; i++)
            {
                yield return null;
            }

            _canKnockback = true;
        }

        private void TrySpawnVFX(GameObject other)
        {
            if (other.CompareTag("Shadow"))
            {
                StartCoroutine(SpawnVFXCoroutine(other.gameObject));
                return;
            }

            if (!other.CompareTag("Player")) return;
            
            // if its not a shadow, its a player, check which one has greater ID
            var myID = gameObject.GetInstanceID();
            var theirID = other.GetInstanceID();

            // only one vehicle (the highest ID) spawns the VFX
            if (myID > theirID)
            {
                StartCoroutine(SpawnVFXCoroutine(other.gameObject));
            }
        }

        private IEnumerator SpawnVFXCoroutine(GameObject second)
        {
            var diff = second.transform.position - transform.position;
            var pos = transform.position + diff / 2f;
            var sparkVFXInstance = Instantiate(sparkVFXPrefab, pos, Quaternion.identity);
            sparkVFXInstance.Play();
            
            yield return new WaitForSeconds(sparkLifetime);
            
            sparkVFXInstance.Stop();
            sparkVFXInstance.Reinit();
            Destroy(sparkVFXInstance.gameObject);
        }
    }
}