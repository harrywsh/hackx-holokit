#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <summary>
        /// Provides access to device's connectivity options like Bluetooth.
        /// </summary>
        public interface IConnectivity {
#if !(UNITY_STANDALONE_OSX || UNITY_TVOS) || UNITY_EDITOR_OVERRIDE
            /// <summary>
            /// Whether Bluetooth is enabled on the device.
            /// </summary>
            bool IsBluetoothEnabled { get; }

            /// <summary>
            /// Opens the native OS dialog asking user to enable Bluetooth.
            /// </summary>
            /// <returns>true on success, false if Bluetooth is already enabled, or on error.</returns>
            void OpenEnableBluetoothPrompt();
#endif
        }

        private sealed class ConnectivityContainer : IConnectivity {
            public bool IsBluetoothEnabled {
                get {
#if !(UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_TVOS)
                    return NativeMethods.UMCMediatorFacade.IsBluetoothEnabled();
#else
                    return false;
#endif
                }
            }

            public void OpenEnableBluetoothPrompt() {
#if !(UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_TVOS)
                IntPtr error;
                bool success = NativeMethods.UMCMediatorFacade.OpenBluetoothEnablePrompt(out error);
                try {
                    AssertNativeError(success, error);
                } catch (NotSupportedException) {
                    // Not supported on simulator
                }
#endif
            }
        }
    }
}

#endif
