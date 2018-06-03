using UnityEngine;
using UnityEngine.Networking;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    public class CreateTapMarkerMessage : MessageBase {
        // Some arbitrary message type id number
        public const short kMessageType = 12345;

        // Position of the tap
        public Vector2 Position;
    }
}
