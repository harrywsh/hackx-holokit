using System;
using UnityEngine;

namespace LostPolygon.Apple.LocalMultiplayer.Internal {
    /// <summary>
    /// Provides static access to global Unity messages.
    /// </summary>
    internal class UnityMessagesBroadcaster : SingletonMonoBehaviour<UnityMessagesBroadcaster> {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        public static event Action GenericMessageEntered;
        public static event Action ApplicationQuitEntered;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void UpdateInstanceOnLoad() {
            TryUpdateInstance();
        }

        protected override void Awake() {
            base.Awake();

            // Make it hidden and indestructible
            gameObject.hideFlags =
                HideFlags.NotEditable |
                HideFlags.HideInHierarchy |
                HideFlags.DontSaveInBuild |
                HideFlags.DontSaveInEditor;
        }

        private void OnApplicationFocus(bool focusStatus) {
            if (!focusStatus)
                return;

            if (GenericMessageEntered != null) GenericMessageEntered();
        }

        private void OnApplicationPause(bool pauseStatus) {
            if (!pauseStatus)
                return;

            if (GenericMessageEntered != null) GenericMessageEntered();
        }

        private void OnApplicationQuit() {
            if (ApplicationQuitEntered != null) ApplicationQuitEntered();
        }

        private void Update() {
            if (GenericMessageEntered != null) GenericMessageEntered();
        }
#endif
    }
}
