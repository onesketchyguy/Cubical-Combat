using UnityEngine;

namespace Mirror.Logging
{
    /// <summary>
    /// Used to replace log hanlder with Console Color LogHandler
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkHeadlessLogger")]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkHeadlessLogger.html")]
    public class NetworkHeadlessLogger : MonoBehaviour
    {
        [SerializeField] private bool showExceptionStackTrace = false;

        private void Awake()
        {
            if (NetworkManager.isHeadless)
            {
                LogFactory.ReplaceLogHandler(new ConsoleColorLogHandler(showExceptionStackTrace));
            }
        }
    }
}