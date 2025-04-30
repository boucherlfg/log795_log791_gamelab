using UnityEngine;
using UnityEngine.UI;

namespace GameLab.UI.Game
{
    public class DoubleSlider : MonoBehaviour
    {
        [SerializeField] private Image part1;
        [SerializeField] private Image part2;

        [SerializeField] [Range(0, 1)] private float value1;
        [SerializeField] [Range(0, 1)] private float value2;
        
        [SerializeField] private TMPro.TMP_Text value1Text;
        [SerializeField] private TMPro.TMP_Text value2Text;
        
        public float Value1
        {
            get => value1;
            set
            {
                part1.fillAmount = value1 = value;
                value1Text.text = $"{(int)(100 * value)} %";
            }
        }
        public float Value2
        {
            get => value2;
            set
            {
                part2.fillAmount = value2 = value;
                value2Text.text = $"{(int)(100 * value)} %";
            } 
        }

        private void OnValidate()
        {
            if(part1) part1.fillAmount = value1;
            if(part2) part2.fillAmount = value2;
        }
    }
}
