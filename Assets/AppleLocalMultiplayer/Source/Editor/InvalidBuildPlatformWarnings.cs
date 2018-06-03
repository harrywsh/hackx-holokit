using UnityEngine;
#if !UNITY_2017_3_OR_NEWER
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace LostPolygon.Apple.LocalMultiplayer.Editor.Internal {
    public class InvalidBuildPlatformWarnings : MonoBehaviour {
#if !UNITY_2017_3_OR_NEWER
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
            if (target == BuildTarget.StandaloneOSXIntel ||
                target == BuildTarget.StandaloneOSXUniversal) {
                Debug.LogWarning("Apple Local Multiplayer only supports x86_64 macOS Editor and Player. 32-bit x86 builds are not supported");
            }
        }

        [InitializeOnLoadMethod]
        public static void OnLoad() {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXUniversal) {
                Debug.LogWarning("Apple Local Multiplayer only supports x86_64 macOS Editor and Player. 32-bit x86 builds are not supported");
            }
        }
#endif
    }
}
