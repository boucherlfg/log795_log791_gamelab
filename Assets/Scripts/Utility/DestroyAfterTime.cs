using System.Collections;
using UnityEngine;
namespace GameLab.Utility
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float time;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}