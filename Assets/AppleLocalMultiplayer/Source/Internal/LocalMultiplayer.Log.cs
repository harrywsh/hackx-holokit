#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        /// <summary>
        ///     Controls
        /// </summary>
        public interface ILog {
            /// <summary>
            ///     Enables or disables verbose log mode.
            /// </summary>
            void SetVerboseLog(bool isVerbose);

            /// <summary>
            ///     Enables or disables forwarding native logs to Unity.
            /// </summary>
            void SetLogForwarding(bool isEnabled);
        }

        private class LogContainer : ILog {
            public void SetVerboseLog(bool isVerbose) {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                NativeMethods.UMCLog.SetIsVerboseLog(isVerbose);
#endif
            }

            public void SetLogForwarding(bool isEnabled) {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                NativeMethods.UMCMediatorFacade.SetLogForwarding(isEnabled);
#endif
            }
        }
    }
}

#endif
