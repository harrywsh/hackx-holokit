using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    /// <summary>
    /// The demos menu screen logic.
    /// </summary>
    public class DemoMenu : MonoBehaviour {
        protected virtual void OnEnable() {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        protected virtual void OnDisable() {
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            SceneLoadedHandler(scene.buildIndex);
        }

        protected virtual void SceneLoadedHandler(int buildIndex) {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            CameraFade.StartAlphaFade(Color.black, true, 0.25f, 0.0f);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                QuitApplication();
        }

        private void QuitApplication() {
            CameraFade.StartAlphaFade(Color.black, false, 0.25f, 0f, Application.Quit);
        }

        public void LoadLevel(string levelName) {
            CameraFade.StartAlphaFade(Color.black, false, 0.25f, 0f, () => SceneManager.LoadScene(levelName, LoadSceneMode.Single));
        }
    }
}