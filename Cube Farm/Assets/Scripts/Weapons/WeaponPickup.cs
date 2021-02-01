using CubeFarm.Player;
using CubeFarm.Weapons;
using UnityEngine;

namespace CubeFarm.Pickups
{
    public class WeaponPickup : ItemPickup
    {
        public WeaponObject weapon;

        public override void OnPickedUp(GameObject player)
        {
            var playerShooting = player.GetComponent<PlayerShooting>();
            playerShooting.AddWeapon(weapon);

            base.OnPickedUp(player);
        }
    }
}