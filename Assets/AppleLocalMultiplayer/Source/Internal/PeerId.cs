#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    /// <summary>
    /// Uniquely represents a peer of the local multiplayer session.
    /// </summary>
    public class PeerId : NSObject {
        private readonly int _hash;
        private readonly string _name;

        internal PeerId(IntPtr pointer, string name, int hash)
            : base(pointer) {
            _name = name;
            _hash = hash;
        }

        internal PeerId(IntPtr pointer)
            : base(pointer) {
#if !UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE
            _name = NativeMethods.MCPeerID.displayName(pointer);
            _hash = unchecked((int) NativeMethods.NSObject.hash(pointer));
#endif
        }

        /// <summary>
        /// Name of the peer.
        /// </summary>
        public string Name {
            get { return _name; }
        }

        public override int GetHashCode() {
            return _hash;
        }

        public override string ToString() {
            return _name;
        }
    }
}

#endif
