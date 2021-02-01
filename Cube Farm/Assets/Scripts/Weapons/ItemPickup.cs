using Mirror;
using UnityEngine;

namespace CubeFarm.Pickups
{
    public class ItemPickup : NetworkBehaviour
    {
        public GameObject graphic;

        [Tooltip("Time in minutes to wait before respawning")]
        public float respawnWait = 3;

        private float wait;

        private void Update()
        {
            if (graphic.activeSelf) wait = Time.time + (respawnWait * 60);
            else if (Time.time > wait)
            {
                RpcEnableSelf();
            }
        }

        internal virtual void OnTriggerEnter(Collider other)
        {
            if (graphic.activeSelf == false) return;

            if (other.CompareTag("Player"))
            {
                // Pickup item
                OnPickedUp(other.gameObject);
            }
        }

        public virtual void OnPickedUp(GameObject player)
        {
            if (isServer)
                RpcDisableSelf();
            else
            {
                CmdDisableSelf();
            }
        }

        [Command]
        public void CmdDisableSelf()
        {
            RpcDisableSelf();
        }

        [ClientRpc]
        public void RpcDisableSelf()
        {
            graphic.SetActive(false);
        }

        [ClientRpc]
        public void RpcEnableSelf()
        {
            graphic.SetActive(true);
        }
    }
}