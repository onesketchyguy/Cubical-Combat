using UnityEngine;
using UnityEngine.UI;

namespace CubeFarm.Player
{
    [RequireComponent(typeof(Text))]
    public class PlayerScoreDisplay : MonoBehaviour
    {
        public PlayerManager player;

        private Text textObject;

        private void Start()
        {
            textObject = GetComponent<Text>();
        }

        private void Update()
        {
            textObject.text = player.score.ToString();
        }
    }
}