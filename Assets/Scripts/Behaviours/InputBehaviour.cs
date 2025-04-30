using UnityEngine;

namespace GameLab.Behaviours
{
    public class InputBehaviour : MonoBehaviour
    {
        public void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}