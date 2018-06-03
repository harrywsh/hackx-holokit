#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;

namespace LostPolygon.Apple.LocalMultiplayer.Internal {
    /// <summary>
    /// Wrapper for the native Objective-C NSObject. Automatically decreases the reference counter when GC'd.
    /// </summary>
    public class NSObject {
        private readonly IntPtr _pointer;

        protected internal NSObject(IntPtr pointer) {
            _pointer = pointer;
        }

        ~NSObject() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
            NativeMethods.NSObject.release(_pointer);
#endif
        }

        internal IntPtr NativePointer {
            get { return _pointer; }
        }

        protected bool Equals(NSObject other) {
            return _pointer.Equals(other._pointer)
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
                || NativeMethods.NSObject.isEquals(_pointer, other._pointer)
#endif
                ;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((NSObject) obj);
        }

        public override int GetHashCode() {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
            return unchecked((int) NativeMethods.NSObject.hash(_pointer));
#else
            return _pointer.ToInt32();
#endif
        }

        public static bool operator ==(NSObject left, NSObject right) {
            return Equals(left, right);
        }

        public static bool operator !=(NSObject left, NSObject right) {
            return !Equals(left, right);
        }
    }
}

#endif
