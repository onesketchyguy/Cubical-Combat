using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace CubeFarm.Weapons
{
    [CreateAssetMenu(menuName = "Weapon")]
    public class WeaponObject : ScriptableObject
    {
        public GameObject weaponModel;
        [Range(100f, 1000)] public float launchForce = 500;
        [Range(0, 1)] public float fireRate;
        public Vector3 firePoint;
        public AmmoType ammoType;
        public int startMagCount = 3;
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(WeaponObject))]
    public class WeaponObjectEditor : Editor
    {
        private string debugText;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var obj = (WeaponObject)target;

            if (GUILayout.Button("Set FirePoint"))
            {
                if (obj.weaponModel != null)
                {
                    bool foundPoint = false;

                    foreach (var item in obj.weaponModel.GetComponentsInChildren<Transform>())
                    {
                        if (item.name.ToLower().Contains("point"))
                        {
                            // Fire point located
                            obj.firePoint = item.localPosition;
                            foundPoint = true;
                            break;
                        }
                    }

                    if (foundPoint == false)
                        debugText = "Could not locate fire point." +
                            "\nPlease remember to name the firepoint \"FirePoint\" or something along those lines.";
                    else debugText = "Successfully set fire point.";
                }
                else
                {
                    debugText = "Weapon Model not set!";
                }
            }

            if (string.IsNullOrEmpty(debugText) == false)
            {
                GUILayout.TextArea(debugText);
            }
        }
    }

#endif
}