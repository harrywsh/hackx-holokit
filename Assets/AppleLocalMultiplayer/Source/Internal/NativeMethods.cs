#if (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && (!UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE)

using System;
using System.Runtime.InteropServices;

namespace LostPolygon.Apple.LocalMultiplayer.Internal {
    /// <summary>
    /// Contains all the native Objective-C methods that are called from Unity.
    /// </summary>
    internal static class NativeMethods {
        private const string kLibraryName =
#if !UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS)
            "__Internal";
#else
            "UnityMultipeerConnectivity";
#endif

        [DllImport(kLibraryName, EntryPoint = "UMC_free")]
        public static extern void free(IntPtr ptr);

#if !UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS)
        [DllImport(kLibraryName, EntryPoint = "UMC_GetViewController")]
        public static extern IntPtr GetViewController();
#else
        [DllImport(kLibraryName, EntryPoint = "UMC_GetMainWindow")]
        public static extern IntPtr GetMainWindow();
#endif

        public static class UMCMediatorFacade {
            #region Setup

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_SetDstHostAndDstPort")]
            public static extern bool SetDstHostAndDstPort(string dstHost, ushort dstPort, out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_SetServiceType")]
            public static extern bool SetServiceType(string serviceType, out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_SetDisplayName")]
            public static extern bool SetDisplayName(string displayName, out IntPtr error);

#if !UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS)
            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_SetViewController")]
            public static extern bool SetViewController(IntPtr viewController, out IntPtr error);
#else
            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_SetMainWindow")]
            public static extern bool SetMainWindow(IntPtr mainWindow, out IntPtr error);
#endif

            #endregion

            #region Session

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StartSession")]
            public static extern bool StartSession(out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_DisconnectFromSession")]
            public static extern bool DisconnectFromSession(out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_IsSessionActive")]
            public static extern bool IsSessionActive();

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_InvitePeerToSession")]
            public static extern bool InvitePeerToSession(IntPtr peerId, double timeout, out IntPtr error);

            #endregion

            #region Connectivity

#if !(UNITY_STANDALONE_OSX || UNITY_TVOS)
            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_IsBluetoothEnabled")]
            public static extern bool IsBluetoothEnabled();

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_OpenBluetoothEnablePrompt")]
            public static extern bool OpenBluetoothEnablePrompt(out IntPtr error);
#endif

            #endregion

            #region Advertiser

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StartAdvertiserWithDiscoveryInfo")]
            public static extern bool StartAdvertiserWithDiscoveryInfo(
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] UMC_StringStringKeyValuePair[] discoveryInfoKeyValuePairs,
                int discoveryInfoKeyValuePairCount,
                out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StopAdvertiser")]
            public static extern bool StopAdvertiser(out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_IsAdvertiserAdvertising")]
            public static extern bool IsAdvertiserAdvertising();

            #endregion

            #region Advertiser assistant

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StartAdvertiserAssistantWithDiscoveryInfo")]
            public static extern bool StartAdvertiserAssistantWithDiscoveryInfo(
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] UMC_StringStringKeyValuePair[] discoveryInfoKeyValuePairs,
                int discoveryInfoKeyValuePairCount,
                out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StopAdvertiserAssistant")]
            public static extern bool StopAdvertiserAssistant(out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_IsAdvertiserAssistantAdvertising")]
            public static extern bool IsAdvertiserAssistantAdvertising();

            #endregion

            #region Peer discovery

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StartPeerDiscovery")]
            public static extern bool StartPeerDiscovery(out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_StopPeerDiscovery")]
            public static extern bool StopPeerDiscovery(out IntPtr error);

            #endregion

            #region Peer browser

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_OpenPeerBrowser")]
            public static extern bool OpenPeerBrowser(uint minimumNumberOfPeers, uint maximumNumberOfPeers, out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_ClosePeerBrowser")]
            public static extern bool ClosePeerBrowser(out IntPtr error);

            #endregion

            #region Other methods

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_GetConnectedPeers")]
            public static extern bool GetConnectedPeers(
                int maxConnectedPeersCount,
                out IntPtr connectedPeersArrayOut,
                out int connectedPeersCountOut,
                out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_GetConnectedPeersCount")]
            public static extern bool GetConnectedPeersCount(
                out int connectedPeersCountOut,
                out IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCMediatorFacade_SetLogForwarding")]
            public static extern void SetLogForwarding(bool isEnabled);

            #endregion
        }

        public static class UMCLog {
            [DllImport(kLibraryName, EntryPoint = "UMC_UMCLog_GetIsVerboseLog")]
            public static extern bool GetIsVerboseLog();

            [DllImport(kLibraryName, EntryPoint = "UMC_UMCLog_SetIsVerboseLog")]
            public static extern void SetIsVerboseLog(bool isVerbose);
        }

        public static class NSObject {
            [DllImport(kLibraryName, EntryPoint = "UMC_NSObject_isEquals")]
            public static extern bool isEquals(IntPtr objA, IntPtr objB);

            [DllImport(kLibraryName, EntryPoint = "UMC_NSObject_hash")]
            public static extern ulong hash(IntPtr obj);

            [DllImport(kLibraryName, EntryPoint = "UMC_NSObject_retain")]
            public static extern void retain(IntPtr ptr);

            [DllImport(kLibraryName, EntryPoint = "UMC_NSObject_release")]
            public static extern void release(IntPtr ptr);

            [DllImport(kLibraryName, EntryPoint = "UMC_NSObject_retainCount")]
            public static extern ulong retainCount(IntPtr obj);
        }

        public static class MCPeerID {
            [DllImport(kLibraryName, EntryPoint = "UMC_MCPeerID_displayName")]
            public static extern string displayName(IntPtr peerId);
        }

        public static class NSError {
            [DllImport(kLibraryName, EntryPoint = "UMC_NSError_code")]
            public static extern int code(IntPtr error);

            [DllImport(kLibraryName, EntryPoint = "UMC_NSError_localizedDescription")]
            public static extern string localizedDescription(IntPtr error);
        }

        public static class AdvertiserInvitationHandlerBlock {
            [DllImport(kLibraryName, EntryPoint = "UMC_AdvertiserInvitationHandlerBlock_Invoke")]
            public static extern void Invoke(IntPtr block, bool accept, IntPtr session);
        }

        public static class Events {
            public delegate void UnityEventHandlerFunc(EventType eventType, IntPtr eventData);

            //Internal
            [DllImport(kLibraryName, EntryPoint = "UMC_UMCUnityEvents_SetEventListener")]
            public static extern void UMC_UMCUnityEvents_SetEventListener(UnityEventHandlerFunc eventHandlerFunc);

            [DllImport(kLibraryName, EntryPoint = "UMC_FreeEventData")]
            public static extern void UMC_FreeEventData(EventType eventType, IntPtr eventData);

            public enum EventType : int {
                // Session
                SessionStarted,
                SessionDisconnected,
                SessionPeerStateChanged,

                // Advertiser
                AdvertiserInvitationReceived,
                AdvertiserStartFailed,

                // Advertiser assistant
                AdvertiserAssistantInvitationDismissed,
                AdvertiserAssistantInvitationPresenting,

                // Custom peer discovery
                NearbyServiceBrowserPeerFound,
                NearbyServiceBrowserPeerLost,
                NearbyServiceBrowserStartFailed,

                // Peer discovery UI
                BrowserViewControllerCancelled,
                BrowserViewControllerFinished,
                BrowserViewControllerNearbyPeerPresenting,

                // Logs
                Log
            }

            // Common
            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_ErrorStringEventData {
                public readonly string Error;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_PeerFoundEventData {
                public readonly IntPtr PeerId;
                public readonly IntPtr DiscoveryInfoPairArray;
                public readonly int DiscoveryInfoArrayPairCount;
            }

            // Session
            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_SessionPeerStateChangedEventData {
                public readonly IntPtr PeerId;
                public readonly PeerState NewPeerState;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_AdvertiserInvitationReceivedEventData {
                public readonly IntPtr PeerId;
                public readonly IntPtr InvitationHandler;
            }

            // Custom peer discovery
            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_NearbyServiceBrowserPeerLostEventData {
                public readonly IntPtr PeerId;
            }

            // Peer discovery UI
            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_BrowserViewControllerNearbyPeerPresentingResultEventData {
                public bool ShouldPresent;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_BrowserViewControllerNearbyPeerPresentingEventData {
                public readonly IntPtr PeerId;
                public readonly IntPtr DiscoveryInfoPairArray;
                public readonly int DiscoveryInfoArrayPairCount;

                public readonly /* UMC_BrowserViewControllerNearbyPeerPresentingResultEventData */ IntPtr Result;
            }

            // Logs
            [StructLayout(LayoutKind.Sequential)]
            public struct UMC_LogEventData {
                public readonly LogType Type;
                public readonly string Text;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UMC_StringStringKeyValuePair {
            public string Key;
            public string Value;
        }

        public enum LogType {
            Log,
            Warning,
            Error
        }

        public enum ErrorType : int {
            None,
            Fatal,
            NotSupported,
            SessionActive,
            SessionNotActive,
            InvalidState,
            InvalidInput
        }
    }
}

#endif
