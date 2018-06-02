using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace LostPolygon.Apple.LocalMultiplayer.Networking {
    /// <summary>
    /// Version of <see cref="NetworkManagerHUD"/> that uses
    /// <see cref="AppleLocalMultiplayerNetworkManagerHelper"/> for networking routines.
    /// </summary>
    [AddComponentMenu("Network/Apple Local Multiplayer/AppleLocalMultiplayerNetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [RequireComponent(typeof(AppleLocalMultiplayerNetworkManagerHelper))]
    public class AppleLocalMultiplayerNetworkManagerHUD : NetworkManagerHUD {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        private AppleLocalMultiplayerNetworkManagerHelper managerHelper;
#endif

        private void Awake() {
            manager = GetComponent<NetworkManager>();
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
            managerHelper = GetComponent<AppleLocalMultiplayerNetworkManagerHelper>();
#endif
        }

        private void Update() {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
            if (!showGUI)
                return;

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null) {
                if (Input.GetKeyDown(KeyCode.S)) {
                    managerHelper.StartServer();
                }
                if (Input.GetKeyDown(KeyCode.H)) {
                    managerHelper.StartHost();
                }
                if (Input.GetKeyDown(KeyCode.C)) {
                    managerHelper.StartClient();
                }
            }
            if (NetworkServer.active && manager.IsClientConnected()) {
                if (Input.GetKeyDown(KeyCode.X)) {
                    manager.StopHost();
                }
            }
#else
            BaseUpdate();
#endif
        }

        private void OnGUI() {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
            if (!showGUI)
                return;

            int xpos = 10 + offsetX;
            int ypos = 50 + offsetY;
            const int spacing = 60;
            const int spacingSmall = 24;
            const int buttonHeight = 55;

            bool noConnection =
                manager.client == null ||
                manager.client.connection == null ||
                manager.client.connection.connectionId == -1;

            if (!AppleLocalMultiplayer.Session.IsSessionActive && !manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null) {
                if (noConnection) {
                    if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Client(C)")) {
                        managerHelper.StartClient();
                    }

                    ypos += spacing;

                    if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Host(H)")) {
                        managerHelper.StartHost();
                    }
                    ypos += spacing;

                    if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Server Only(S)")) {
                        managerHelper.StartServer();
                    }
                    ypos += spacing;
                } else {
                    GUI.Label(new Rect(xpos, ypos, 200, buttonHeight), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..");
                    ypos += spacingSmall;

                    if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Cancel Connection Attempt")) {
                        manager.StopClient();
                    }
                }
            } else {
                if (NetworkServer.active) {
                    string serverMsg = "Server: port=" + manager.networkPort;
                    GUI.Label(new Rect(xpos, ypos, 300, 20), serverMsg);
                    ypos += spacingSmall;
                }
                if (manager.IsClientConnected()) {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                    ypos += spacingSmall;
                }
            }

            if (manager.IsClientConnected() && !ClientScene.ready) {
                if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Client Ready")) {
                    ClientScene.Ready(manager.client.connection);

                    if (ClientScene.localPlayers.Count == 0) {
                        ClientScene.AddPlayer(0);
                    }
                }
                ypos += spacing;
            }

            if (NetworkServer.active || manager.IsClientConnected()) {
                if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Stop (X)")) {
                    manager.StopHost();
                }
                ypos += spacing;
            } else if (AppleLocalMultiplayer.Session.IsSessionActive) {
                bool isServer =
                    AppleLocalMultiplayer.CustomServiceAdvertiser.IsAdvertising ||
                    AppleLocalMultiplayer.ServiceAdvertiser.IsAdvertising;
                if (!isServer) {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Connecting to local multiplayer server...");
                    ypos += spacingSmall;
                } else {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Starting local multiplayer server...");
                    ypos += spacingSmall;
                }
                if (GUI.Button(new Rect(xpos, ypos, 200, buttonHeight), "Stop (X)")) {
                    AppleLocalMultiplayer.Session.StopSession();
                }
                ypos += spacing;
            }
#else
            BaseOnGUI();
#endif
        }

        private void BaseUpdate() {
            // Call the base method via reflection since it's private
            typeof(NetworkManagerHUD).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
        }

        private void BaseOnGUI() {
            // Call the base method via reflection since it's private
            typeof(NetworkManagerHUD).GetMethod("OnGUI", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
        }
    }
}