using CubeFarm.Managment;
using UnityEngine;
using UnityEngine.UI;

namespace CubeFarm.Player
{
    public class PlayerHealthBar : MonoBehaviour
    {
        public PlayerManager player;
        public Slider slider;

        private void Update()
        {
            slider.value = player.GetHealth() / AppDefaults.MAX_HEALTH;
        }
    }
}