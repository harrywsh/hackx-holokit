#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using System.Collections.Generic;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <tocexclude />
        public delegate void InvitationHandler(bool acceptInvitation);

        /// <tocexclude />
        public delegate void CustomServiceAdvertiserInvitationReceivedHandler(PeerId invitingPeerId, InvitationHandler invitationHandler);

        /// <tocexclude />
        public delegate void CustomServiceAdvertiserStartFailedHandler(string error);

        /// <summary>
        /// Provides full programmatic access to the service advertiser API. 
        /// Allows implementing complex custom behaviours (like auto-joining the session or 
        /// implementing a in-game invitation accept/decline interface). 
        /// </summary>
        public interface ICustomServiceAdvertiser {
            /// <summary>
            /// Occurs when an invitation to join the session has been received.
            /// </summary>
            event CustomServiceAdvertiserInvitationReceivedHandler InvitationReceived;

            /// <summary>
            /// Occurs when starting the advertiser has failed.
            /// </summary>
            event CustomServiceAdvertiserStartFailedHandler StartFailed;

            /// <summary>
            /// Whether advertiser is currently active.
            /// </summary>
            bool IsAdvertising { get; }

            /// <summary>
            /// Start advertising the current service to nearby peers.
            /// </summary>
            /// <param name="discoveryInfo">
            /// Additional information that will be advertised to the peer browsers.
            /// Keep the size of this dictionary small for best network performance.
            /// </param>
            /// <returns>true on success, false if already advertising.</returns>
            void StartAdvertising(IDictionary<string, string> discoveryInfo = null);

            /// <summary>
            /// Stops advertising of the current service to nearby peers.
            /// </summary>
            /// <returns>true on success, false on error.</returns>
            bool StopAdvertising();
        }

        private sealed class CustomServiceAdvertiserContainer : ICustomServiceAdvertiser {
            public event CustomServiceAdvertiserInvitationReceivedHandler InvitationReceived;
            public event CustomServiceAdvertiserStartFailedHandler StartFailed;

            public bool IsAdvertising {
                get {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                    return NativeMethods.UMCMediatorFacade.IsAdvertiserAdvertising();
#else
                    return false;
#endif
                }
            }

            public void StartAdvertising(IDictionary<string, string> discoveryInfo = null) {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                NativeMethods.UMC_StringStringKeyValuePair[] keyValuePairs = null;
                int keyValuePairCount = 0;
                if (discoveryInfo != null) {
                    keyValuePairs = MarshalUtility.StringStringDictionaryToPairArray(discoveryInfo);
                    keyValuePairCount = keyValuePairs.Length;
                }
                IntPtr error;
                bool success =
                    NativeMethods.UMCMediatorFacade.StartAdvertiserWithDiscoveryInfo(keyValuePairs, keyValuePairCount, out error);
                AssertNativeError(success, error);
#endif
            }

            public bool StopAdvertising() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.StopAdvertiser(out error);
                ReleaseNativeError(success, error);
                return success;
#else
                return false;
#endif
            }

            public void OnInvitationReceived(PeerId invitingPeerId, InvitationHandler invitationHandler) {
                if (InvitationReceived != null) InvitationReceived(invitingPeerId, invitationHandler);
            }

            public void OnStartFailed(string error) {
                if (StartFailed != null) StartFailed(error);
            }
        }
    }
}

#endif
