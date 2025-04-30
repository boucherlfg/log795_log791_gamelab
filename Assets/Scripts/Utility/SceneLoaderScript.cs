using GameLab.Managers;
using UnityEngine;

namespace GameLab
{
    public class SceneLoaderScript : MonoBehaviour
    {
        [SerializeField] private bool loadDirect = false;
        [SerializeField] private SceneLoader.SceneTarget target;
        public void LoadScene()
        {
            if(loadDirect) SceneLoader.LoadDirect(target);
            else SceneLoader.Load(target);
        }
    }
}
