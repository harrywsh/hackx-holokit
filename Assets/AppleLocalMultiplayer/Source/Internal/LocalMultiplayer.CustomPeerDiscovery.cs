#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using System.Collections.Generic;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <tocexclude />
        public delegate void CustomPeerDiscoveryPeerFoundHandler(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo);

        /// <tocexclude />
        public delegate void CustomPeerDiscoveryPeerLostHandler(PeerId peerId);

        /// <tocexclude />
        public delegate void CustomPeerDiscoveryStartFailedHandler(string error);

        /// <summary>
        /// Provides full programmatic access to the peer discovery API. 
        /// Allows implementing complex custom behaviours (like auto-joining the session or 
        /// implementing a in-game peer discovery interface). 
        /// </summary>
        public interface ICustomPeerDiscovery {
            /// <summary>
            /// Occurs when a new peer is found.
            /// </summary>
            event CustomPeerDiscoveryPeerFoundHandler PeerFound;

            /// <summary>
            /// Occurs when a known peer is lost.
            /// </summary>
            event CustomPeerDiscoveryPeerLostHandler PeerLost;

            /// <summary>
            /// Occurs when starting the discovery has failed.
            /// </summary>
            event CustomPeerDiscoveryStartFailedHandler StartFailed;

            /// <summary>
            /// Starts discovery of nearby peers.
            /// </summary>
            void StartDiscovery();

            /// <summary>
            /// Stop discovery of nearby peers.
            /// </summary>
            bool StopDiscovery();

            /// <summary>
            /// Invites peer <paramref name="peerId" /> to the current session.
            /// </summary>
            /// <param name="peerId">Invited peer.</param>
            /// <param name="timeout">Maximum time for peer to accept the invitation.</param>
            void InvitePeer(PeerId peerId, float timeout = 30f);
        }

        private sealed class CustomPeerDiscoveryContainer : ICustomPeerDiscovery {
            public event CustomPeerDiscoveryPeerFoundHandler PeerFound;
            public event CustomPeerDiscoveryPeerLostHandler PeerLost;
            public event CustomPeerDiscoveryStartFailedHandler StartFailed;

            public void StartDiscovery() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.StartPeerDiscovery(out error);
                AssertNativeError(success, error);
#endif
            }

            public bool StopDiscovery() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.StopPeerDiscovery(out error);
                ReleaseNativeError(success, error);
                return success;
#else
                return false;
#endif
            }

            public void InvitePeer(PeerId peerId, float timeout = 30f) {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.InvitePeerToSession(peerId.NativePointer, timeout, out error);
                AssertNativeError(success, error);
#endif
            }

            public void OnPeerFound(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo) {
                if (PeerFound != null) PeerFound(peerId, peerDiscoveryInfo);
            }

            public void OnPeerLost(PeerId peerId) {
                if (PeerLost != null) PeerLost(peerId);
            }

            public void OnStartFailed(string error) {
                if (StartFailed != null) StartFailed(error);
            }
        }
    }
}

#endif
