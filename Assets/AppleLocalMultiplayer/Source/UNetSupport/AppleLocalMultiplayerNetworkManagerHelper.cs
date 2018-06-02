using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace LostPolygon.Apple.LocalMultiplayer.Networking {
    /// <summary>
    /// A helper class that works in conjunction with <see cref="NetworkManager"/>.
    /// It automatically manages showing the device picker
    /// and correctly handling the local multiplayer session.
    /// </summary>
    /// <example>
    /// The NetworkManager.Start* family of methods is mirrored, just use this class
    /// instead of using NetworkManager directly to start your client/server/host.
    /// </example>
    [RequireComponent(typeof(NetworkManager))]
    public class AppleLocalMultiplayerNetworkManagerHelper : MonoBehaviour {
        [SerializeField]
        protected LocalMultiplayerNetworkManagerSettings _multipeerConnectivitySettings = new LocalMultiplayerNetworkManagerSettings();

        [SerializeField]
        [HideInInspector]
        protected NetworkManager _networkManager;

#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        private MultiplayerMode _desiredMode = MultiplayerMode.None;
        private Action _clientAction;
        private Action _hostAction;
        private PeerId _serverPeerId;

        /// <summary>
        /// A custom nearby peer browser can be used instead of a native one.
        /// </summary>
        private ICustomDeviceBrowser _customDeviceBrowser;

        /// <summary>
        /// Gets a value indicating whether the plugin has initialized successfully.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Discovery info sent to other peers when advertising.
        /// </summary>
        public IDictionary<string, string> DiscoveryInfo { get; set; }

        private void OnEnable() {
            _networkManager = GetComponent<NetworkManager>();

            //  Setting the service type. Must be unique for every application
            try {
                AppleLocalMultiplayer.Session.SetServiceType(_multipeerConnectivitySettings.ServiceType);
                IsInitialized = true;
            } catch (Exception e) {
                Debug.LogException(e);
            }

            // Registering the event listeners
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
        }

        private void OnDisable() {
            // Unregistering the event listeners
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
        }

        /// <summary>
        /// Sets the custom nearby peer browser.
        /// </summary>
        public void SetCustomDeviceBrowser(ICustomDeviceBrowser customDeviceBrowser) {
            if (_customDeviceBrowser != null) {
                _customDeviceBrowser.OnPeerPicked -= OnPeerPicked;
            }

            _customDeviceBrowser = customDeviceBrowser;
            if (_customDeviceBrowser != null) {
                _customDeviceBrowser.OnPeerPicked += OnPeerPicked;
            }
        }

        private void StartLocalMultiplayerClient(Action onReadyAction) {
            _clientAction = onReadyAction;

            _serverPeerId = null;
            _desiredMode = MultiplayerMode.Client;
            StartSession();
            if (_customDeviceBrowser != null) {
                _customDeviceBrowser.Open();
            } else {
                AppleLocalMultiplayer.PeerDiscovery.OpenBrowser();
            }
        }

        private void StartLocalMultiplayerHost(Action onReadyAction) {
            _hostAction = onReadyAction;

            _desiredMode = MultiplayerMode.Host;
            StartSession();
            AppleLocalMultiplayer.ServiceAdvertiser.StartAdvertising(DiscoveryInfo);
            if (_hostAction != null) {
                _hostAction();
                _hostAction = null;
            }
        }

        private void StopAll() {
            AppleLocalMultiplayer.ServiceAdvertiser.StopAdvertising();
            _networkManager.StopHost();
            _serverPeerId = null;
            _desiredMode = MultiplayerMode.None;
        }

        private void StartSession() {
            AppleLocalMultiplayer.Session.SetServerPort(_networkManager.networkPort);
            AppleLocalMultiplayer.Session.StartSession();
        }

        private void OnPeerPicked(PeerId peerId) {
            // Trying to invite the device picked by user
            AppleLocalMultiplayer.CustomPeerDiscovery.InvitePeer(peerId);
            if (_customDeviceBrowser != null) {
                _customDeviceBrowser.Close();
            }
        }

        #region NetworkManager methods

        /// <seealso cref="NetworkManager.StartClient()"/>
        public void StartClient() {
            StartLocalMultiplayerClient(() => _networkManager.StartClient());
        }

        /// <seealso cref="NetworkManager.StartClient()"/>
        public void StartClient(MatchInfo info) {
            StartLocalMultiplayerClient(() => _networkManager.StartClient(info));
        }

        /// <seealso cref="NetworkManager.StartClient()"/>
        public void StartClient(MatchInfo info, ConnectionConfig config) {
            StartLocalMultiplayerClient(() => _networkManager.StartClient(info, config));
        }

        /// <seealso cref="NetworkManager.StartServer()"/>
        public void StartServer() {
            StartLocalMultiplayerHost(() => _networkManager.StartServer());
        }

        /// <seealso cref="NetworkManager.StartServer()"/>
        public void StartServer(MatchInfo info) {
            StartLocalMultiplayerHost(() => _networkManager.StartServer(info));
        }

        /// <seealso cref="NetworkManager.StartServer()"/>
        public void StartServer(ConnectionConfig config, int maxConnections) {
            StartLocalMultiplayerHost(() => _networkManager.StartServer(config, maxConnections));
        }

        /// <seealso cref="NetworkManager.StartHost()"/>
        public void StartHost() {
            StartLocalMultiplayerHost(() => _networkManager.StartHost());
        }

        /// <seealso cref="NetworkManager.StartHost()"/>
        public void StartHost(MatchInfo info) {
            StartLocalMultiplayerHost(() => _networkManager.StartHost(info));
        }

        /// <seealso cref="NetworkManager.StartHost()"/>
        public void StartHost(ConnectionConfig config, int maxConnections) {
            StartLocalMultiplayerHost(() => _networkManager.StartHost(config, maxConnections));
        }

        #endregion

        #region  Events

        #region Session

        private void OnSessionPeerStateChanged(PeerId peerId, PeerState newPeerState) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnSessionPeerStateChanged, peer: \"{0}\", state: {1}", peerId, newPeerState);
            }

            if (_desiredMode == MultiplayerMode.Client) {
                // The first connected peer is the server
                if (newPeerState == PeerState.Connected && _serverPeerId == null) {
                    _serverPeerId = peerId;
                    AppleLocalMultiplayer.PeerDiscovery.CloseBrowser();
                    if (_clientAction != null) {
                        _clientAction();
                        _clientAction = null;
                    }
                }

                // Stop networking if disconnected from the server
                if (newPeerState == PeerState.NotConnected && _serverPeerId == peerId) {
                    StopAll();
                }
            }
        }

        private void OnSessionStarted() {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnSessionStarted");
            }
        }

        private void OnSessionStopped() {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnSessionStopped");
            }
        }

        #endregion

        #region CustomPeerDiscovery

        private void OnCustomPeerDiscoveryPeerFound(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnCustomPeerDiscoveryPeerFound, peer: \"{0}\", discovery info: \n{1}", peerId, FormatDiscoveryInfo(peerDiscoveryInfo));
            }
        }

        private void OnCustomPeerDiscoveryPeerLost(PeerId peerId) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnCustomPeerDiscoveryPeerLost, peer: \"{0}\"", peerId);
            }
        }

        private void OnCustomPeerDiscoveryStartFailed(string error) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnCustomPeerDiscoveryStartFailed, error: \"{0}\"", error);
            }
        }

        #endregion

        #region PeerDiscovery

        private void OnPeerDiscoveryNearbyPeerPresenting(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo, ref bool shouldPresent) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnPeerDiscoveryNearbyPeerPresenting, peer: \"{0}\", discovery info: \r\n{1}", peerId, FormatDiscoveryInfo(peerDiscoveryInfo));
            }
        }

        private void OnPeerDiscoveryCancelled() {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnPeerDiscoveryCancelled");
            }

            StopAll();
        }

        private void OnPeerDiscoveryFinished() {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnPeerDiscoveryFinished");
            }
        }

        #endregion

        #region CustomServiceAdvertiser

        private void OnCustomServiceAdvertiserInvitationReceived(PeerId invitingPeerId, AppleLocalMultiplayer.InvitationHandler invitationHandler) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnCustomServiceAdvertiserInvitationReceived, inviting peer: \"{0}\"", invitingPeerId);
            }
        }

        private void OnCustomServiceAdvertiserStartFailed(string error) {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnCustomServiceAdvertiserStartFailed, error: \"{0}\"", error);
            }
        }

        #endregion

        #region ServiceAdvertiser

        private void OnServiceAdvertiserInvitationDismissed() {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnServiceAdvertiserInvitationDismissed");
            }
        }

        private void OnServiceAdvertiserInvitationPresenting() {
            if (_multipeerConnectivitySettings.LogEvents) {
                Debug.LogFormat("Event - OnServiceAdvertiserInvitationPresenting");
            }
        }

        #endregion

        #endregion

