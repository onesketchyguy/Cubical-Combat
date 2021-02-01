using UnityEngine;

namespace Mirror.Logging
{
    /// <summary>
    /// Used to load LogSettings in build
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkLogSettings")]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkLogSettings.html")]
    public class NetworkLogSettings : MonoBehaviour
    {
        [Header("Log Settings Asset")]
        [SerializeField] internal LogSettings settings;

#if UNITY_EDITOR

        // called when component is added to GameObject
        private void Reset()
        {
            LogSettings existingSettings = EditorLogSettingsLoader.FindLogSettings();
            if (existingSettings != null)
            {
                settings = existingSettings;

                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

#endif

        private void Awake()
        {
            RefreshDictionary();
        }

        private void OnValidate()
        {
            // if settings field is changed
            RefreshDictionary();
        }

        private void RefreshDictionary()
        {
            settings.LoadIntoDictionary(LogFactory.loggers);
        }
    }
}