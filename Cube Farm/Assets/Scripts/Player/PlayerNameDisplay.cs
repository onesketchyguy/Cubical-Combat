using UnityEngine;
using UnityEngine.UI;

namespace CubeFarm.Player
{
    [RequireComponent(typeof(Text))]
    public class PlayerNameDisplay : MonoBehaviour
    {
        public PlayerManager player;

        private Text textObject;

        private void Start()
        {
            textObject = GetComponent<Text>();
        }

        private void Update()
        {
            textObject.text = player.userName;
        }
    }
}