using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Finch controller position in skeletal bones hierarchy. Is used for FinchPlayer scripts.
    /// </summary>
    public enum ControllerOffset
    {
        /// <summary>
        /// Matches to FinchBone.LeftHandCenter or FinchBone.RightHandCenter.
        /// </summary>
        Standard,

        /// <summary>
        /// Matches to FinchBone.LeftHand or FinchBone.RightHand.
        /// </summary>
        Wrist,

        /// <summary>
        /// Matches to FinchBone.LeftLowerArm or FinchBone.RightLowerArm.
        /// </summary>
        Elbow,

        /// <summary>
        /// Matches to FinchBone.LeftUpperArm or FinchBone.RightUpperArm.
        /// </summary>
        Shoulder
    }

    /// <summary>
    /// Virtual instance of the Finch controller.
    /// </summary>
    public class FinchController
    {
        /// <summary>
        /// Controller chirality (e.g. right or left controller).
        /// </summary>
        public readonly FinchChirality Chirality;

        /// <summary>
        /// Bone corresponding this controller. By default, FinchBone.LeftHandCenter or FinchBone.RightHandCenter.
        /// </summary>
        public readonly FinchBone Bone;

        /// <summary>
        /// Controller node (physical device).
        /// </summary>
        public readonly FinchNodeType NodeType;

        private readonly Quaternion fPoseLeft = new Quaternion(0.5f, -0.5f, -0.5f, 0.5f);
        private readonly Quaternion fPoseRight = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);

        /// <summary>
        /// Creates new instance of the FinchController with specified chirality.
        /// </summary>
        /// <param name="chirality">Right or left</param>
        public FinchController(FinchChirality chirality)
        {
            Chirality = chirality;
            switch (chirality)
            {
                case FinchChirality.Left:
                    Bone = FinchBone.LeftHandCenter;
                    NodeType = FinchNodeType.LeftHand;
                    break;
                case FinchChirality.Right:
                    Bone = FinchBone.RightHandCenter;
                    NodeType = FinchNodeType.RightHand;
                    break;
                default:
                    Bone = FinchBone.Unknown;
                    NodeType = FinchNodeType.Unknown;
                    break;
            }
        }

        /// <summary>
        /// Returns controller rotation.
        /// </summary>
        /// <returns>Controller rotation quaternion</returns>
        public Quaternion GetRotation()
        {
            return FinchVR.State.GetRotation(PlayerState.Bones[NodeType]) * GetFPose(Chirality);
        }

        /// <summary>
        /// Returns controller posistion.
        /// </summary>
        /// <returns>Controller position</returns>
        public Vector3 GetPosition()
        {
            return GetPositionByOffset(FinchSettings.ControllerOffset);
        }

        /// <summary>
        /// Returns true, if element was pressed.
        /// </summary>
        /// <param name="element">Button, touchpad or another controller element</param>
        /// <returns>Controller element state. True, if element was pressed</returns>
        public bool GetPress(FinchControllerElement element)
        {
            ushort events = FinchVR.State.ElementsState[(int)Chirality];
            return (events & (0x1 << (int)element)) != 0;
        }

        /// <summary>
        /// Returns true, if element has just been pressed.
        /// </summary>
        /// <param name="element">Button, touchpad or another controller element</param>
        /// <returns>Controller element state. True, if element has just been pressed</returns>
        public bool GetPressDown(FinchControllerElement element)
        {
            ushort events = FinchVR.State.ElementsBeginEvents[(int)Chirality];
            return (events & (0x1 << (int)element)) != 0;
        }

        /// <summary>
        /// Returns true, if element has just been unpressed.
        /// </summary>
        /// <param name="element">Button, touchpad or another controller element</param>
        /// <returns>Controller element state. True, if element has just been unpressed</returns>
        public bool GetPressUp(FinchControllerElement element)
        {
            ushort events = FinchVR.State.ElementsEndEvents[(int)Chirality];
            return (events & (0x1 << (int)element)) != 0;
        }

        /// <summary>
        /// Returns touchpos coordinates.
        /// </summary>
        /// <returns>Touchpad touch position for touchpad and thumstick axes values for thumbstick</returns>
        public Vector2 GetTouchAxes()
        {
            return FinchVR.State.TouchAxes[(int)Chirality];
        }

        /// <summary>
        /// Returns true, if touchpad element is touched, otherwise false.
        /// </summary>
        /// <returns>Touchpad state. True, if touchpad element is touched, otherwise false</returns>
        public bool IsTouching()
        {
            return FinchVR.State.IsTouching[(int)Chirality];
        }

        /// <summary>
        /// Returns IndexTrigger value.
        /// </summary>
        /// <returns>IndexTrigger value</returns>
        public float GetIndexTrigger()
        {
            return FinchVR.State.IndexTrigger[(int)Chirality];
        }

        /// <summary>
        /// Returns gyroscope value in controller local coordinate system in degrees/sec.
        /// </summary>
        /// <returns>Gyroscope value in controller local coordinate system in degrees/sec</returns>
        public Vector3 GetGyro()
        {
            return FinchVR.State.Gyro[(int)NodeType];
        }

        /// <summary>
        /// Returns accelerometer value in controller local coordinate system.
        /// </summary>
        /// <returns>Accelerometer value in controller local coordinate system in m/s2.</returns>
        public Vector3 GetAccel()
        {
            return FinchVR.State.Accel[(int)NodeType];
        }

        /// <summary>
        /// Returns true, if hand node have state FinchNodesStateType.Connected.
        /// </summary>
        /// <returns>Hand node state. True, if hand node have state FinchNodesStateType.Connected</returns>
        public bool IsHandNodeConnected()
        {
            return FinchVR.State.NodesState.GetState(NodeType, FinchNodesStateType.Connected);
        }

        /// <summary>
        /// Sends vibration signal to the controller node. There will be vibration in certain milliseconds time, but not more than 2500 ms.
        /// </summary>
        /// <param name="millisecond">Certain milliseconds time, but not more than 2500 ms</param>
        public void HapticPulse(uint millisecond)
        {
            FinchVR.HapticPulse(NodeType, millisecond);
        }

        /// <summary>
        /// Sends instructions pack for vibration engine to the controller node. Every next instruction will be work after previous one end.
        /// </summary>
        /// <param name="vibrationPackage">instructions pack for vibration engine</param>
        public void HapticPulse(params VibrationPackage[] vibrationPackage)
        {
            FinchVR.HapticPulse(NodeType, vibrationPackage);
        }

        /// <summary>
        /// Calibrates arm with controller chirality.
        /// </summary>
        public void Calibrate()
        {
            FinchVR.Calibrate(Chirality);
        }

        /// <summary>
        /// Recenters arm with controller chirality.
        /// </summary>
        public void Recenter()
        {
            FinchVR.Recenter(Chirality);
        }

        private Quaternion GetFPose(FinchChirality chirality)
        {
            switch (chirality)
            {
                case FinchChirality.Left:
                    return fPoseLeft;
                case FinchChirality.Right:
                    return fPoseRight;
                default:
                    return Quaternion.identity;
            }
        }

        private Vector3 GetPositionByOffset(ControllerOffset offset)
        {
            switch (offset)
            {
                case ControllerOffset.Standard:
                    return FinchVR.State.GetPosition(Bone);
                case ControllerOffset.Wrist:
                    return Chirality == FinchChirality.Right ? FinchVR.State.GetPosition(FinchBone.RightHand) : FinchVR.State.GetPosition(FinchBone.LeftHand);
                case ControllerOffset.Elbow:
                    return Chirality == FinchChirality.Right ? FinchVR.State.GetPosition(FinchBone.RightLowerArm) : FinchVR.State.GetPosition(FinchBone.LeftLowerArm);
                case ControllerOffset.Shoulder:
                    return Chirality == FinchChirality.Right ? FinchVR.State.GetPosition(FinchBone.RightUpperArm) : FinchVR.State.GetPosition(FinchBone.LeftUpperArm);
                default:
                    return FinchVR.State.GetPosition(Bone);
            }
        }
    }
}