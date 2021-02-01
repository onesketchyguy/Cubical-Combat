using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CubeFarm.Player
{
    [RequireComponent(typeof(Text))]
    public class PlayerAmmoDisplay : MonoBehaviour
    {
        public PlayerShooting player;
        private Text textObject;

        private void Start()
        {
            textObject = GetComponent<Text>();
        }

        private void Update()
        {
            textObject.text = $"<color=white><size=30>{player.currentAmmo}</size></color>\n<color=grey><size=25>{player.totalAmmo}</size></color>";
        }
    }
}