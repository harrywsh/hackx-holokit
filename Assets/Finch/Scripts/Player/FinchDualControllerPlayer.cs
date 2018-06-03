using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Manages head and two controllers GameObjects on scene.
    /// </summary>
    public class FinchDualControllerPlayer : FinchPlayer
    {
        /// <summary>
        /// Right controller GameObject.
        /// </summary>
        public GameObject RightController;

        /// <summary>
        /// Left controller GameObject.
        /// </summary>
        public GameObject LeftController;

        protected override void Start()
        {
            base.Start();
            SetVisualControllerChirality(LeftController, FinchChirality.Left);
            SetVisualControllerChirality(RightController, FinchChirality.Right);
        }

        protected override void Update()
        {
            base.Update();
            if (HideDisconnectedController)
            {
                if (FinchSettings.ControllersCount == FinchControllersCount.One)
                {
                    UpdateControllerObjectActive(FinchSettings.PreferredHandedness == FinchChirality.Left ? LeftController : RightController, true);
                    UpdateControllerObjectActive(FinchSettings.PreferredHandedness == FinchChirality.Left ? RightController : LeftController, false);
                }
                else
                {
                    bool leftConnected = FinchVR.LeftController.IsHandNodeConnected();
                    bool rightConnected = FinchVR.RightController.IsHandNodeConnected();

                    UpdateControllerObjectActive(LeftController, leftConnected);
                    UpdateControllerObjectActive(RightController, rightConnected);
                }
            }
            else
            {
                UpdateControllerObjectActive(LeftController, true);
                UpdateControllerObjectActive(RightController, true);
            }

            UpdateTransforms();
        }

        private void UpdateTransforms()
        {
            UpdateHandTransform(LeftController, FinchVR.LeftController);
            UpdateHandTransform(RightController, FinchVR.RightController);
        }
    }
}