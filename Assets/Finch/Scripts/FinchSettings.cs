using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Finch
{
    /// <summary>
    /// Data source.
    /// </summary>
    public enum FinchDataSource
    {
        /// <summary>
        /// Use default values.
        /// </summary>
        Controller,

        /// <summary>
        /// Use data from controllers.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Target Finch controllers count.
    /// </summary>
    public enum FinchControllersCount
    {
        /// <summary>
        /// Single controller mode.
        /// </summary>
        One,

        /// <summary>
        /// Two controllers mode.
        /// </summary>
        Two
    }

    /// <summary>
    /// Pose tracking algorithm using for calculations.
    /// </summary>
    public enum FinchPoseTrackingAlgorithm
    {
        /// <summary>
        /// FinchVR pose tracking algorithm.
        /// </summary>
        FinchVR,

        /// <summary>
        /// GoogleVR (Daydream controller) pose tracking algorithm.
        /// </summary>
        GoogleVR
    }

    /// <summary>
    /// Type of data from camera (head) that used for tracking 
    /// </summary>
    public enum FinchHeadUpdateType
    {
        /// <summary>
        /// No data from camera are used for tracking, neither the rotation nor the position
        /// </summary>
        NoHeadUpdate,

        /// <summary>
        /// Only rotation is used for tracking. Default option
        /// </summary>
        RotationUpdate,

        /// <summary>
        /// Both position and rotation are used for tracking. Use this option if you have headset that allows head tracking (for example, HTC Vive)
        /// </summary>
        RotationAndPositionUpdate
    }

    /// <summary>
    /// Finch controller modes settings.
    /// </summary>
    public static class FinchSettings
    {
        public delegate void PreferredHandednessChangeAction(FinchChirality newChirality);

        public static event PreferredHandednessChangeAction OnPreferredHandednessChange;

        private static readonly string PreferredHandednessKey = "FINCH_HANDEDNESS";
        private static readonly string ControllerOffsetKey = "FINCH_CONTROLLER_OFFSET";

        public static void InitBuildSettings(FinchDataSource source, FinchControllerType type, FinchPoseTrackingAlgorithm trackingAlgorithm)
        {
            DataSource = source;
            DeviceType = type;
            PoseTrackingAlgorithm = trackingAlgorithm;
        }

        public static void InitSceneSettings(FinchHeadUpdateType headUpdateType, FinchControllersCount count, FinchBodyRotationMode rotationMode, FinchRecenterMode recenterMode)
        {
            HeadUpdateType = headUpdateType;
            UnityEngine.XR.InputTracking.disablePositionalTracking = HeadUpdateType == FinchHeadUpdateType.NoHeadUpdate || HeadUpdateType == FinchHeadUpdateType.RotationUpdate;

            ControllersCount = count;
            BodyRotationMode = rotationMode;
            RecenterMode = recenterMode;
        }

        public static void InitPlayerSettings(FinchChirality handedness = FinchChirality.Right, ControllerOffset offset = ControllerOffset.Standard)
        {
            preferredHandedness = (FinchChirality)PlayerPrefs.GetInt(PreferredHandednessKey, (int)handedness);
            controllerOffset = (ControllerOffset)PlayerPrefs.GetInt(ControllerOffsetKey, (int)offset);
        }

        /// <summary>
        /// Data source.
        /// </summary>
        public static FinchDataSource DataSource { get; private set; }

        /// <summary>
        /// Single controller mode or two controllers mode.
        /// </summary>
        public static FinchControllersCount ControllersCount { get; private set; }

        /// <summary>
        /// Finch device type. Use FinchDeviceType.Dash for 3DOF controllers and FinchDeviceType.Shift for 6DOF controllers.
        /// </summary>
        public static FinchControllerType DeviceType { get; private set; }

        /// <summary>
        /// Defines which type of data about camera transform should be sent to the core - none of them, only rotation, rotation and position
        /// </summary>
        public static FinchHeadUpdateType HeadUpdateType { get; private set; }

        /// <summary>
        /// Pose tracking algorithm used for calculations.
        /// </summary>
        public static FinchPoseTrackingAlgorithm PoseTrackingAlgorithm { get; private set; }

        /// <summary>
        /// Method of body rotation computing.
        /// </summary>
        public static FinchBodyRotationMode BodyRotationMode { get; private set; }

        /// <summary>
        /// Recentering mode.
        /// </summary>
        public static FinchRecenterMode RecenterMode { get; private set; }

        private static FinchChirality preferredHandedness;

        /// <summary>
        /// Chirality of main controller.
        /// </summary>
        public static FinchChirality PreferredHandedness
        {
            get
            {
#if UNITY_EDITOR
                return (FinchChirality)EditorPrefs.GetInt(PreferredHandednessKey);
#else
                return preferredHandedness;
#endif
            }
            set
            {
                if (preferredHandedness != value)
                {
                    PlayerPrefs.SetInt(PreferredHandednessKey, (int)value);
                    PlayerPrefs.Save();
#if UNITY_EDITOR
                    EditorPrefs.SetInt(PreferredHandednessKey, (int)value);
#endif
                    if (OnPreferredHandednessChange != null)
                        OnPreferredHandednessChange(value);
                }
                preferredHandedness = value;
            }
        }

        private static ControllerOffset controllerOffset;

        /// <summary>
        /// Finch controller position in skeletal bones hierarchy.
        /// </summary>
        public static ControllerOffset ControllerOffset
        {
            get
            {
#if UNITY_EDITOR
                return (ControllerOffset)EditorPrefs.GetInt(ControllerOffsetKey);
#else
                return controllerOffset;
#endif
            }
            set
            {
                if (controllerOffset != value)
                {
                    PlayerPrefs.SetInt(ControllerOffsetKey, (int)value);
                    PlayerPrefs.Save();
#if UNITY_EDITOR
                    EditorPrefs.SetInt(ControllerOffsetKey, (int)value);
#endif
                }
                controllerOffset = value;
            }
        }
    }
}