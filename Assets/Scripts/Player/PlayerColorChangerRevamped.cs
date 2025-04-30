using System.Linq;
using GameLab.Behaviours;
using GameLab.ScriptableObjects;
using UnityEngine;

namespace GameLab.Player
{
    /// <summary>
    /// Was renamed to Revamped because unity isa piece of crap and PlayerColorChanger was broken even tho there was no error
    /// </summary>
    public class PlayerColorChangerRevamped : MonoBehaviour
    {
        private static readonly int HueOffset = Shader.PropertyToID("_Hue_Offset");
        public PlayerConfigs playerConfigs;

        public void SetPlayerHue(MovingObject player, int playerNumber)
        {
            if (player is PlayerScript)
            {
                PlayerScript playerScript = (PlayerScript)player;

                foreach (GameObject head in playerScript.Heads)
                {
                    head.SetActive(false);
                }

                playerScript.Heads[playerConfigs.PlayerHeadIndex[playerNumber-1]].SetActive(true);
            }

            var renderers = player.GetComponentsInChildren<Renderer>();
            var vehicleData = player.GetComponentInChildren<VehicleData>();
            var hue = playerConfigs.PlayerHues[playerNumber - 1];

            foreach (var rend in renderers)
            {
                var mats = rend.materials.Where(mat => playerConfigs.MaterialsToLookFor.Any(m => mat.name.StartsWith(m.name)));
                foreach (var mat in mats)
                {
                    mat.SetFloat(HueOffset, hue);
                }
            }

            if (vehicleData)
            {
                vehicleData.Color = Color.HSVToRGB(Mathf.Repeat(hue + HueOffset, 1f), 1f, 1f);
            }
        }
    }
}