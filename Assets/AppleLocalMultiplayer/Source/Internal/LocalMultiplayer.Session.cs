#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using System.Text;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <tocexclude />
        public delegate void SessionPeerStateChangedHandler(PeerId peerId, PeerState newPeerState);

        /// <tocexclude />
        public delegate void SessionStartedHandler();

        /// <tocexclude />
        public delegate void SessionStoppedHandler();

        /// <summary>
        /// Provides access to the local multiplayer session, setting the peer name,
        /// service type, getting other peers that are connected to the session.
        /// </summary>
        public interface ISession {
            /// <summary>
            /// Occurs when the connection state of a peer has changed.
            /// </summary>
            event SessionPeerStateChangedHandler PeerStateChanged;

            /// <summary>
            /// Occurs when session has been started.
            /// </summary>
            event SessionStartedHandler Started;

            /// <summary>
            /// Occurs when session has been stopped.
            /// </summary>
            event SessionStoppedHandler Stopped;

            /// <summary>
            /// Whether session is currently active.
            /// </summary>
            bool IsSessionActive { get; }

            /// <summary>
            /// Sets the service type identifier.
            /// </summary>
            /// <param name="serviceType">
            /// Text identifier of the service. Effectively, the name of the "room".
            /// Must be the same for all peers who want to join the session.
            /// Can be up to 15 characters long,
            /// valid characters are ASCII lowercase letters, numbers, and the hyphen.</param>
            /// <returns>true on success, false on error.</returns>
            void SetServiceType(string serviceType);

            /// <summary>
            /// Sets the name of the local peer.
            /// </summary>
            /// <param name="localPeerName">
            /// Name of the local peer that will be displayed to peer browsers.
            /// Can be up to 63 bytes in UTF8 encoding.
            /// </param>
            /// <returns>true on success, false on error.</returns>
            void SetLocalPeerName(string localPeerName);

            /// <summary>
            /// Sets the server port. Must be called before starting the networking server.
            /// </summary>
            /// <param name="port">Server port number. Must be the same as used by the networking server.</param>
            /// <returns>true on success, false on error.</returns>
            void SetServerPort(int port);

            /// <summary>
            /// Starts the session.
            /// </summary>
            /// <returns>true on success, false on error.</returns>
            void StartSession();

            /// <summary>
            /// Disconnects current device from the session, stops all service advertisers and peer browsers.
            /// </summary>
            /// <returns>true on success, false if session is not started, or on error.</returns>
            bool StopSession();

            /// <summary>
            /// Gets the number of peers that are connected to the session.
            /// </summary>
            /// <returns>Number of peers that are connected to the session.</returns>
            int GetConnectedPeersCount();

            /// <summary>
            /// Gets the array of peers that are connected to the session.
            /// </summary>
            /// <returns>Array of peers that are connected to the session.</returns>
            PeerId[] GetConnectedPeers();

            /// <summary>
            /// Gets peers that are connected to the session, and stores them in the provided buffer.
            /// </summary>
            /// <param name="results">The buffer to store the results in.</param>
            /// <returns>The amount of peers stored to the <paramref name="results"/> buffer.</returns>
            int GetConnectedPeers(PeerId[] results);
        }

        private sealed class SessionContainer : ISession {
            private static readonly PeerId[] kEmptyPeerIdArray = new PeerId[0];

            public event SessionPeerStateChangedHandler PeerStateChanged;
            public event SessionStartedHandler Started;
            public event SessionStoppedHandler Stopped;

            public bool IsSessionActive {
                get {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                    return NativeMethods.UMCMediatorFacade.IsSessionActive();
#else
                    return false;
#endif
                }
            }

            public void SetServiceType(string serviceType) {
                if (String.IsNullOrEmpty(serviceType))
                    throw new ArgumentNullException("serviceType");

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                serviceType = Utility.StringToValidServiceTypeName(serviceType);

                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.SetServiceType(serviceType, out error);
                AssertNativeError(success, error);
#endif
            }

            public void SetLocalPeerName(string localPeerName) {
                if (String.IsNullOrEmpty(localPeerName))
                    throw new ArgumentNullException("localPeerName");

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                localPeerName = Utility.TruncateStringToEncodingBytes(localPeerName, 63, Encoding.UTF8);

                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.SetDisplayName(localPeerName, out error);
                AssertNativeError(success, error);
#endif
            }

            public void SetServerPort(int port) {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.SetDstHostAndDstPort("127.0.0.1", (ushort) port, out error);
                AssertNativeError(success, error);
#endif
            }

            public void StartSession() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.StartSession(out error);
                AssertNativeError(success, error);
#endif
            }

            public bool StopSession() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.DisconnectFromSession(out error);
                ReleaseNativeError(success, error);
                return success;
#else
                return false;
#endif
            }

            public int GetConnectedPeersCount() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                int nativePeerCount;
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.GetConnectedPeersCount(out nativePeerCount, out error);
                ReleaseNativeError(success, error);
                return nativePeerCount;
#else
                return 0;
#endif
            }

            public int GetConnectedPeers(PeerId[] results) {
                if (results == null)
                    throw new ArgumentNullException("results");

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr nativePeersArrayPtr = IntPtr.Zero;
                try {
                    int nativePeerCount;
                    IntPtr error;
                    bool success =
                        NativeMethods.UMCMediatorFacade.GetConnectedPeers(
                            results.Length,
                            out nativePeersArrayPtr,
                            out nativePeerCount,
                            out error);
                    AssertNativeError(success, error);

                    if (nativePeerCount == 0)
                        return 0;

                    MarshalUtility.MarshalNativeArray(nativePeersArrayPtr, nativePeerCount, results, ptr => new PeerId(ptr));
                    return nativePeerCount;
                } finally {
                    NativeMethods.free(nativePeersArrayPtr);
                }
#else
                return 0;
#endif
            }

            public PeerId[] GetConnectedPeers() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE

                IntPtr nativePeersArrayPtr = IntPtr.Zero;
                try {
                    int nativePeerCount;
                    IntPtr error;
                    bool success =
                        NativeMethods.UMCMediatorFacade.GetConnectedPeers(
                            -1,
                            out nativePeersArrayPtr,
                            out nativePeerCount,
                            out error);
                    AssertNativeError(success, error);

                    if (nativePeerCount == 0)
                        return kEmptyPeerIdArray;

                    PeerId[] peers = MarshalUtility.MarshalNativeArray(nativePeersArrayPtr, nativePeerCount, ptr => new PeerId(ptr));
                    return peers;
                } finally {
                    NativeMethods.free(nativePeersArrayPtr);
                }
#else
                return kEmptyPeerIdArray;
#endif
            }

            public void OnPeerStateChanged(PeerId peerId, PeerState newPeerState) {
                if (PeerStateChanged != null) PeerStateChanged(peerId, newPeerState);
            }

            public void OnStarted() {
                if (Started != null) Started();
            }

            public void OnStopped() {
                if (Stopped != null) Stopped();
            }
        }
    }
}

#endif
