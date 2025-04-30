using Unity.VisualScripting;
using UnityEngine;

namespace GameLab
{

    public class S_HologramWall : MonoBehaviour
    {
        private Material material;

        private Material holo;

        private float timePassed;

        private Texture2D currentTexture;

        private Texture2D nextTexture;

        private int textureIndex;

        private bool lerping;

        private float percent;

        private float delay;

        [SerializeField]
        public float delayMin;
        [SerializeField]
        public float delayMax;


        [SerializeField]
        public Texture2D[] textures;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

            holo = GetComponent<MeshRenderer>().materials[1];

            timePassed = 0;

            lerping = false;

            percent = 0;

            currentTexture = textures[0];

            delay = Random.Range(delayMin, delayMax);

        }

        // Update is called once per frame
        void Update()
        {

            if (lerping == false)
            {
                timePassed += Time.deltaTime;
            }


            if (timePassed >= delay)
            {
                ChangeTexture();
    
            }

            if (lerping)
            {
                percent++;
                holo.SetFloat("_Lerp", percent / 500);

                Debug.Log(percent/500);

                if (percent/500 >= 1)
                {
                    percent = 0;
                    holo.SetFloat("_Lerp", 0);
                    lerping = false;

                    currentTexture = nextTexture;
                    holo.SetTexture("_Texture2", currentTexture);

                }

            }

        }

        void ChangeTexture()
        {
            timePassed = 0;
            textureIndex = Random.Range(0, textures.Length - 1);
            nextTexture = textures[textureIndex];


            holo.SetTexture("_NextTexture", nextTexture);

            lerping = true;

            delay = Random.Range(delayMin, delayMax);

        }
    }
}
