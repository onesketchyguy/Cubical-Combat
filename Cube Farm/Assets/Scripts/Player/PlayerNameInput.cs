using UnityEngine;
using UnityEngine.UI;

namespace CubeFarm.Player
{
    public class PlayerNameInput : MonoBehaviour
    {
        public InputField inputField;
        public Button continueButton;

        public static string DisplayName { get; private set; }

        private const string NAMEKEY = "PLAYER_NAME";

        private void Start() => SetUpInputField();

        private void SetUpInputField()
        {
            var defName = "";

            if (PlayerPrefs.HasKey(NAMEKEY))
                inputField.text = defName = PlayerPrefs.GetString(NAMEKEY);

            SetPlayerName(defName);
        }

        public void SetPlayerName(string value)
        {
            continueButton.interactable = !string.IsNullOrEmpty(value);
        }

        public void SavePlayerName()
        {
            DisplayName = inputField.text;
            PlayerPrefs.SetString(NAMEKEY, DisplayName);
        }
    }
}