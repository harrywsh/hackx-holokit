#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX
using System;

namespace LostPolygon.Apple.LocalMultiplayer.Networking {
    /// <summary>
    /// Custom local multiplayer device browser definition.
    /// </summary>
    /// <tocexclude />
    public interface ICustomDeviceBrowser {
        event Action OnOpened;
        event Action OnClosing;
        event Action<PeerId> OnPeerPicked;
        void Open();
        void Close();
    }
}

#endif
