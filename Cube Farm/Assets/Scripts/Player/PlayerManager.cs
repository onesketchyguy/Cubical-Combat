using CubeFarm.Managment;
using Mirror;
using UnityEngine;

namespace CubeFarm.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnHealthModified))]
        private float health = AppDefaults.MAX_HEALTH;

        [SyncVar]
        internal int score;

        [SyncVar]
        internal string userName = "DEFAULT";

        public Transform[] localGraphics;
        public Transform[] remoteGraphics;

        [Tooltip("Objects only visable to the local player")]
        public int localLayer;

        [Tooltip("Objects visable to remove players but not the local player")]
        public int remoteLayer;

        public float GetHealth()
        {
            return health;
        }

        public void OnHealthModified(float lastHealth, float currentHealth)
        {
            // We should probably do something here...
            // ¯\_(ツ)_/¯

            // Check if dead
            if (currentHealth <= 0)
            {
                // Die
                CmdDie();
            }
        }

        public void ModifyHealth(float mod)
        {
            var modAmount = health + mod;

            if (modAmount > health)
            {
                // Healed
            }
            else
            {
                // Damaged
            }

            // Change the health
            health = Mathf.Clamp(modAmount, 0, AppDefaults.MAX_HEALTH);
        }

        internal NetworkGamePlayer manager;

        [Command]
        public void CmdSetUsername(string userName)
        {
            this.userName = userName;
        }

        [Command]
        public void CmdSetScore(int score)
        {
            this.score = score;
        }

        // destroy for everyone on the server
        [Command]
        private void CmdDie()
        {
            manager.playerDead = true;
            NetworkServer.Destroy(gameObject);
            manager.CmdSetScore(score);
        }

        [Command]
        private void CmdSetOwner()
        {
            foreach (var item in connectionToClient.clientOwnedObjects)
            {
                if (item.tag == "GameController")
                {
                    manager = item.gameObject.GetComponent<NetworkGamePlayer>();
                }
            }

            manager.playerDead = false;
            CmdSetScore(manager.score);
        }

        private void Start()
        {
            for (int i = 0; i < localGraphics.Length; i++)
                foreach (var item in localGraphics[i].GetComponentsInChildren<Transform>())
                    item.gameObject.layer = hasAuthority ? remoteLayer : localLayer;

            for (int i = 0; i < remoteGraphics.Length; i++)
                foreach (var item in remoteGraphics[i].GetComponentsInChildren<Transform>())
                    item.gameObject.layer = hasAuthority ? localLayer : remoteLayer;

            if (hasAuthority)
            {
                CmdSetUsername(PlayerNameInput.DisplayName);

                CmdSetOwner();
            }
        }

        private void Update()
        {
            if (!hasAuthority) return;

            if (Input.GetKeyDown(KeyCode.F12))
            {
                // Fullscreen toggle
                if (Screen.fullScreen == false)
                {
                    Screen.SetResolution(1280, 720, false);
                }
                else
                {
                    Screen.SetResolution((int)(1280 / 1.5f), (int)(720 / 1.5f), true);
                }
            }

            if (Input.GetKey(KeyCode.End))
            {
                ModifyHealth(-1);
            }
        }
    }
}