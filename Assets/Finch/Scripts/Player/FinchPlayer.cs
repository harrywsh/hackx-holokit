using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Abstract script. Children classes (FinchSingleControllerPlayer and FinchDualControllerPlayer) manage head and controllers GameObjects on scene.
    /// </summary>
    public class FinchPlayer : MonoBehaviour
    {
        /// <summary>
        /// Camera container GameObject.
        /// </summary>
        public GameObject Head;

        /// <summary>
        /// Disables controller GameObject if correspondent controller is not connected.
        /// </summary>
        public bool HideDisconnectedController;

        protected virtual void Start() { }

        protected virtual void Update()
        {
            if (FinchSettings.HeadUpdateType != FinchHeadUpdateType.RotationAndPositionUpdate)
                UpdateCameraPosition();
        }

        protected void UpdateControllerObjectActive(GameObject controllerObject, bool isConnected)
        {
            if (controllerObject != null)
            {
                if (!isConnected && controllerObject.activeSelf) //if controller became unavailable
                    controllerObject.SetActive(false);
                else if (isConnected && !controllerObject.activeSelf) //if controller became available
                    controllerObject.SetActive(true);
            }
        }

        protected void UpdateHandTransform(GameObject controllerObject, FinchController controller)
        {
            if (controllerObject != null && controllerObject.activeSelf)
            {
                controllerObject.transform.localRotation = controller.GetRotation();
                controllerObject.transform.localPosition = controller.GetPosition();
            }
        }

        protected void SetVisualControllerChirality(GameObject controllerObject, FinchChirality chirality)
        {
            Finch3DOFButtonsVisualize fcv = controllerObject.GetComponent<Finch3DOFButtonsVisualize>();
            if (fcv != null)
                fcv.SetChirality(chirality);
        }

        private void UpdateCameraPosition()
        {
            if (Head != null)
            {
                if (FinchSettings.DeviceType == FinchControllerType.Dash)
                    Head.transform.localPosition = FinchVR.State.GetPosition(FinchBone.Head);
                else if (FinchSettings.DeviceType == FinchControllerType.Shift)
                    Head.transform.localPosition = FinchVR.State.GetPosition(FinchBone.RightEye); //Center position of both eyes
            }
        }
    }
}