#endif

        private static string FormatDiscoveryInfo(IDictionary<string, string> discoveryInfo) {
            if (discoveryInfo == null)
                return "none";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in discoveryInfo) {
                sb.AppendFormat("key: \"{0}\", value: \"{1}\"\n", pair.Key, pair.Value);
            }

            return sb.ToString();
        }

#if UNITY_EDITOR
        protected virtual void Reset() {
            OnValidate();
        }

        protected virtual void OnValidate() {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
            _multipeerConnectivitySettings.ServiceType =
                AppleLocalMultiplayer.Utility.StringToValidServiceTypeName(_multipeerConnectivitySettings.ServiceType);
#endif
        }
#endif

        /// <summary>
        /// Container for Local Multiplayer settings.
        /// </summary>
        /// <tocexclude />
        [Serializable]
        public class LocalMultiplayerNetworkManagerSettings {
            [Tooltip("Indicates whether Local Multiplayer events should be logged.")]
            [SerializeField]
            private bool _logEvents;

            [Tooltip(
                "Text identifier of the service. Effectively, the name of the \"room\". " +
                "Must be the same for all peers who want to join the session.\n" +
                "Can be up to 15 characters long, valid characters include ASCII lowercase letters, numbers, and the hyphen.")]
            [SerializeField]
            private string _serviceType = "mygame";

            public string ServiceType {
                get { return _serviceType; }
                set {
                    if (value == null)
                        throw new ArgumentNullException("value");

#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
                    _serviceType = AppleLocalMultiplayer.Utility.StringToValidServiceTypeName(value);
#else
                    _serviceType = value;
#endif
                }
            }

            public bool LogEvents {
                get { return _logEvents; }
                set { _logEvents = value; }
            }
        }

        private enum MultiplayerMode {
            None,
            Host,
            Client
        }
    }
}
