using CubeFarm.Weapons;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace CubeFarm.Player
{
    public class PlayerShooting : NetworkBehaviour
    {
        public Transform handIkPoint;
        public Transform weaponParent;

        public List<WeaponObject> weapons = new List<WeaponObject>();

        private WeaponObject weapon;

        [SyncVar]
        public int currentWeaponIndex;

        [SyncVar]
        public int currentAmmo;

        private Dictionary<string, int> stored_currentAmmo = new Dictionary<string, int>();

        public int totalAmmo
        {
            get
            {
                int val = 0;
                if (weapon != null)
                    ammoPouch.TryGetValue(weapon.ammoType.name, out val);
                return val;
            }
        }

        private Dictionary<string, int> ammoPouch = new Dictionary<string, int>();

        private void Start()
        {
            // Equip starter weapon
            CmdEquipWeapon(0);
        }

        private void Update()
        {
            if (!hasAuthority) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))//left mouse button
            {
                if (currentAmmo <= 0)
                {
                    CancelInvoke(nameof(Shoot));

                    CmdReload();
                }
                else InvokeRepeating(nameof(Shoot), 0, weapon.fireRate);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                CancelInvoke(nameof(Shoot));

                if (currentAmmo <= 0) CmdReload();
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                currentWeaponIndex++;
                if (currentWeaponIndex >= weapons.Count)
                    currentWeaponIndex = 0;

                CmdEquipWeapon(currentWeaponIndex);
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                currentWeaponIndex--;
                if (currentWeaponIndex < 0)
                    currentWeaponIndex = weapons.Count - 1;

                CmdEquipWeapon(currentWeaponIndex);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdReload();
            }

            handIkPoint.position = weaponParent.position;
        }

        public void AddWeapon(WeaponObject weapon)
        {
            if (weapons.Contains(weapon) == false)
                weapons.Add(weapon);
            else CmdPickupAmmo(weapon.ammoType.name, weapon.startMagCount * weapon.ammoType.magSize);
        }

        private void Shoot()
        {
            CmdFire();
            //if (weapon.fireRate == 1) CancelInvoke(nameof(Shoot));
        }

        [Command]
        public void CmdPickupAmmo(string type, int ammoCount)
        {
            if (ammoPouch.ContainsKey(type) == false) ammoPouch.Add(type, ammoCount);
            else ammoPouch[type] += ammoCount;
        }

        [Command]
        public void CmdEquipWeapon(int weaponIndex)
        {
            // Handle the data
            RpcEquipWeapon(weaponIndex);

            // Remove the old weapon
            if (weaponParent.childCount > 0)
                RpcClearWeaponChildren();

            // Add the new weapon
            RpcCreateWeaponModel();
        }

        [ClientRpc]
        public void RpcEquipWeapon(int weaponIndex)
        {
            // Make sure we don't lose any ammo between weapon switches
            if (currentAmmo > 0)
            {
                if (stored_currentAmmo.ContainsKey(weapon.ammoType.name) == false)
                    stored_currentAmmo.Add(weapon.ammoType.name, currentAmmo);
                else stored_currentAmmo[weapon.ammoType.name] = currentAmmo;
                currentAmmo = 0;
            }

            weapon = weapons[weaponIndex];
            currentWeaponIndex = weaponIndex;

            if (ammoPouch.ContainsKey(weapon.ammoType.name) == false)
                ammoPouch.Add(weapon.ammoType.name, weapon.ammoType.magSize * weapon.startMagCount);

            if (stored_currentAmmo.ContainsKey(weapon.ammoType.name) == false)
                CmdReload();
            else currentAmmo = stored_currentAmmo[weapon.ammoType.name];
        }

        [ClientRpc]
        public void RpcCreateWeaponModel()
        {
            Instantiate(weapon.weaponModel, weaponParent);
        }

        [ClientRpc]
        public void RpcClearWeaponChildren()
        {
            foreach (var child in weaponParent.GetComponentsInChildren<Transform>())
            {
                if (child == weaponParent) continue;

                Destroy(child.gameObject);
            }
        }

        [Command]
        public void CmdFire()
        {
            if (currentAmmo <= 0) return;

            var pnt = weapon.firePoint;

            // Fire projectile
            var projectile = Instantiate(weapon.ammoType.projectile,
                weaponParent.transform.TransformPoint(pnt),
                weaponParent.rotation).GetComponent<Projectile>();

            projectile.source = gameObject;
            projectile.damage = weapon.ammoType.projectileDamage;
            projectile.launchForce = weapon.launchForce;
            projectile.destroyAfter = weapon.ammoType.lifeTime;

            // Deduct ammo
            currentAmmo--;

            NetworkServer.Spawn(projectile.gameObject);
            RpcOnFire();
        }

        [Command]
        public void CmdReload()
        {
            // Get the ammo missing from the magezine
            var missingAmmo = weapon.ammoType.magSize - currentAmmo;
            // Get the amount of ammo remaining
            var r_ammo = (ammoPouch[weapon.ammoType.name] - missingAmmo > 0) ? missingAmmo : ammoPouch[weapon.ammoType.name];

            currentAmmo += r_ammo;
            ammoPouch[weapon.ammoType.name] -= r_ammo;
        }

        [ClientRpc]
        public void RpcOnFire()
        {
            // Client needs feedback from this shot
            // Animate!
        }
    }
}