using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI
{
    public class DifficultyIndicator : MonoBehaviour
    {
        [SerializeField] private Image[] stars;
        public int Value
        {
            get;
            set;
        }

        private void Update()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].gameObject.SetActive(i < Value);
            }
        }
    }
}