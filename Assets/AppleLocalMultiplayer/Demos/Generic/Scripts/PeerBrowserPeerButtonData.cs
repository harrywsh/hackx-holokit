using UnityEngine;
using UnityEngine.UI;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    /// <summary>
    /// Container for references to the peer browser item elements.
    /// </summary>
    public class PeerBrowserPeerButtonData : MonoBehaviour {
        public Button Button;
        public Text PeerNameText;
        public Text PeerStateText;

        [HideInInspector]
        public bool IsButtonRegistered;
    }
}
