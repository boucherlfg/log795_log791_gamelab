using FMODUnity;
using UnityEngine;

namespace GameLab.Utility
{
    public class SoundBankLoader : MonoBehaviour
    {
        [SerializeField] private StudioBankLoader studioBankLoader;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }


        public void LoadBanks()
        {
            studioBankLoader.Load();
        }
    }
}
