using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using LostPolygon.Apple.LocalMultiplayer.Networking;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    public class MultiplayerDemoGui : DemoGuiBase {
        public GameObject TapMarkerPrefab;

        public AppleLocalMultiplayerNetworkManagerHelper AppleLocalMultiplayerNetworkManagerHelper;
        public AppleLocalMultiplayerDemoNetworkManager AppleLocalMultiplayerDemoNetworkManager;

        public PeerBrowserController CustomDeviceBrowser;

        public GameObject UIPanelGameObject;
        public GameObject ErrorUIPanelGameObject;
        public GameObject SessionPeerListPanelGameObject;

        public GameObject StartServerButtonGameObject;
        public GameObject ConnectToServerButtonGameObject;
        public GameObject DisconnectButtonGameObject;

        public Toggle StressTestToggle;
        public Toggle UseCustomDeviceBrowserUIToggle;

        public Text SessionPeerListText;

#if !(UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX)
        private void Awake() {
            Debug.LogError(kWrongBuildPlatformMessage);
        }

        protected override void OnEnable() {
            base.OnEnable();

            UIPanelGameObject.SetActive(false);
            SessionPeerListPanelGameObject.SetActive(false);
            ErrorUIPanelGameObject.SetActive(true);
        }
#else
        // Spawns 100 actors instead of 1?
        private bool _isStressTestMode;

        protected override void Update() {
            base.Update();

            // Spawn an effect where user has tapped
            if (NetworkClient.active && Input.GetMouseButtonDown(0)) {
                Vector2 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Send the message with the tap position to the server, so it can send it to other clients
                NetworkManager.singleton.client.Send(
                    CreateTapMarkerMessage.kMessageType,
                    new CreateTapMarkerMessage { Position = tapPosition });

                // Local client can just instantiate the effect for immediate response, as this is a purely visual thing
                Instantiate(TapMarkerPrefab, tapPosition, Quaternion.identity);
            }

            // Refresh UI
            UIPanelGameObject.SetActive(AppleLocalMultiplayerNetworkManagerHelper.IsInitialized);
            ErrorUIPanelGameObject.SetActive(!AppleLocalMultiplayerNetworkManagerHelper.IsInitialized);

            // The plugin has failed to initialize for some reason, nothing else to do here
            if (!AppleLocalMultiplayerNetworkManagerHelper.IsInitialized)
                return;

            // Set UI state depending on whether session is active
            bool isSessionActive = AppleLocalMultiplayer.Session.IsSessionActive;
            StartServerButtonGameObject.SetActive(!isSessionActive);
            ConnectToServerButtonGameObject.SetActive(!isSessionActive);
            DisconnectButtonGameObject.SetActive(isSessionActive);
            if (DisconnectButtonGameObject.activeInHierarchy) {
                string disconnectButtonText;
                if (NetworkServer.active) {
                    disconnectButtonText = "Stop Server";
                } else {
                    if (!ClientScene.ready) {
                        disconnectButtonText = "Disconnect from Session";
                    } else {
                        disconnectButtonText = "Disconnect";
                    }
                }
                DisconnectButtonGameObject.GetComponentInChildren<Text>().text = disconnectButtonText;
            }

            bool togglesInteractable = !isSessionActive;
            bool togglesActive = !NetworkServer.active && !NetworkClient.active;

            // Deactivates toggles if needed
            StressTestToggle.interactable = togglesInteractable;
            StressTestToggle.gameObject.SetActive(togglesActive);

            UseCustomDeviceBrowserUIToggle.interactable = togglesInteractable;
            UseCustomDeviceBrowserUIToggle.gameObject.SetActive(togglesActive);

            // Fill the list of connected peers
            if (AppleLocalMultiplayer.Session.IsSessionActive) {
                // Construct peer list string
                string sessionPeerList = "";
                SessionPeerListPanelGameObject.SetActive(true);
                PeerId[] connectedPeers = AppleLocalMultiplayer.Session.GetConnectedPeers();
                for (int i = 0; i < connectedPeers.Length; i++) {
                    sessionPeerList += "<b>" + (i + 1) + ". </b>" + connectedPeers[i].Name + "\n";
                }

                if (sessionPeerList == "") {
                    sessionPeerList = "None";
                }
                SessionPeerListText.text = sessionPeerList.TrimEnd();
            } else {
                SessionPeerListPanelGameObject.SetActive(false);
            }

            // Update values
            AppleLocalMultiplayerDemoNetworkManager.StressTestMode = StressTestToggle.isOn;
        }

        private void Awake() {
            // Enable verbose logging. See device logs!
            AppleLocalMultiplayer.Log.SetVerboseLog(true);

            // Set the local peer name that will be seenby other peers
            AppleLocalMultiplayer.Session.SetLocalPeerName(SystemInfo.deviceName);

            // Set discovery info with some system info that will be sent to other peers when advertising
            AppleLocalMultiplayerNetworkManagerHelper.DiscoveryInfo = CreateSimpleDiscoveryInfo();
        }

        protected override void OnGoingBackToMenu() {
            // Gracefully closing all connectivity and loading the menu
            try {
                AppleLocalMultiplayerDemoNetworkManager.StopHost();
                AppleLocalMultiplayer.Session.StopSession();
            } catch {
                //
            }
        }

        #region UI Handlers

        public void OnBackToMenuButton() {
            GoBackToMenu();
        }

        public void OnStartServerButton() {
            AppleLocalMultiplayerNetworkManagerHelper.StartHost();
        }

        public void OnConnectToServerButton() {
            AppleLocalMultiplayerNetworkManagerHelper.SetCustomDeviceBrowser(UseCustomDeviceBrowserUIToggle.isOn ? CustomDeviceBrowser : null);
            AppleLocalMultiplayerNetworkManagerHelper.StartClient();
        }

        public void OnDisconnectButton() {
            AppleLocalMultiplayerDemoNetworkManager.StopHost();
            AppleLocalMultiplayer.Session.StopSession();
        }

        #endregion
#endif
    }
}
