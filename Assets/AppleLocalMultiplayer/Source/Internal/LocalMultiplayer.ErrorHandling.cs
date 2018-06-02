#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        private static void AssertNativeError(bool success, IntPtr error) {
            if (success)
                return;

            if (error == IntPtr.Zero)
                throw new ArgumentNullException("error");

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
            try {
                NativeMethods.ErrorType code = (NativeMethods.ErrorType) NativeMethods.NSError.code(error);
                if (code == NativeMethods.ErrorType.None)
                    return;

                string localizedDescription = NativeMethods.NSError.localizedDescription(error);
                switch (code) {
                    case NativeMethods.ErrorType.None:
                        break;
                    case NativeMethods.ErrorType.Fatal:
                        throw new ApplicationException(localizedDescription);
                    case NativeMethods.ErrorType.NotSupported:
                        throw new NotSupportedException(localizedDescription);
                    case NativeMethods.ErrorType.SessionActive:
                        throw new InvalidOperationException(
                            "Local multiplayer session must be non-active for this operation. " +
                            "Check the value of Session.IsSessionActive"
                        );
                    case NativeMethods.ErrorType.SessionNotActive:
                        throw new InvalidOperationException(
                            "Local multiplayer session must be active for this operation. " +
                            "Check the value of Session.IsSessionActive"
                        );
                    case NativeMethods.ErrorType.InvalidState:
                        throw new InvalidOperationException(localizedDescription);
                    case NativeMethods.ErrorType.InvalidInput:
                        throw new ArgumentException(localizedDescription);
                    default:
                        throw new ArgumentOutOfRangeException(
                            String.Format("Unknown native error: (code {0}) {1} ", code, localizedDescription));
                }
            } finally {
                NativeMethods.NSObject.release(error);
            }
#endif
        }

        private static void ReleaseNativeError(bool success, IntPtr error) {
            if (success)
                return;

            if (error == IntPtr.Zero)
                throw new ArgumentNullException("error");

#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
            NativeMethods.NSObject.release(error);
#endif
        }
    }
}

#endif
