using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Manages head and single controller GameObjects on scene.
    /// </summary>
    public class FinchSingleControllerPlayer : FinchPlayer
    {
        /// <summary>
        /// Controller GameObject.
        /// </summary>
        public GameObject Controller;

        protected override void Start()
        {
            base.Start();
            SetVisualControllerChirality(Controller, FinchSettings.PreferredHandedness);
        }

        protected override void Update()
        {
            FinchChirality handedness = FinchSettings.PreferredHandedness;
            bool rightConnected = FinchVR.RightController.IsHandNodeConnected();
            bool leftConnected = FinchVR.LeftController.IsHandNodeConnected();

            if (HideDisconnectedController)
                UpdateControllerObjectActive(Controller, ((handedness == FinchChirality.Right) && rightConnected) || ((handedness == FinchChirality.Left) && leftConnected));
            else
                UpdateControllerObjectActive(Controller, true);


            base.Update();
            UpdateHandTransform(Controller, FinchVR.MainController);
        }
    }
}