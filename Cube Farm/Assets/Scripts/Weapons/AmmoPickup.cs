using CubeFarm.Player;
using CubeFarm.Weapons;
using UnityEngine;

namespace CubeFarm.Pickups
{
    public class AmmoPickup : ItemPickup
    {
        public AmmoType ammoType;
        public int mags = 1;

        public override void OnPickedUp(GameObject player)
        {
            var playerShooting = player.GetComponent<PlayerShooting>();
            playerShooting.CmdPickupAmmo(ammoType.name, ammoType.magSize * mags);

            base.OnPickedUp(player);
        }
    }
}