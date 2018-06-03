#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

namespace LostPolygon.Apple.LocalMultiplayer {
    /// <summary>
    /// Peer connection state.
    /// </summary>
    public enum PeerState: int {
        NotConnected,
        Connecting,
        Connected
    }
}

#endif
