using UnityEngine.Networking;

namespace LostPolygon.Apple.LocalMultiplayer.Networking {
    /// <summary>
    /// Version of <see cref="NetworkLobbyManager"/> that disconnects
    /// from the host if UNet client is stopped.
    /// </summary>
    /// <seealso cref="UnityEngine.Networking.NetworkLobbyManager" />
    public class AppleLocalMultiplayerNetworkLobbyManager : NetworkLobbyManager {
        public override void OnStopClient() {
            base.OnStopClient();

#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX            // Stopping all connectivity on Unity networking disconnect event
            AppleLocalMultiplayer.Session.StopSession();
#endif
        }

#if UNITY_EDITOR
        protected virtual void Reset() {
            OnValidate();
        }

        protected virtual void OnValidate() {
            networkAddress = "127.0.0.1";
        }
#endif
    }
}
