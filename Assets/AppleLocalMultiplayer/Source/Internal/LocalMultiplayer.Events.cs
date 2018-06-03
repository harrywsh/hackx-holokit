#if (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && (!UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        private static readonly Stack<NativeEvent> _nativeEvents = new Stack<NativeEvent>();

        private static void RegisterEventHandlers() {
            UnityMessagesBroadcaster.GenericMessageEntered += DispatchDeferredEvents;

            NativeMethods.Events.UMC_UMCUnityEvents_SetEventListener(MessageHandler);
        }

        /// <summary>
        /// Dispatches the recorded events.
        /// </summary>
        private static void DispatchDeferredEvents() {
            lock (_nativeEvents) {
                while (_nativeEvents.Count > 0) {
                    NativeEvent nativeEvent = _nativeEvents.Pop();
                    try {
                        DispatchNativeEvent(ref nativeEvent);
                    } finally {
                        if (nativeEvent.EventData != IntPtr.Zero) {
                            NativeMethods.Events.UMC_FreeEventData(nativeEvent.EventType, nativeEvent.EventData);
                        }
                    }
                }
            }
        }

        private static void DispatchNativeEvent(ref NativeEvent nativeEvent) {
            switch (nativeEvent.EventType) {
                // Session
                case NativeMethods.Events.EventType.SessionStarted:
                    _session.OnStarted();
                    break;
                case NativeMethods.Events.EventType.SessionDisconnected:
                    _session.OnStopped();
                    break;
                case NativeMethods.Events.EventType.SessionPeerStateChanged: {
                    var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_SessionPeerStateChangedEventData>(nativeEvent.EventData);

                    PeerId peerId = new PeerId(eventData.PeerId);
                    _session.OnPeerStateChanged(peerId, eventData.NewPeerState);
                    break;
                }

                // Advertiser
                case NativeMethods.Events.EventType.AdvertiserInvitationReceived: {
                    var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_AdvertiserInvitationReceivedEventData>(nativeEvent.EventData);

                    PeerId peerId = new PeerId(eventData.PeerId);

                    // The block is wrapped into an NSObject wrapper so that it can be GC'd and eventually released
                    NSObject invitationHandlerBlock = new NSObject(eventData.InvitationHandler);
                    InvitationHandler invitationHandlerAction = accept => {
                        NativeMethods.AdvertiserInvitationHandlerBlock.Invoke(invitationHandlerBlock.NativePointer, accept, IntPtr.Zero);
                    };

                    _customServiceAdvertiser.OnInvitationReceived(peerId, invitationHandlerAction);
                    break;
                }
                case NativeMethods.Events.EventType.AdvertiserStartFailed: {
                    var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_ErrorStringEventData>(nativeEvent.EventData);

                    _customServiceAdvertiser.OnStartFailed(eventData.Error);
                    break;
                }

                // Advertiser assistant
                case NativeMethods.Events.EventType.AdvertiserAssistantInvitationDismissed:
                    _serviceAdvertiser.OnInvitationDismissed();
                    break;
                case NativeMethods.Events.EventType.AdvertiserAssistantInvitationPresenting:
                    _serviceAdvertiser.OnInvitationPresenting();
                    break;

                // Peer discovery
                case NativeMethods.Events.EventType.NearbyServiceBrowserPeerFound: {
                    var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_PeerFoundEventData>(nativeEvent.EventData);
                    PeerId peerId = new PeerId(eventData.PeerId);
                    Dictionary<string, string> discoveryInfo =
                        eventData.DiscoveryInfoPairArray != IntPtr.Zero ?
                            MarshalUtility.MarshalStringStringDictionary(eventData.DiscoveryInfoPairArray, eventData.DiscoveryInfoArrayPairCount) :
                            null;

                    _customPeerDiscovery.OnPeerFound(peerId, discoveryInfo);
                    break;
                }
                case NativeMethods.Events.EventType.NearbyServiceBrowserPeerLost: {
                    var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_NearbyServiceBrowserPeerLostEventData>(nativeEvent.EventData);
                    PeerId peerId = new PeerId(eventData.PeerId);

                    _customPeerDiscovery.OnPeerLost(peerId);
                    break;
                }
                case NativeMethods.Events.EventType.NearbyServiceBrowserStartFailed: {
                    var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_ErrorStringEventData>(nativeEvent.EventData);

                    _customPeerDiscovery.OnStartFailed(eventData.Error);
                    break;
                }

                // Peer discovery UI
                case NativeMethods.Events.EventType.BrowserViewControllerCancelled:
                    _peerDiscovery.OnCancelled();
                    break;
                case NativeMethods.Events.EventType.BrowserViewControllerFinished:
                    _peerDiscovery.OnFinished();
                    break;
                default:
                    throw new InvalidEnumArgumentException("nativeEvent.EventType", (int) nativeEvent.EventType, typeof(NativeMethods.Events.EventType));
            }
        }

        [MonoPInvokeCallback(typeof(NativeMethods.Events.UnityEventHandlerFunc))]
        private static void MessageHandler(NativeMethods.Events.EventType eventType, IntPtr eventDataPtr) {
            lock (_nativeEvents) {
                switch (eventType) {
                    // Session
                    case NativeMethods.Events.EventType.SessionStarted:
                    case NativeMethods.Events.EventType.SessionDisconnected:
                    case NativeMethods.Events.EventType.SessionPeerStateChanged:

                    // Advertiser
                    case NativeMethods.Events.EventType.AdvertiserInvitationReceived:
                    case NativeMethods.Events.EventType.AdvertiserStartFailed:

                    // Advertiser assistant
                    case NativeMethods.Events.EventType.AdvertiserAssistantInvitationDismissed:
                    case NativeMethods.Events.EventType.AdvertiserAssistantInvitationPresenting:

                    // Peer discovery
                    case NativeMethods.Events.EventType.NearbyServiceBrowserPeerFound:
                    case NativeMethods.Events.EventType.NearbyServiceBrowserPeerLost:
                    case NativeMethods.Events.EventType.NearbyServiceBrowserStartFailed:

                    // Peer discovery UI
                    case NativeMethods.Events.EventType.BrowserViewControllerCancelled:
                    case NativeMethods.Events.EventType.BrowserViewControllerFinished:
                        // Just store the event data, the event marshalling and invocation are deferred until we are on the main Unity thread
                        _nativeEvents.Push(new NativeEvent(eventType, eventDataPtr));
                        return;
                    case NativeMethods.Events.EventType.BrowserViewControllerNearbyPeerPresenting: {
                        // Special event, can not defer, must be processed immediately
                        var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_BrowserViewControllerNearbyPeerPresentingEventData>(eventDataPtr);
                        PeerId peerId = new PeerId(eventData.PeerId);

                        Dictionary<string, string> discoveryInfo =
                            eventData.DiscoveryInfoPairArray != IntPtr.Zero ?
                            MarshalUtility.MarshalStringStringDictionary(eventData.DiscoveryInfoPairArray, eventData.DiscoveryInfoArrayPairCount) :
                            null;

                        NativeMethods.Events.UMC_BrowserViewControllerNearbyPeerPresentingResultEventData result;
                        result.ShouldPresent = true;
                        _peerDiscovery.OnNearbyPeerPresenting(peerId, discoveryInfo, ref result.ShouldPresent);

                        // Copy from managed to native
                        Marshal.StructureToPtr(result, eventData.Result, false);
                        // Event data is freed on the native side automatically after this method returns
                        return;
                    }
                    // Logs
                    case NativeMethods.Events.EventType.Log: {
                        // Special event, logs can be logged at any time
                        var eventData = MarshalUtility.PtrToStructure<NativeMethods.Events.UMC_LogEventData>(eventDataPtr);
                        switch (eventData.Type) {
                            case NativeMethods.LogType.Log:
                                Debug.Log("[ALM] " + eventData.Text);
                                break;
                            case NativeMethods.LogType.Warning:
                                Debug.LogWarning("[ALM] " + eventData.Text);
                                break;
                            case NativeMethods.LogType.Error:
                                Debug.LogError("[ALM] " + eventData.Text);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("eventData.Type");
                        }

                        NativeMethods.Events.UMC_FreeEventData(eventType, eventDataPtr);
                        return;
                    }
                    default:
                        throw new ArgumentOutOfRangeException("eventType", eventType, null);
                }
            }
        }

        private struct NativeEvent {
            public readonly NativeMethods.Events.EventType EventType;

            /// <summary>
            /// Pointer to the native event data.
            /// </summary>
            public readonly IntPtr EventData;

            public NativeEvent(NativeMethods.Events.EventType eventType, IntPtr eventData) {
                EventType = eventType;
                EventData = eventData;
            }
        }
    }
}

#endif

