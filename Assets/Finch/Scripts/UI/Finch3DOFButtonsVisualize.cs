using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Visualizes Finch 3DOF (Dash) element pressings.
    /// </summary>
    public class Finch3DOFButtonsVisualize : MonoBehaviour
    {
        /// <summary>
        /// Controller chirality. It is strictly recommended to use only SetChirality method to change this option in runtime.
        /// </summary>
        public FinchChirality Chirality;

        /// <summary>
        /// Corresponding touchpoint visualizer.
        /// </summary>
        public FinchTouchPoint TouchPoint;

        /// <summary>
        /// AppButton mesh renderer.
        /// </summary>
        public MeshRenderer AppButtonRenderer;

        /// <summary>
        /// AppButton material, when AppButton (FinchControllerElement.ButtonOne) is not pressed.
        /// </summary>
        public Material AppButtonUnpressed;

        /// <summary>
        /// AppButton material, when AppButton (FinchControllerElement.ButtonOne) is pressed.
        /// </summary>
        public Material AppButtonPressed;

        /// <summary>
        /// HomeButton mesh renderer.
        /// </summary>
        public MeshRenderer HomeButtonRenderer;

        /// <summary>
        /// HomeCalibrationButton material, when HomeCalibrationButton (FinchControllerElement.ButtonZero) is not pressed.
        /// </summary>
        public Material HomeCalibrationButtonUnpressed;

        /// <summary>
        /// HomeCalibrationButton material, when HomeCalibrationButton (FinchControllerElement.ButtonZero) is pressed.
        /// </summary>
        public Material HomeCalibrationButtonPressed;

        /// <summary>
        /// Trigger mesh renderer.
        /// </summary>
        public MeshRenderer TriggerRenderer;

        /// <summary>
        /// Trigger object needed to visualize degree of press
        /// </summary>
        public GameObject TriggerObject;

        /// <summary>
        /// Trigger material, when trigger (FinchControllerElement.IndexTrigger) is not pressed.
        /// </summary>
        public Material TriggerUnpressed;

        /// <summary>
        /// Trigger material, when trigger (FinchControllerElement.IndexTrigger) is pressed.
        /// </summary>
        public Material TriggerPressed;

        /// <summary>
        /// Volume buttons (both) mesh renderer.
        /// </summary>
        public MeshRenderer VolumeRenderer;

        /// <summary>
        /// VolumeUp material, when VolumeUp (FinchControllerElement.ButtonThree) is not pressed.
        /// </summary>
        public Material VolumePlusUnpressed;

        /// <summary>
        /// VolumeUp material, when VolumeUp (FinchControllerElement.ButtonThree) is pressed.
        /// </summary>
        public Material VolumePlusPressed;

        /// <summary>
        /// VolumeMinus material, when VolumeMinus (FinchControllerElement.ButtonTwo) is not pressed.
        /// </summary>
        public Material VolumeMinusUnpressed;

        /// <summary>
        /// VolumeMinus material, when VolumeMinus (FinchControllerElement.ButtonTwo) is pressed.
        /// </summary>
        public Material VolumeMinusPressed;

        private void Update()
        {
            FinchController controller = FinchVR.GetFinchController(Chirality);

            if (controller == null)
                return;

            HandleMaterialChange(controller, TriggerRenderer, 0, TriggerPressed, TriggerUnpressed, FinchControllerElement.IndexTrigger);
            TriggerAnimation(controller, FinchControllerElement.IndexTrigger, TriggerObject);
            HandleMaterialChange(controller, VolumeRenderer, 1, VolumePlusPressed, VolumePlusUnpressed, FinchControllerElement.ButtonThree);
            HandleMaterialChange(controller, VolumeRenderer, 2, VolumeMinusPressed, VolumeMinusUnpressed, FinchControllerElement.ButtonTwo);
            HandleMaterialChange(controller, HomeButtonRenderer, 0, HomeCalibrationButtonPressed, HomeCalibrationButtonUnpressed, FinchControllerElement.ButtonZero);
            HandleMaterialChange(controller, AppButtonRenderer, 1, AppButtonPressed, AppButtonUnpressed, FinchControllerElement.ButtonOne);
        }

        /// <summary>
        /// Change this and FinchTouchPoint scripts chirality fields.
        /// </summary>
        /// <param name="chirality">New chirality (right or left)</param>
        public void SetChirality(FinchChirality chirality)
        {
            Chirality = chirality;
            if (TouchPoint != null)
                TouchPoint.Chirality = chirality;
        }

        private void HandleMaterialChange(FinchController controller, MeshRenderer Renderer, int materialIndex, Material pressed, Material unpressed, FinchControllerElement element)
        {
            if (controller.GetPressDown(element))
            {
                Material[] arrayCopy = Renderer.materials;
                arrayCopy[materialIndex] = pressed;
                Renderer.materials = arrayCopy;
            }
            else if (controller.GetPressUp(element))
            {
                Material[] arrayCopy = Renderer.materials;
                arrayCopy[materialIndex] = unpressed;
                Renderer.materials = arrayCopy;
            }
        }

        private void HandleMaterialReset(MeshRenderer Renderer, int materialIndex, Material Mat)
        {
            Material[] arrayCopy = Renderer.materials;
            arrayCopy[materialIndex] = Mat;
            Renderer.materials = arrayCopy;
        }

        private void TriggerAnimation(FinchController controller, FinchControllerElement element, GameObject model)
        {
            if (controller.GetPressDown(element))
            {
                model.transform.localPosition = new Vector3(0, 0, 0.021f);
            }
            else if (controller.GetPressUp(element))
            {
                model.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}