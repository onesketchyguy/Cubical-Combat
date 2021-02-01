using Mirror;
using UnityEngine;

namespace CubeFarm.Player
{
    public class PlayerAnimator : NetworkBehaviour
    {
        private Animator anim;

        public PlayerMovement movementManager;

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (!hasAuthority) return;

            CmdSetFloat("Speed", Input.GetAxis("Vertical") * 2);// this is awful, we should fix this. And by we I mean FUTURE FORREST
            CmdSetFloat("Crouch", movementManager.crouched ? 1 : 0);
        }

        [Command]
        private void CmdSetFloat(string key, float val)
        {
            RpcSetFloat(key, val);
        }

        [ClientRpc]
        private void RpcSetFloat(string key, float val)
        {
            anim.SetFloat(key, val);
        }
    }
}