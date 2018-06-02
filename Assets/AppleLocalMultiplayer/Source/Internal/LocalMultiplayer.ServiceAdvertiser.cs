#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using System.Collections.Generic;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <tocexclude />
        public delegate void ServiceAdvertiserInvitationDismissedHandler();

        /// <tocexclude />
        public delegate void ServiceAdvertiserInvitationPresentingHandler();

        /// <summary>
        /// Access to the native built-in service advertiser. 
        /// Whenever another peer sends an invitation, present a dialog that allows declining or accepting the invitation. 
        /// Very easy to use and setup, but limited to a single interaction model.
        /// </summary>
        public interface IServiceAdvertiser {
            /// <summary>
            /// Occurs when user taps "Dismiss" or "Accept" in the incoming invitation UI.
            /// </summary>
            event ServiceAdvertiserInvitationDismissedHandler InvitationDismissed;

            /// <summary>
            /// Occurs when incoming invitation UI is about to be shown.
            /// </summary>
            event ServiceAdvertiserInvitationPresentingHandler InvitationPresenting;

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
            /// <returns>true on success, false on error.</returns>
            void StartAdvertising(IDictionary<string, string> discoveryInfo = null);

            /// <summary>
            /// Stops advertising of the current service to nearby peers.
            /// </summary>
            /// <returns>true on success, false on error.</returns>
            bool StopAdvertising();
        }

        private sealed class ServiceAdvertiserContainer : IServiceAdvertiser {
            public event ServiceAdvertiserInvitationDismissedHandler InvitationDismissed;
            public event ServiceAdvertiserInvitationPresentingHandler InvitationPresenting;

            public bool IsAdvertising {
                get {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                    return NativeMethods.UMCMediatorFacade.IsAdvertiserAssistantAdvertising();
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
                    NativeMethods.UMCMediatorFacade.StartAdvertiserAssistantWithDiscoveryInfo(keyValuePairs, keyValuePairCount, out error);
                AssertNativeError(success, error);
#endif
            }

            public bool StopAdvertising() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.StopAdvertiserAssistant(out error);
                ReleaseNativeError(success, error);
                return success;
#else
                return false;
#endif
            }

            public void OnInvitationDismissed() {
                if (InvitationDismissed != null) InvitationDismissed();
            }

            public void OnInvitationPresenting() {
                if (InvitationPresenting != null) InvitationPresenting();
            }
        }
    }
}

#endif
