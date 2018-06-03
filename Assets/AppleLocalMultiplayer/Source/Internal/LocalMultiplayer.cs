#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using UnityEngine;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        private static bool _isInitialized;
        private static bool _isDeinitialized;
        private static readonly ConnectivityContainer _connectivity = new ConnectivityContainer();
        private static readonly SessionContainer _session = new SessionContainer();
        private static readonly CustomPeerDiscoveryContainer _customPeerDiscovery = new CustomPeerDiscoveryContainer();
        private static readonly PeerDiscoveryContainer _peerDiscovery = new PeerDiscoveryContainer();
        private static readonly CustomServiceAdvertiserContainer _customServiceAdvertiser = new CustomServiceAdvertiserContainer();
        private static readonly ServiceAdvertiserContainer _serviceAdvertiser = new ServiceAdvertiserContainer();
        private static readonly LogContainer _log = new LogContainer();

        /// <inheritdoc cref="IConnectivity"/>
        public static IConnectivity Connectivity {
            get { return _connectivity; }
        }

        /// <inheritdoc cref="ICustomPeerDiscovery"/>
        public static ICustomPeerDiscovery CustomPeerDiscovery {
            get { return _customPeerDiscovery; }
        }

        /// <inheritdoc cref="ICustomServiceAdvertiser"/>
        public static ICustomServiceAdvertiser CustomServiceAdvertiser {
            get { return _customServiceAdvertiser; }
        }

        /// <inheritdoc cref="IPeerDiscovery"/>
        public static IPeerDiscovery PeerDiscovery {
            get { return _peerDiscovery; }
        }

        /// <inheritdoc cref="IServiceAdvertiser"/>
        public static IServiceAdvertiser ServiceAdvertiser {
            get { return _serviceAdvertiser; }
        }

        /// <inheritdoc cref="ISession"/>
        public static ISession Session {
            get { return _session; }
        }

        /// <inheritdoc cref="ILog"/>
        public static ILog Log {
            get { return _log; }
        }

        static AppleLocalMultiplayer() {
            Initialize();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            if (_isInitialized)
                return;

            if (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.OSXPlayer) {
                if (IntPtr.Size == 4)
                    throw new NotSupportedException("Apple Local Multiplayer only supports x86_64 macOS Editor and Player. 32-bit x86 not supported");
            }

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
            RegisterEventHandlers();
            if (PlayerPrefs.GetInt("ALM_LogForwardingAutoEnable", 0) != 0) {
                NativeMethods.UMCMediatorFacade.SetLogForwarding(true);
            }

            if (PlayerPrefs.GetInt("ALM_VerboseLogAutoEnable", 0) != 0) {
                NativeMethods.UMCLog.SetIsVerboseLog(true);
            }

            IntPtr error;
#if !UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS)
            bool success = NativeMethods.UMCMediatorFacade.SetViewController(NativeMethods.GetViewController(), out error);
#else
            bool success = NativeMethods.UMCMediatorFacade.SetMainWindow(NativeMethods.GetMainWindow(), out error);
#endif
            AssertNativeError(success, error);
            UnityMessagesBroadcaster.UpdateInstanceOnLoad();
#endif
            _isInitialized = true;

            // Stop and deinitialize native Obj-C side gracefully.
            // This is required pretty much only for the Editor.
            UnityMessagesBroadcaster.ApplicationQuitEntered += StopAndDeinitializeNative;
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => {
                StopAndDeinitializeNative();
            };
        }

        /// <summary>
        /// Stop and deinitialize native Obj-C side gracefully.
        /// </summary>
        private static void StopAndDeinitializeNative() {
#if (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && (!UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE)
            if (_isDeinitialized)
                return;

            Session.StopSession();
            ServiceAdvertiser.StopAdvertising();
            CustomServiceAdvertiser.StopAdvertising();
            PeerDiscovery.CloseBrowser();
            CustomPeerDiscovery.StopDiscovery();
            NativeMethods.Events.UMC_UMCUnityEvents_SetEventListener(null);
            IntPtr error;
#if !UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS)
            NativeMethods.UMCMediatorFacade.SetViewController(IntPtr.Zero, out error);
#else
            NativeMethods.UMCMediatorFacade.SetMainWindow(IntPtr.Zero, out error);
#endif
            _isDeinitialized = true;
#endif
        }
    }
}

#endif
