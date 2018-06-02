using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    /// <summary>
    /// Base GUI used for demos.
    /// </summary>
    public abstract class DemoGuiBase : MonoBehaviour {
#if !(UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX)
        protected const string kWrongBuildPlatformMessage =
            "Build platform is not set to iOS, tvOS, or Standalone macOS. Please select a compatible build platform in 'File - Build Settings...'";
#endif

        protected virtual void OnEnable() {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        protected virtual void OnDisable() {
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        protected virtual void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.tvOS) {
                Application.targetFrameRate = 60;
            }
            Screen.sleepTimeout = 500;
            if (Time.frameCount != 0) {
                CameraFade.StartAlphaFade(Color.black, true, 0.25f, 0.0f);
            }
        }

        protected virtual void OnDestroy() {
            Application.targetFrameRate = -1;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        protected virtual void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GoBackToMenu();
            }
        }

        protected void GoBackToMenu() {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
            OnGoingBackToMenu();
#endif
            CameraFade.StartAlphaFade(Color.black, false, 0.25f, 0f, () => SceneManager.LoadScene("DemoMenu", LoadSceneMode.Single));
        }

#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        protected abstract void OnGoingBackToMenu();
#endif

        // Returns a discovery info with some system info that will be sent to other peers when advertising
        protected static IDictionary<string, string> CreateSimpleDiscoveryInfo() {
            Dictionary<string, string> discoveryInfo = new Dictionary<string, string> {
                { "deviceName", SystemInfo.deviceName },
                { "platform", Application.platform.ToString() },
                { "deviceModel", SystemInfo.deviceModel },
                { "operatingSystem", SystemInfo.operatingSystem },
                { "systemLanguage", Application.systemLanguage.ToString() }
            };

            return discoveryInfo;
        }
    }
}
