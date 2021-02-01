using UnityEngine;
using Mirror;
using UnityEngine.UI;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkRoomPlayer.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomPlayer.html
*/

namespace CubeFarm.Managment
{
    /// <summary>
    /// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
    /// The RoomPrefab object of the NetworkRoomManager must have this component on it.
    /// This component holds basic room player data required for the room to function.
    /// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
    /// </summary>
    public class NetworkRoomPlayerLobby : NetworkRoomPlayer
    {
        public GameObject lobbyUI;
        public Text[] playerNameText;
        public Button startGameButton;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = AppDefaults.LOADING_USERNAME;

        internal bool isLeader
        {
            get
            {
                return startGameButton.gameObject.activeSelf;
            }
            set
            {
                startGameButton.gameObject.SetActive(value); // only the leader may start a game
            }
        }

        public void HandleReadyToStart(bool readyToStart)
        {
            UpdateDisplay();

            if (isLeader) startGameButton.interactable = readyToStart;
        }

        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

        [Command]
        private void CmdSetDisplayName(string val)
        {
            // We could be doing some validation here
            DisplayName = val;
        }

        [Command]
        public void CmdReadyUp()
        {
            // We could add a delay here so people wont spam the ready button like ass holes

            readyToBegin = !readyToBegin;

            room.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            // Check if the individual trying to start the game is the leader
            if (room.lobbyPlayers[0].connectionToClient != connectionToClient) return;

            // Start game
            room.StartGame();
        }

        private MultiplayerNetworkManager _r;

        private MultiplayerNetworkManager room
        {
            get
            {
                if (_r != null) return _r;
                else _r = NetworkManager.singleton as MultiplayerNetworkManager;

                return room;
            }
        }

        private void UpdateDisplay()
        {
            if (isLeader)
            {
                startGameButton.gameObject.SetActive(true);
            }

            if (!hasAuthority) // If a non local player has left or otherwise modified the ui update with them
            {
                foreach (var item in room.lobbyPlayers)
                {
                    if (item.hasAuthority)
                    {
                        item.UpdateDisplay();
                    }
                }

                lobbyUI.SetActive(false);

                return;
            }

            // Update
            for (int i = 0; i < playerNameText.Length; i++)
            {
                if (i < room.lobbyPlayers.Count)
                {
                    var player = room.lobbyPlayers[i];
                    var readyText = (player.readyToBegin ?
                        $" <color=green>✓</color>" :
                        $" <color=red>✗</color>");

                    playerNameText[i].text = player.DisplayName + readyText;
                }
                else
                {
                    playerNameText[i].text = AppDefaults.LOADING_USERNAME;
                }
            }
        }

        /// <summary>
        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
        /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStartAuthority()
        {
            CmdSetDisplayName(Player.PlayerNameInput.DisplayName);
            lobbyUI.SetActive(true);
        }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient()
        {
            readyToBegin = true; // We should wait until this client has voted for a map
            room.lobbyPlayers.Add(this);

            UpdateDisplay();
        }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient()
        {
            room.lobbyPlayers.Remove(this);

            UpdateDisplay();
        }

        /// <summary>
        /// Called when the local player object has been set up.
        /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            UpdateDisplay();
        }
    }
}