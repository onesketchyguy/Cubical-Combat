using CubeFarm.Player;
using Mirror;
using System.Linq;
using UnityEngine;
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
    public class NetworkGamePlayer : NetworkRoomPlayer
    {
        [SyncVar]
        public int score;

        [Command]
        public void CmdSetScore(int score)
        {
            this.score = score;
        }

        public Text respawnText;

        [SyncVar]
        internal bool playerDead = true;

        public GameObject playerPrefab;

        [Tooltip("Time in seconds that it will take to spawn/respawn this player.")]
        public float respawnTime = 3;

        private float lastTimeAlive;

        private bool respawning;

        public GameObject cam;

        [SyncVar]
        public string displayName = AppDefaults.LOADING_USERNAME;

        private MultiplayerNetworkManager _;

        private MultiplayerNetworkManager room
        {
            get
            {
                if (_ != null) return _;
                else _ = NetworkManager.singleton as MultiplayerNetworkManager;

                return room;
            }
        }

        private PlayerSpawnSystem spawnSystem;

        private void Awake()
        {
            lastTimeAlive = Time.time;
        }

        private void Update()
        {
            if (hasAuthority == false)
            {
                cam.SetActive(false);
                return;
            }

            if (playerDead)
            {
                int timeBeforeSpawn = Mathf.CeilToInt((lastTimeAlive + respawnTime) - Time.time);

                respawnText.text = timeBeforeSpawn.ToString();

                if (timeBeforeSpawn <= Mathf.Epsilon && !respawning)
                {
                    // Respawn player
                    SpawnPlayer();
                    respawning = true;
                }
            }
            else
            {
                lastTimeAlive = Time.time;
                respawning = false;
            }

            cam.SetActive(playerDead);
        }

        private void SpawnPlayer()
        {
            CmdSpawnPlayer();
        }

        [Command]
        private void CmdSpawnPlayer()
        {
            if (spawnSystem == null)
            {
                spawnSystem = FindObjectOfType<PlayerSpawnSystem>();

                if (spawnSystem == null)
                {
                    // If the spawn point is still null we need to give it a sec and try again
                    Invoke(nameof(SpawnPlayer), 0.1f);

                    return;
                }
            }

            var point = spawnSystem.GetSpawnPoint(netId);
            transform.position = point.position;
            transform.rotation = point.rotation;

            if (transform.position != point.position)
            {
                Invoke(nameof(SpawnPlayer), 0.1f);

                return;
            }

            var playerInstance = Instantiate(playerPrefab, transform.position, transform.rotation);
            NetworkServer.Spawn(playerInstance, connectionToClient);
        }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);
            room.gamePlayers.Add(this);
        }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient()
        {
            room.gamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string val)
        {
            // We could be doing some validation here
            displayName = val;
        }
    }
}