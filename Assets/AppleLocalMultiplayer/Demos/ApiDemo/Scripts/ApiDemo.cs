using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    /// <summary>
    /// API demo. Uses all Local Multiplayer APIs in order to demonstrate their usage.
    /// </summary>
    public class ApiDemo : DemoGuiBase {
        public PeerBrowserController PeerBrowserController;
        public int ServerPort = 7777;
        public string ServiceType = "apidemo";

        public GameObject UIPanelGameObject;
        public GameObject ErrorUIPanelGameObject;
        public GameObject SessionPeerListPanelGameObject;

        public Text BluetoothStateText;
        public Text SessionPeerListText;

        public Button SessionStartStopButton;
        public Button EnableBluetoothButton;
        public Button CustomAdvertiserStartStopButton;
        public Button AdvertiserStartStopButton;

        public GameObject Canvas;
        public GameObject InvitationPanelPrefab;

        public GameObject[] SessionActiveDependentGameObjects;

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

        private void Start() {
            // Enable verbose logging. See device logs!
            AppleLocalMultiplayer.Log.SetVerboseLog(true);

            // The service type uniquely defines an local multiplayer application.
            // Application with different service types would not be able to see and interact with each other.
            AppleLocalMultiplayer.Session.SetServiceType(ServiceType);

            // The server port must be the same port used for networking
            AppleLocalMultiplayer.Session.SetServerPort(ServerPort);

            // Set the local peer name that will be seen by other peers
            AppleLocalMultiplayer.Session.SetLocalPeerName(SystemInfo.deviceName);

            // Update the UI
            UpdateUIAfterSessionStateChange();
        }

        protected override void Update() {
            base.Update();

            // Update UI
#if !(UNITY_STANDALONE_OSX || UNITY_TVOS)
            EnableBluetoothButton.interactable = !AppleLocalMultiplayer.Connectivity.IsBluetoothEnabled;
            BluetoothStateText.text =
                String.Format("Bluetooth is {0}", AppleLocalMultiplayer.Connectivity.IsBluetoothEnabled ? "ON" : "OFF");
#else
            EnableBluetoothButton.interactable = false;
            BluetoothStateText.text = "Only on iOS";
#endif
            CustomAdvertiserStartStopButton.GetComponentInChildren<Text>().text =
                AppleLocalMultiplayer.CustomServiceAdvertiser.IsAdvertising ? "Stop" : "Start";
            AdvertiserStartStopButton.GetComponentInChildren<Text>().text =
                AppleLocalMultiplayer.ServiceAdvertiser.IsAdvertising ? "Stop" : "Start";

            // Peer list control
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
        }

        protected override void OnEnable() {
            base.OnEnable();

            // Subscribe to events. Don't forget to unsubscribe later!
            AppleLocalMultiplayer.Session.PeerStateChanged += OnSessionPeerStateChanged;
            AppleLocalMultiplayer.Session.Started += OnSessionStarted;
            AppleLocalMultiplayer.Session.Stopped += OnSessionStopped;

            AppleLocalMultiplayer.CustomPeerDiscovery.PeerFound += OnCustomPeerDiscoveryPeerFound;
            AppleLocalMultiplayer.CustomPeerDiscovery.PeerLost += OnCustomPeerDiscoveryPeerLost;
            AppleLocalMultiplayer.CustomPeerDiscovery.StartFailed += OnCustomPeerDiscoveryStartFailed;

            AppleLocalMultiplayer.PeerDiscovery.NearbyPeerPresenting += OnPeerDiscoveryNearbyPeerPresenting;
            AppleLocalMultiplayer.PeerDiscovery.Cancelled += OnPeerDiscoveryCancelled;
            AppleLocalMultiplayer.PeerDiscovery.Finished += OnPeerDiscoveryFinished;

            AppleLocalMultiplayer.CustomServiceAdvertiser.InvitationReceived += OnCustomServiceAdvertiserInvitationReceived;
            AppleLocalMultiplayer.CustomServiceAdvertiser.StartFailed += OnCustomServiceAdvertiserStartFailed;

            AppleLocalMultiplayer.ServiceAdvertiser.InvitationDismissed += OnServiceAdvertiserInvitationDismissed;
            AppleLocalMultiplayer.ServiceAdvertiser.InvitationPresenting += OnServiceAdvertiserInvitationPresenting;

            PeerBrowserController.OnPeerPicked += OnPeerBrowserControllerPickedPeer;
        }

        protected override void OnDisable() {
            base.OnDisable();

            // Unsubscribe from events
            AppleLocalMultiplayer.Session.PeerStateChanged -= OnSessionPeerStateChanged;
            AppleLocalMultiplayer.Session.Started -= OnSessionStarted;
            AppleLocalMultiplayer.Session.Stopped -= OnSessionStopped;

            AppleLocalMultiplayer.CustomPeerDiscovery.PeerFound -= OnCustomPeerDiscoveryPeerFound;
            AppleLocalMultiplayer.CustomPeerDiscovery.PeerLost -= OnCustomPeerDiscoveryPeerLost;
            AppleLocalMultiplayer.CustomPeerDiscovery.StartFailed -= OnCustomPeerDiscoveryStartFailed;

            AppleLocalMultiplayer.PeerDiscovery.NearbyPeerPresenting -= OnPeerDiscoveryNearbyPeerPresenting;
            AppleLocalMultiplayer.PeerDiscovery.Cancelled -= OnPeerDiscoveryCancelled;
            AppleLocalMultiplayer.PeerDiscovery.Finished -= OnPeerDiscoveryFinished;

            AppleLocalMultiplayer.CustomServiceAdvertiser.InvitationReceived -= OnCustomServiceAdvertiserInvitationReceived;
            AppleLocalMultiplayer.CustomServiceAdvertiser.StartFailed -= OnCustomServiceAdvertiserStartFailed;

            AppleLocalMultiplayer.ServiceAdvertiser.InvitationDismissed -= OnServiceAdvertiserInvitationDismissed;
            AppleLocalMultiplayer.ServiceAdvertiser.InvitationPresenting -= OnServiceAdvertiserInvitationPresenting;

            PeerBrowserController.OnPeerPicked -= OnPeerBrowserControllerPickedPeer;
        }

        // Updates UI that is dependent on session being active or not
        private void UpdateUIAfterSessionStateChange() {
            bool isActive = AppleLocalMultiplayer.Session.IsSessionActive;
            SessionStartStopButton.GetComponentInChildren<Text>().text = isActive ? "Stop session" : "Start session";
            foreach (GameObject dependentGameObject in SessionActiveDependentGameObjects) {
                Button[] childrenButtons = dependentGameObject.GetComponentsInChildren<Button>();
                foreach (Button childrenButton in childrenButtons) {
                    childrenButton.interactable = isActive;
                }
            }
        }

        // Formats the discovery info dictionary for logging purposes
        private static string FormatDiscoveryInfo(IDictionary<string, string> discoveryInfo) {
            if (discoveryInfo == null)
                return "none";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in discoveryInfo) {
                sb.AppendFormat("key: \"{0}\", value: \"{1}\"\n", pair.Key, pair.Value);
            }

            return sb.ToString();
        }

        #region PeerBrowserController

        private void OnPeerBrowserControllerPickedPeer(PeerId peerId) {
            // Called when peer is tapped in a custom peer discovery UI
            AppleLocalMultiplayer.CustomPeerDiscovery.InvitePeer(peerId);
        }

        #endregion

        #region Session

        private void OnSessionPeerStateChanged(PeerId peerId, PeerState newPeerState) {
            Debug.LogFormat("Event - OnSessionPeerStateChanged, peer: \"{0}\", state: {1}", peerId, newPeerState);
        }

        private void OnSessionStarted() {
            Debug.LogFormat("Event - OnSessionStarted");
        }

        private void OnSessionStopped() {
            Debug.LogFormat("Event - OnSessionStopped");
        }

        #endregion

        #region CustomPeerDiscovery

        private void OnCustomPeerDiscoveryPeerFound(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo) {
            Debug.LogFormat("Event - OnCustomPeerDiscoveryPeerFound, peer: \"{0}\", discovery info: \n{1}", peerId, FormatDiscoveryInfo(peerDiscoveryInfo));
        }

        private void OnCustomPeerDiscoveryPeerLost(PeerId peerId) {
            Debug.LogFormat("Event - OnCustomPeerDiscoveryPeerLost, peer: \"{0}\"", peerId);
        }

        private void OnCustomPeerDiscoveryStartFailed(string error) {
            Debug.LogFormat("Event - OnCustomPeerDiscoveryStartFailed, error: \"{0}\"", error);
        }

        #endregion

        #region PeerDiscovery

        private void OnPeerDiscoveryNearbyPeerPresenting(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo, ref bool shouldPresent) {
            // We can set shouldPresent to false if we want, and the peer would not be presented to the list
            Debug.LogFormat("Event - OnPeerDiscoveryNearbyPeerPresenting, peer:  \"{0}\"', discovery info: \n{1}", peerId, FormatDiscoveryInfo(peerDiscoveryInfo));
        }

        private void OnPeerDiscoveryCancelled() {
            Debug.LogFormat("Event - OnPeerDiscoveryCancelled");
        }

        private void OnPeerDiscoveryFinished() {
            Debug.LogFormat("Event - OnPeerDiscoveryFinished");
        }

        #endregion

        #region CustomServiceAdvertiser

        private void OnCustomServiceAdvertiserInvitationReceived(PeerId invitingPeerId, AppleLocalMultiplayer.InvitationHandler invitationHandler) {
            Debug.LogFormat("Event - OnCustomServiceAdvertiserInvitationReceived, inviting peer:  \"{0}\"", invitingPeerId);

            // Create custom UI for accepting or dismissing an incoming invitation
            GameObject invitationPanelGameObject = Instantiate(InvitationPanelPrefab);
            invitationPanelGameObject.transform.SetParent(Canvas.transform, false);
            invitationPanelGameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            IncomingInvitationPanelData incomingInvitationPanelData = invitationPanelGameObject.GetComponent<IncomingInvitationPanelData>();
            incomingInvitationPanelData.InvitingText.text = invitingPeerId.Name;

            UnityAction onAccept = null;
            onAccept = () => {
                invitationHandler(true);
                incomingInvitationPanelData.AcceptButton.onClick.RemoveListener(onAccept);
                Destroy(invitationPanelGameObject);
            };

            UnityAction onDismiss = null;
            onDismiss = () => {
                invitationHandler(false);
                incomingInvitationPanelData.AcceptButton.onClick.RemoveListener(onDismiss);
                Destroy(invitationPanelGameObject);
            };

            incomingInvitationPanelData.AcceptButton.onClick.AddListener(onAccept);
            incomingInvitationPanelData.DismissButton.onClick.AddListener(onDismiss);
        }

        private void OnCustomServiceAdvertiserStartFailed(string error) {
            Debug.LogFormat("Event - OnCustomServiceAdvertiserStartFailed, error: {0}", error);
        }

        #endregion

        #region ServiceAdvertiser

        private void OnServiceAdvertiserInvitationDismissed() {
            Debug.LogFormat("Event - OnServiceAdvertiserInvitationDismissed");
        }

        private void OnServiceAdvertiserInvitationPresenting() {
            Debug.LogFormat("Event - OnServiceAdvertiserInvitationPresenting");
        }

        #endregion

        #region UI handlers

        protected override void OnGoingBackToMenu() {
            // Stop session before loading the menu
            AppleLocalMultiplayer.Session.StopSession();
        }

        public void OnSessionStartStop() {
            // Start/stop the session
            if (!AppleLocalMultiplayer.Session.IsSessionActive) {
                AppleLocalMultiplayer.Session.StartSession();
            } else {
                PeerBrowserController.Clear();
                AppleLocalMultiplayer.Session.StopSession();
            }

            UpdateUIAfterSessionStateChange();
        }

        public void OnBackToMenuButton() {
            GoBackToMenu();
        }

        public void OnCustomPeerDiscoveryOpen() {
            PeerBrowserController.Open();
        }

        public void OnPeerDiscoveryOpen() {
            AppleLocalMultiplayer.PeerDiscovery.OpenBrowser();
        }

        public void OnPeerDiscoveryClose() {
            AppleLocalMultiplayer.PeerDiscovery.CloseBrowser();
        }

        public void OnCustomServiceAdvertiserStartStop() {
            if (!AppleLocalMultiplayer.CustomServiceAdvertiser.IsAdvertising) {
                AppleLocalMultiplayer.CustomServiceAdvertiser.StartAdvertising(CreateSimpleDiscoveryInfo());
            } else {
                AppleLocalMultiplayer.CustomServiceAdvertiser.StopAdvertising();
            }
        }

        public void OnServiceAdvertiserStartStop() {
            if (!AppleLocalMultiplayer.ServiceAdvertiser.IsAdvertising) {
                AppleLocalMultiplayer.ServiceAdvertiser.StartAdvertising(CreateSimpleDiscoveryInfo());
            } else {
                AppleLocalMultiplayer.ServiceAdvertiser.StopAdvertising();
            }
        }

        public void OnBluetoothEnablePrompt() {
#if !(UNITY_STANDALONE_OSX || UNITY_TVOS)
            AppleLocalMultiplayer.Connectivity.OpenEnableBluetoothPrompt();
#endif
        }

        #endregion
#endif
    }
}
