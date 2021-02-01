using UnityEngine;

namespace CubeFarm.Weapons
{
    [CreateAssetMenu(menuName = "AmmoType")]
    public class AmmoType : ScriptableObject
    {
        public GameObject projectile;
        public int magSize = 360;
        [Range(1f, 50)] public float projectileDamage = 1;
        [Range(3, 10)] public float lifeTime = 3;
    }
}