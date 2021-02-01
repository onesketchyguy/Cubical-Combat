using UnityEngine;
using UnityEngine.UI;

namespace CubeFarm.Managment
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        public MultiplayerNetworkManager networkManager;

        public GameObject landingPagePanel;
        public InputField ipInputField;
        public Button joinButton;
        private Text joinButtonText;

        private void Start()
        {
            joinButtonText = joinButton.GetComponentInChildren<Text>();
        }

        private void OnEnable()
        {
            MultiplayerNetworkManager.OnClientConnected += HandleClientConnected;
            MultiplayerNetworkManager.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            MultiplayerNetworkManager.OnClientConnected -= HandleClientConnected;
            MultiplayerNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void HostLobby()
        {
            networkManager.StartHost();
        }

        public void JoinLobby()
        {
            networkManager.networkAddress = ipInputField.text;
            networkManager.StartClient();

            joinButton.interactable = false;
            joinButtonText.text = "Trying to connect...";
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            joinButtonText.text = "Join";

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
            joinButtonText.text = "Join";
        }
    }
}