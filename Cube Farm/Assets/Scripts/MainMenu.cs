using UnityEngine;

namespace CubeFarm.Managment
{
    public class MainMenu : MonoBehaviour
    {
        public MultiplayerNetworkManager networkManager;

        public GameObject landingPagePanel;

        private void Awake()
        {
            AppDefaults.Init();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void HostLobby()
        {
            networkManager.StartHost();
            landingPagePanel.SetActive(false);
        }
    }
}