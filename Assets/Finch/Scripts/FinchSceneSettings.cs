using UnityEngine;

namespace Finch
{
    public class FinchSceneSettings : MonoBehaviour
    {
        [Header("Camera Settings")]
        [Tooltip("Leave this field empty if you want to use data of UnityEngine.Camera.main")]
        public Transform Camera;

        [Header("Scene Settings")]
        public FinchControllersCount ControllersCount;
        public FinchHeadUpdateType HeadUpdateType = FinchHeadUpdateType.RotationUpdate;
        public FinchBodyRotationMode BodyRotationMode = FinchBodyRotationMode.HmdRotation;
        public FinchRecenterMode RecenterMode;

        [Header("Player Settings")]
        [Tooltip("These settings are stored in PlayerPrefs")]
        public FinchChirality PreferredHandedness;
        public ControllerOffset ControllerOffset;

        void Awake()
        {
            FinchVR.SetCamera(Camera != null ? Camera : UnityEngine.Camera.main.transform);
            FinchSettings.InitPlayerSettings();
            FinchSettings.InitSceneSettings(HeadUpdateType, ControllersCount, BodyRotationMode, RecenterMode);
        }
    }
}