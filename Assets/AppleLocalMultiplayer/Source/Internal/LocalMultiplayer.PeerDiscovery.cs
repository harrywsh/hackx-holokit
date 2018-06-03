#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using System.Collections.Generic;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <tocexclude />
        public delegate void PeerDiscoveryCancelledHandler();

        /// <tocexclude />
        public delegate void PeerDiscoveryFinishedHandler();

        /// <tocexclude />
        public delegate void PeerDiscoveryNearbyPeerPresentingHandler(PeerId peerId, IDictionary<string, string> peerDiscoveryInfo, ref bool shouldPresent);

        /// <summary>
        /// Access to the native built-in service advertiser. 
        /// Whenever another peer sends an invitation, present a dialog that allows declining or accepting the invitation. 
        /// Very easy to use and setup, but limited to a single interaction model.
        /// </summary>
        public interface IPeerDiscovery {
            /// <summary>
            /// Occurs when "Cancel" button is pressed in the peer browser UI.
            /// </summary>
            event PeerDiscoveryCancelledHandler Cancelled;

            /// <summary>
            /// Occurs when "Finish" button is pressed in the peer browser UI.
            /// </summary>
            event PeerDiscoveryFinishedHandler Finished;

            /// <summary>
            /// Occurs when a peer is found and is about to be presented in the peer browser UI.
            /// Note: this event is called on a background thread, which means that you can't access Unity APIs
            /// from within the handler of this event.
            /// </summary>
            event PeerDiscoveryNearbyPeerPresentingHandler NearbyPeerPresenting;

            /// <summary>
            /// Opens the peer browser UI.
            /// </summary>
            /// <returns>true on success, false on error.</returns>
            void OpenBrowser(int minimumNumberOfPeers = 2, int maximumNumberOfPeers = 8);

            /// <summary>
            /// Closes the peer browser UI.
            /// </summary>
            /// <returns>true on success, false on error.</returns>
            bool CloseBrowser();
        }

        private sealed class PeerDiscoveryContainer : IPeerDiscovery {
            private const int kMinimumNumberOfPeers = 2;
            private const int kMaximumNumberOfPeers = 8;

            public event PeerDiscoveryCancelledHandler Cancelled;
            public event PeerDiscoveryFinishedHandler Finished;
            public event PeerDiscoveryNearbyPeerPresentingHandler NearbyPeerPresenting;

            public void OpenBrowser(int minimumNumberOfPeers = 2, int maximumNumberOfPeers = 8) {
                if (minimumNumberOfPeers > maximumNumberOfPeers)
                    throw new ArgumentOutOfRangeException("minimumNumberOfPeers can't be larger than maximumNumberOfPeers");

                if (minimumNumberOfPeers < kMinimumNumberOfPeers || minimumNumberOfPeers > kMaximumNumberOfPeers)
                    throw new ArgumentOutOfRangeException(
                        "minimumNumberOfPeers",
                        String.Format("minimumNumberOfPeers must be in range of [{0}..{1}]", kMinimumNumberOfPeers, kMaximumNumberOfPeers));

                if (maximumNumberOfPeers < kMinimumNumberOfPeers || maximumNumberOfPeers > kMaximumNumberOfPeers)
                    throw new ArgumentOutOfRangeException(
                        "maximumNumberOfPeers",
                        String.Format("maximumNumberOfPeers must be in range of [{0}..{1}]", kMinimumNumberOfPeers, kMaximumNumberOfPeers));

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success =
                    NativeMethods.UMCMediatorFacade.OpenPeerBrowser((uint) minimumNumberOfPeers, (uint) maximumNumberOfPeers, out error);
                AssertNativeError(success, error);
#endif
            }

            public bool CloseBrowser() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.ClosePeerBrowser(out error);
                ReleaseNativeError(success, error);
                return success;
#else
                return false;
#endif
            }

            public void OnNearbyPeerPresenting(PeerId peerId, IDictionary<string, string> peerDiscovery, ref bool shouldPresent) {
                if (NearbyPeerPresenting != null) NearbyPeerPresenting(peerId, peerDiscovery, ref shouldPresent);
            }

            public void OnCancelled() {
                if (Cancelled != null) Cancelled();
            }

            public void OnFinished() {
                if (Finished != null) Finished();
            }
        }
    }
}

#endif
