using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameLab
{
    public class DelayedTrigger : MonoBehaviour
    {
        [SerializeField] private float time; 
        [SerializeField] private UnityEvent onTimeEnded;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(time);
            onTimeEnded.Invoke();
        }
    }
}
