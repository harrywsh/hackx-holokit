using UnityEngine;
using UnityEngine.Networking;
using LostPolygon.Apple.LocalMultiplayer.Networking;
using Random = UnityEngine.Random;

namespace LostPolygon.Apple.LocalMultiplayer.Demos {
    public class AppleLocalMultiplayerDemoNetworkManager : AppleLocalMultiplayerNetworkManager {
        public GameObject TapMarkerPrefab; // Reference to the tap effect
        public bool StressTestMode;

        private const int kStressModeActors = 100;

        public override void OnStartServer() {
            base.OnStartServer();

            // Register the handler for the CreateTapMarkerMessage that is sent from client to server
            NetworkServer.RegisterHandler(CreateTapMarkerMessage.kMessageType, OnServerCreateTapMarkerHandler);
        }

        public override void OnStartClient(NetworkClient client) {
            base.OnStartClient(client);

            // Register the handler for the CreateTapMarkerMessage that is sent from server to clients
            client.RegisterHandler(CreateTapMarkerMessage.kMessageType, OnClientCreateTapMarkerHandler);
        }

        public override void OnServerReady(NetworkConnection connection) {
            base.OnServerReady(connection);

            // Spawn the controllable actors
            int actorCount = !StressTestMode ? 1 : kStressModeActors;
            for (int i = 0; i < actorCount; i++) {
                Vector3 position = Random.insideUnitCircle * 15f;
                GameObject actorGameObject = (GameObject) Instantiate(playerPrefab, position, Quaternion.identity);
                TestActor testActor = actorGameObject.GetComponent<TestActor>();

                // Make them smaller and more random in stress test mode
                if (StressTestMode) {
                    testActor.PositionRandomOffset = 10f;
                    actorGameObject.transform.localScale *= 0.5f;
                    testActor.TransformLocalScale = actorGameObject.transform.localScale;
                }

                // Set player authority
                NetworkServer.SpawnWithClientAuthority(actorGameObject, connection);

                // Create a new virtual player for this actor
                PlayerController playerController = new PlayerController();
                playerController.gameObject = actorGameObject;
                playerController.unetView = actorGameObject.GetComponent<NetworkIdentity>();

                connection.playerControllers.Add(playerController);
            }
        }

        // Called when client receives a CreateTapMarkerMessage
        private void OnClientCreateTapMarkerHandler(NetworkMessage netMsg) {
            // Just instantiate a tap marker in the tap position
            CreateTapMarkerMessage createTapMarkerMessage = netMsg.ReadMessage<CreateTapMarkerMessage>();
            Instantiate(TapMarkerPrefab, createTapMarkerMessage.Position, Quaternion.identity);
        }

        // Called when server receives a CreateTapMarkerMessage
        private void OnServerCreateTapMarkerHandler(NetworkMessage netMsg) {
            // Read the message
            CreateTapMarkerMessage createTapMarkerMessage = netMsg.ReadMessage<CreateTapMarkerMessage>();

            // Retransmit this message to all other clients except the one who initially sent it,
            // since that client already creates a local tap marker on his own
            foreach (NetworkConnection connection in NetworkServer.connections) {
                if (connection == null || connection == netMsg.conn)
                    continue;

                connection.Send(CreateTapMarkerMessage.kMessageType, createTapMarkerMessage);
            }

            foreach (NetworkConnection connection in NetworkServer.localConnections) {
                if (connection == null || connection == netMsg.conn)
                    continue;

                connection.Send(CreateTapMarkerMessage.kMessageType, createTapMarkerMessage);
            }
        }
    }
}
