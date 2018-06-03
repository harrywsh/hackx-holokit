using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    /// <summary>
    /// Controls and update the log panel.
    /// </summary>
    public class LogWindowController : MonoBehaviour {
        public GameObject LogWindow;
        public Text LogText;

#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        private const int kMaxLogItems = 50;
        private string _logTextString = "";
        private readonly List<string> _logItems = new List<string>();

        private void OnEnable() {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        private void OnDisable() {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        private void Update() {
            LogText.text = _logTextString;
        }

        private void HandleLog(string logString, string stackTrace, LogType logType) {
            if (logType == LogType.Error || logType == LogType.Exception) {
                logString = string.Format("Error: {0}, stacktrace: \n {1}", logString, stackTrace);
            }

            _logItems.Add(logString);
            // Make sure the log is not too long
            if (_logItems.Count > kMaxLogItems) {
                string[] items = new string[kMaxLogItems];
                _logItems.CopyTo(_logItems.Count - kMaxLogItems, items, 0, kMaxLogItems);
                _logItems.Clear();
                _logItems.AddRange(items);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _logItems.Count; i++) {
                sb.Append(_logItems[_logItems.Count - i - 1]);
                if (i < _logItems.Count - 1) {
                    sb.Append('\n');
                }
            }

            _logTextString = sb.ToString();
        }

        #region UI handlers

        public void OnLogOpen() {
            LogWindow.SetActive(true);
        }

        public void OnLogClose() {
            LogWindow.SetActive(false);
        }

        public void OnLogClear() {
            _logItems.Clear();
            _logTextString = "";
        }

        #endregion
#endif
    }
}
