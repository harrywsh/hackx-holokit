using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
using LostPolygon.Apple.LocalMultiplayer.Networking;
#endif

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    public class PeerBrowserController : MonoBehaviour
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        , ICustomDeviceBrowser
#endif
    {
        public GameObject PeerBrowserPanelGameObject;
        public GameObject PeerBrowserContentPanelGameObject;
        public GameObject PeerBrowserPeerButtonPrefab;

#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
        private readonly Dictionary<PeerId, PeerBrowserPeerButtonData> _peerBrowserButtons = new Dictionary<PeerId, PeerBrowserPeerButtonData>();
        private readonly Dictionary<PeerId, PeerState> _peerState = new Dictionary<PeerId, PeerState>();

        public void Clear() {
            // Remove all existing peer buttons
            foreach (PeerBrowserPeerButtonData peerBrowserButton in _peerBrowserButtons.Values.ToArray()) {
                DestroyPeerBrowserButton(peerBrowserButton);
            }

            _peerBrowserButtons.Clear();
            // Clear any known information about peers
            _peerState.Clear();
        }

        private void OnEnable() {
            AppleLocalMultiplayer.Session.PeerStateChanged += OnSessionPeerStateChanged;

            AppleLocalMultiplayer.CustomPeerDiscovery.PeerFound += OnCustomPeerDiscoveryPeerFound;
            AppleLocalMultiplayer.CustomPeerDiscovery.PeerLost += OnCustomPeerDiscoveryPeerLost;
        }

        private void OnDisable() {
            AppleLocalMultiplayer.Session.PeerStateChanged -= OnSessionPeerStateChanged;

            AppleLocalMultiplayer.CustomPeerDiscovery.PeerFound -= OnCustomPeerDiscoveryPeerFound;
            AppleLocalMultiplayer.CustomPeerDiscovery.PeerLost -= OnCustomPeerDiscoveryPeerLost;
        }

        private PeerState GetPeerState(PeerId peerId) {
            // Return known state if we already know this peer, otherwise assume it is not connected
            return _peerState.ContainsKey(peerId) ? _peerState[peerId] : PeerState.NotConnected;
        }

        private void UpdatePeerBrowserButton(PeerId peerId, bool exitIfNoButton) {
            PeerBrowserPeerButtonData peerBrowserPeerButtonData;
            GameObject peerBrowserButtonGameObject;
            // Try to get a button that corresponds to the peer
            if (!_peerBrowserButtons.TryGetValue(peerId, out peerBrowserPeerButtonData)) {
                // No button exists, and we don't need one, just exit
                if (exitIfNoButton)
                    return;

                // If no button exists, create one
                peerBrowserButtonGameObject = Instantiate(PeerBrowserPeerButtonPrefab);
                peerBrowserPeerButtonData = peerBrowserButtonGameObject.GetComponent<PeerBrowserPeerButtonData>();

                // Make a connection between the peer and its button
                _peerBrowserButtons.Add(peerId, peerBrowserPeerButtonData);

                peerBrowserButtonGameObject.transform.SetParent(PeerBrowserContentPanelGameObject.transform, false);
            }

            peerBrowserButtonGameObject = peerBrowserPeerButtonData.gameObject;
            // Update peer name text
            peerBrowserPeerButtonData.PeerNameText.text = peerId.Name;
            PeerState peerState = GetPeerState(peerId);
            // Update peer state text
            peerBrowserPeerButtonData.PeerStateText.text = GetPeerStateText(peerState);

            // Attach an event handler, unless already attached one
            if (!peerBrowserPeerButtonData.IsButtonRegistered) {
                peerBrowserPeerButtonData.Button.onClick.AddListener(() => {
                    if (OnPeerPicked != null)
                        OnPeerPicked(peerId);
                });
                peerBrowserPeerButtonData.IsButtonRegistered = true;
            }

            // There is no point in trying to connect to an already connected peer
            peerBrowserPeerButtonData.Button.interactable = peerState != PeerState.Connected;
            // Make sure connected peers are at the top of the list
            if (peerState == PeerState.Connected) {
                peerBrowserButtonGameObject.transform.SetAsFirstSibling();
            }
        }

        private void OnSessionPeerStateChanged(PeerId peerId, PeerState newPeerState) {
            _peerState[peerId] = newPeerState;
            // Only update the button if we have found he peer first, ignore otherwise
            UpdatePeerBrowserButton(peerId, true);
        }

        private void OnCustomPeerDiscoveryPeerFound(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo) {
            UpdatePeerBrowserButton(peerId, false);
        }

        private void OnCustomPeerDiscoveryPeerLost(PeerId peerId) {
            // Remove peer from the list
            PeerBrowserPeerButtonData peerBrowserPeerButtonData;
            if (_peerBrowserButtons.TryGetValue(peerId, out peerBrowserPeerButtonData)) {
                DestroyPeerBrowserButton(peerBrowserPeerButtonData);
                _peerBrowserButtons.Remove(peerId);
            }
        }

        private static void DestroyPeerBrowserButton(PeerBrowserPeerButtonData peerBrowserPeerButtonData) {
            peerBrowserPeerButtonData.Button.onClick.RemoveAllListeners();
            Destroy(peerBrowserPeerButtonData.gameObject);
        }

        private static string GetPeerStateText(PeerState peerState) {
            switch (peerState) {
                case PeerState.NotConnected:
                    return "Not Connected";
                case PeerState.Connecting:
                    return "Connecting";
                case PeerState.Connected:
                    return "Connected";
                default:
                    throw new ArgumentOutOfRangeException("peerState", peerState, null);
            }
        }

        #region ICustomDeviceBrowser

#pragma warning disable 67
        public event Action OnOpened;
        public event Action OnClosing;
        public event Action<PeerId> OnPeerPicked;
#pragma warning restore 67

        public void Open() {
            // Open the peer browser panel
            PeerBrowserPanelGameObject.SetActive(true);

            // Start peer discovery
            AppleLocalMultiplayer.CustomPeerDiscovery.StartDiscovery();

            // Fill the peer list with peers that are already connected
            PeerId[] connectedPeers = AppleLocalMultiplayer.Session.GetConnectedPeers();
            foreach (PeerId connectedPeer in connectedPeers) {
                _peerState[connectedPeer] = PeerState.Connected;
                UpdatePeerBrowserButton(connectedPeer, false);
            }

            if (OnOpened != null)
                OnOpened();
        }

        public void Close() {
            AppleLocalMultiplayer.CustomPeerDiscovery.StopDiscovery();
            Clear();

            if (OnClosing != null)
                OnClosing();

            PeerBrowserPanelGameObject.SetActive(false);
        }

        #endregion
#endif
    }
}
