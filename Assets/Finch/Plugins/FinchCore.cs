using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Finch
{
    /// <summary>
    ///     Describes init error code.
    /// </summary>
    public enum FinchInitError : byte
    {
        None,
        AlreadyInitialized,
        NotInitialized,
        IllegalArgument,
        RuntimeError
    }

    /// <summary>
    ///     Describes update error code.
    /// </summary>
    public enum FinchUpdateError : byte
    {
        None,
        NotInitialized,
        IllegalArgument,
        RuntimeError
    }

    /// <summary>
    ///     Describes IO error code.
    /// </summary>
    public enum FinchIOError : byte
    {
        None,
        NotInitialized,
        IllegalArgument,
        RuntimeError
    }

    /// <summary>
    ///     Describes the bone of the skeleton for animation.
    /// </summary>
    public enum FinchBone : byte
    {
        Hips,
        LeftUpperLeg,
        RightUpperLeg,
        LeftLowerLeg,
        RightLowerLeg,
        LeftFoot,
        RightFoot,
        Spine,
        Chest,
        Neck,
        Head,
        LeftShoulder,
        RightShoulder,
        LeftUpperArm,
        RightUpperArm,
        LeftLowerArm,
        RightLowerArm,
        LeftHand,
        RightHand,
        LeftToes,
        RightToes,
        LeftEye,
        RightEye,
        Jaw,
        LeftThumbProximal,
        LeftThumbIntermediate,
        LeftThumbDistal,
        LeftIndexProximal,
        LeftIndexIntermediate,
        LeftIndexDistal,
        LeftMiddleProximal,
        LeftMiddleIntermediate,
        LeftMiddleDistal,
        LeftRingProximal,
        LeftRingIntermediate,
        LeftRingDistal,
        LeftLittleProximal,
        LeftLittleIntermediate,
        LeftLittleDistal,
        RightThumbProximal,
        RightThumbIntermediate,
        RightThumbDistal,
        RightIndexProximal,
        RightIndexIntermediate,
        RightIndexDistal,
        RightMiddleProximal,
        RightMiddleIntermediate,
        RightMiddleDistal,
        RightRingProximal,
        RightRingIntermediate,
        RightRingDistal,
        RightLittleProximal,
        RightLittleIntermediate,
        RightLittleDistal,

        LeftHandCenter,
        LeftThumbTip,
        LeftIndexTip,
        LeftMiddleTip,
        LeftRingTip,
        LeftLittleTip,

        RightHandCenter,
        RightThumbTip,
        RightIndexTip,
        RightMiddleTip,
        RightRingTip,
        RightLittleTip,

        LeftClavicle,
        RightClavicle,

        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes chirality of controller.
    /// </summary>
    public enum FinchChirality : byte
    {
        Right,
        Left,
        Both = 255,
        Last = 2,
        Unknown = Last
    }

    /// <summary>
    ///     Describes type of controller.
    /// </summary>
    public enum FinchControllerType : byte
    {
        Hand,
        Shift,
        Dash,
        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes sensor position.
    /// </summary>
    public enum FinchNodeType : byte
    {
        RightHand,
        LeftHand,
        RightUpperArm,
        LeftUpperArm,
        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes elements of a controller.
    /// </summary>
    public enum FinchControllerElement : byte
    {
        ButtonZero = 7,
        ButtonOne = 0,
        ButtonTwo = 1,
        ButtonThree = 2,
        ButtonFour = 3,
        ButtonThumb = 9,
        Touchpad = 15,
        IndexTrigger = 4,
        ButtonGrip = 5,
        Last = 16,
        Unknown = Last
    }

    /// <summary>
    ///     Describes flag of event state.
    /// </summary>
    public enum FinchEventType : byte
    {
        Begin,
        Process,
        End,
        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes recenter mode.
    /// </summary>
    public enum FinchRecenterMode : byte
    {
        Forward,
        HmdRotation,
        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes body rotation mode.
    /// </summary>
    public enum FinchBodyRotationMode : byte
    {
        None,
        ShoulderRotation,
        HandRotation,
        HandMotion,
        HmdRotation,
        ShoulderRotationWithReachout,
        IMUFullBodyRotation,
        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes step of axis calibration.
    /// </summary>
    public enum FinchAxisCalibrationStep : byte
    {
        One,
        Two,
        Three,
        Four,
        Last,
        Unknown = Last
    }

    /// <summary>
    ///     Describes flag of node state.
    /// </summary>
    public enum FinchNodesStateType : byte
    {
        Connected,
        Correctly,
        Swapped,
        Last,
        Unknown = Last
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FinchVector2
    {
        public FinchVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FinchVector3
    {
        public FinchVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FinchQuaternion
    {
        public FinchQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float X;
        public float Y;
        public float Z;
        public float W;
    }

    /// <summary>
    ///     Finch nodes state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FinchNodesState
    {
        /// <summary>
        ///     Internal representation of nodes state.
        /// </summary>
        public uint Flag;

        /// <summary>
        ///     Get certain type node state.
        /// </summary>
        /// <param name="node">Type of the node (physical device)</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool GetState(FinchNodeType node, FinchNodesStateType type)
        {
            return (Flag & (0x1 << ((int)type + (int)node * (int)FinchNodesStateType.Last))) != 0;
        }

        /// <summary>
        ///     Returns number of controllers that have certain state type.
        /// </summary>
        /// <param name="type">Nodes state type</param>
        /// <returns>Number of controllers that have certain state type</returns>
        public int Count(FinchNodesStateType type)
        {
            int count = 0;
            for (FinchNodeType i = 0; i < FinchNodeType.Last; ++i)
                count += GetState(i, type) ? 1 : 0;
            return count;
        }
    }


    /// <summary>
    ///     Contains device type and platform options.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FinchInitOptions
    {
        public FinchControllerType controller;
        public byte platform;
        public byte scanner;
    }

    /// <summary>
    ///     Vibration package.
    /// </summary>
    public struct VibrationPackage
    {
        /// <summary>
        ///     Time in milliseconds.
        /// </summary>
        public float Time;

        /// <summary>
        ///     Rotation speed from -1 to 1.
        /// </summary>
        public float Speed;
    }

    public static class FinchCore
    {
        /// <summary>
        ///     Version of Finch Core.
        /// </summary>
        public static string Version
        {
            get
            {
                uint size = Interop.FinchGetCoreVersion(null, 0);
                byte[] sb = new byte[size];
                Interop.FinchGetCoreVersion(sb, size);
                return Encoding.ASCII.GetString(sb);
            }
        }

        /// <summary>
        ///     Init Finch Core.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static FinchInitError Init(FinchControllerType controller)
        {
            var options = new FinchInitOptions
            {
                controller = controller,
                platform = 2,
                scanner = 1
            };
            Interop.FinchSetCs(new FinchVector3(0, -1, 0), new FinchVector3(0, 0, 1), new FinchVector3(1, 0, 0));
            return Interop.FinchInit(options);
        }

        /// <summary>
        ///     Exit Finch Core.
        /// </summary>
        public static void Exit()
        {
            Interop.FinchExit();
        }

        /// <summary>
        ///     Type of device that used in the Finch Сore.
        /// </summary>
        public static FinchControllerType ControllerType
        {
            get { return Interop.FinchGetControllerType(); }
        }

        /// <summary>
        ///     Nodes state.
        /// </summary>
        public static FinchNodesState NodesState
        {
            get { return new FinchNodesState {Flag = Interop.FinchGetNodesState()}; }
        }

        /// <summary>
        ///     Returns battery charge in percentages.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static float GetNodeCharge(FinchNodeType node)
        {
            return Interop.FinchGetNodeCharge(node);
        }

        /// <summary>
        ///     Returns the device name selected node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetNodeName(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeName(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeName(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static string GetNodeManufacturerName(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeManufacturerName(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeManufacturerName(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static string GetNodeModelNumber(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeModelNumber(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeModelNumber(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static string GetNodeSerialNumber(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeSerialNumber(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeSerialNumber(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static string GetNodeHardwareRevision(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeHardwareRevision(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeHardwareRevision(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static string GetNodeFirmwareRevision(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeFirmwareRevision(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeFirmwareRevision(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static string GetNodeSoftwareRevision(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeSoftwareRevision(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeSoftwareRevision(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        public static byte GetNodeVendorIDSource(FinchNodeType node)
        {
            return Interop.FinchGetNodeVendorIDSource(node);
        }

        public static ushort GetNodeVendorID(FinchNodeType node)
        {
            return Interop.FinchGetNodeVendorID(node);
        }

        public static ushort GetNodeProductID(FinchNodeType node)
        {
            return Interop.FinchGetNodeProductID(node);
        }

        public static ushort GetNodeProductVersion(FinchNodeType node)
        {
            return Interop.FinchGetNodeProductVersion(node);
        }

        /// <summary>
        ///     Returns the version of Firmware selected node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetNodeAddress(FinchNodeType node)
        {
            uint size = Interop.FinchGetNodeAddress(node, null, 0);
            byte[] sb = new byte[size];
            Interop.FinchGetNodeAddress(node, sb, size);
            return Encoding.ASCII.GetString(sb);
        }

        /// <summary>
        ///     Returns true, if bone data is available.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static bool IsBoneAvailable(FinchBone bone)
        {
            return Interop.FinchIsBoneAvailable(bone) != 0;
        }

        /// <summary>
        ///     Returns bone current orientation in space, as a quaternion.
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="fPose"></param>
        /// <returns></returns>
        public static Quaternion GetBoneRotation(FinchBone bone, bool fPose = false)
        {
            return fPose ? Interop.FinchGetBoneFPoseRotation(bone).ToUnity() : Interop.FinchGetBoneTPoseRotation(bone).ToUnity();
        }

        /// <summary>
        ///     Returns bone position relatively midpoint of left and right shoulders.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static Vector3 GetBoneCoordinate(FinchBone bone)
        {
            return Interop.FinchGetBoneCoordinate(bone).ToUnity();
        }

        /// <summary>
        ///     Returns bone acceleration in local coordinate system.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static Vector3 GetBoneLocalAcceleration(FinchBone bone)
        {
            return Interop.FinchGetBoneLocalAcceleration(bone).ToUnity();
        }

        /// <summary>
        ///     Returns bone angular velocity vector in local coordinate system.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static Vector3 GetBoneLocalAngularVelocity(FinchBone bone)
        {
            return Interop.FinchGetBoneLocalAngularVelocity(bone).ToUnity();
        }

        /// <summary>
        ///     Returns bone acceleration in local coordinate system.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static Vector3 GetBoneGlobalAcceleration(FinchBone bone)
        {
            return Interop.FinchGetBoneGlobalAcceleration(bone).ToUnity();
        }

        /// <summary>
        ///     Returns bone angular velocity vector in local coordinate system.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static Vector3 GetBoneGlobalAngularVelocity(FinchBone bone)
        {
            return Interop.FinchGetBoneGlobalAngularVelocity(bone).ToUnity();
        }

        /// <summary>
        ///     Returns coordinates of the touchpad.
        /// </summary>
        /// <param name="chirality"></param>
        /// <returns></returns>
        public static Vector2 GetTouchpadAxes(FinchChirality chirality)
        {
            return Interop.FinchGetTouchpadAxes(chirality).ToUnity();
        }

        /// <summary>
        ///     Returns the selected event of controller's element.
        /// </summary>
        /// <param name="chirality"></param>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetEvent(FinchChirality chirality, FinchControllerElement element, FinchEventType type)
        {
            return (Interop.FinchGetEvents(chirality, type) & (0x1 << (int)element)) != 0;
        }

        /// <summary>
        ///     Returns the events flag of controller's element.
        /// </summary>
        /// <param name="chirality"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ushort GetEvents(FinchChirality chirality, FinchEventType type)
        {
            return Interop.FinchGetEvents(chirality, type);
        }

        /// <summary>
        ///     Returns the value of the index trigger.
        /// </summary>
        /// <param name="chirality"></param>
        /// <returns></returns>
        public static float GetIndexTrigger(FinchChirality chirality)
        {
            return Interop.FinchGetIndexTrigger(chirality);
        }

        /// <summary>
        ///     Resets calibrate the selected chirality.
        /// </summary>
        /// <param name="chirality"></param>
        public static void ResetCalibration(FinchChirality chirality)
        {
            Interop.FinchResetCalibration(chirality);
        }

        /// <summary>
        ///     Sets forward direction of user by selected direction of hand.
        /// </summary>
        /// <param name="chirality"></param>
        /// <param name="mode"></param>
        public static void Recenter(FinchChirality chirality, FinchRecenterMode mode)
        {
            Interop.FinchRecenter(chirality, mode);
        }

        /// <summary>
        ///     Calibration with the redefine of the chirality of the nodes
        /// </summary>
        /// <param name="during"> if true redefine will be peformed only at the moment when it possible</param>
        public static void ChiralityRedefine(bool during)
        {
            Interop.FinchChiralityRedefine(during ? (byte)1 : (byte)0);
        }

        /// <summary>
        ///     Is chirality redefining calibration in process.
        /// </summary>
        public static bool IsChiralityRedefining
        {
            get { return Interop.FinchIsChiralityRedefining() != 0; }
        }

        /// <summary>
        ///     Full axis calibration.
        /// </summary>
        /// <param name="chirality"></param>
        /// <param name="step"></param>
        public static void AxisCalibration(FinchChirality chirality, FinchAxisCalibrationStep step)
        {
            Interop.FinchAxisCalibration(chirality, step);
        }

        /// <summary>
        ///     Calculates axis calibration matrices by current pose.
        /// </summary>
        /// <param name="chirality"></param>
        /// <param name="mode"></param>
        public static void OnePoseAxisCalibration(FinchChirality chirality, FinchRecenterMode mode)
        {
            Interop.FinchOnePoseAxisCalibration(chirality, mode);
        }

        /// <summary>
        ///     Sets the value of the selected bone length.
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="length"></param>
        public static void SetBoneLength(FinchBone bone, float length)
        {
            Interop.FinchSetBoneLength(bone, length);
        }

        /// <summary>
        ///     Returns the bone length skeletal model.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public static float GetBoneLength(FinchBone bone)
        {
            return Interop.FinchGetBoneLength(bone);
        }

        /// <summary>
        ///     Body rotation mode used in the Finch Сore.
        /// </summary>
        public static FinchBodyRotationMode BodyRotationMode
        {
            get { return Interop.FinchGetBodyRotationMode(); }
            set { Interop.FinchSetBodyRotationMode(value); }
        }

        /// <summary>
        ///     Loads previously saved players calibrations from file.
        /// </summary>
        public static void LoadCalibrations() { }

        /// <summary>
        ///     Loads previously saved players calibrations from file.
        /// </summary>
        public static void LoadMathSettings() { }

        /// <summary>
        ///     Saves current players calibrations to file.
        /// </summary>
        public static void SaveCalibrations() { }

        /// <summary>
        ///     Saves current players calibrations to file.
        /// </summary>
        public static void SaveMathSettings() { }

        /// <summary>
        ///     Update Finch Core Data.
        /// </summary>
        /// <returns></returns>
        public static FinchUpdateError Update()
        {
            return Interop.FinchUpdate();
        }

        /// <summary>
        ///     Update Finch Core Data with a rotation of the HMD.
        /// </summary>
        /// <param name="qhmd">HMD rotation</param>
        /// <returns></returns>
        public static FinchUpdateError Update(FinchQuaternion qhmd)
        {
            return Interop.FinchHmdRotationUpdate(qhmd);
        }

        /// <summary>
        ///     Update Finch Core Data with a transform of the HMD.
        /// </summary>
        /// <param name="qhmd">HMD rotation</param>
        /// <param name="phmd">HMD position</param>
        /// <returns></returns>
        public static FinchUpdateError Update(FinchQuaternion qhmd, FinchVector3 phmd)
        {
            return Interop.FinchHmdTransformUpdate(qhmd, phmd);
        }

        /// <summary>
        ///     Trigger a single haptic pulse on a controller.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static FinchIOError HapticPulse(FinchNodeType node, uint millisecond)
        {
            byte[] package = new byte[2];
            package[1] = 50;
            if (millisecond <= 30)
                package[0] = 3;
            else if (millisecond > 2550)
                package[0] = 255;
            else
                package[0] = (byte)(millisecond * 0.1);

            return Interop.FinchNodeAsyncWriteData(node, package, (uint)package.Length);
        }

        /// <summary>
        ///     Trigger a sequence by pattern haptic pulse on a controller.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static FinchIOError HapticPulseByPattern(FinchNodeType node, params VibrationPackage[] pattern)
        {
            int length = pattern.Length;
            if (length > 10)
                throw new ArgumentException("The number of arguments must be not more than 10.");

            byte[] package = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                byte time;
                sbyte speed;
                int byteIndex = i * 2;

                if (pattern[i].Speed < -1.0f)
                    speed = -50;
                else if (pattern[i].Speed > 1.0f)
                    speed = 50;
                else
                    speed = (sbyte)(50 * pattern[i].Speed);

                if (pattern[i].Time <= 30)
                    time = 3;
                else if (pattern[i].Time > 2550)
                    time = 255;
                else
                    time = (byte)(pattern[i].Time * 0.1);

                package[byteIndex] = time;
                package[byteIndex + 1] = (byte)speed;
            }

            return Interop.FinchNodeAsyncWriteData(node, package, (uint)package.Length);
        }

        public static class Interop
        {
#if (UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
            private const string LibPath = "__Internal";
#elif (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_ANDROID)
            private const string LibPath = "FinchCore";
#endif

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetCoreVersion([Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchInitError FinchInit(FinchInitOptions options);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchExit();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchControllerType FinchGetControllerType();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodesState();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern float FinchGetNodeCharge(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeName(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeManufacturerName(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeModelNumber(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeSerialNumber(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeHardwareRevision(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeFirmwareRevision(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeSoftwareRevision(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern byte FinchGetNodeVendorIDSource(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern ushort FinchGetNodeVendorID(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern ushort FinchGetNodeProductID(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern ushort FinchGetNodeProductVersion(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeAddress(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern byte FinchIsBoneAvailable(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchQuaternion FinchGetBoneTPoseRotation(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchQuaternion FinchGetBoneFPoseRotation(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchVector3 FinchGetBoneCoordinate(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchVector3 FinchGetBoneLocalAcceleration(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchVector3 FinchGetBoneLocalAngularVelocity(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchVector3 FinchGetBoneGlobalAcceleration(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchVector3 FinchGetBoneGlobalAngularVelocity(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchVector2 FinchGetTouchpadAxes(FinchChirality chirality);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern ushort FinchGetEvents(FinchChirality chirality, FinchEventType type);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern float FinchGetIndexTrigger(FinchChirality chirality);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetCs(FinchVector3 newX, FinchVector3 newY, FinchVector3 newZ);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetDefaultCs();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetOffsetCs(FinchVector3 v);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchResetCalibration(FinchChirality chirality);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchRecenter(FinchChirality chirality, FinchRecenterMode mode);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchChiralityRedefine(byte during);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern byte FinchIsChiralityRedefining();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchAxisCalibration(FinchChirality chirality, FinchAxisCalibrationStep step);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchOnePoseAxisCalibration(FinchChirality chirality, FinchRecenterMode mode);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetBoneLength(FinchBone bone, float length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern float FinchGetBoneLength(FinchBone bone);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetBodyRotationMode(FinchBodyRotationMode mode);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchBodyRotationMode FinchGetBodyRotationMode();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetCalibrations(string f);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchSetMathSettings(string f);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetCalibrations([Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetMathSettings([Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchUpdateError FinchUpdate();

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchUpdateError FinchHmdRotationUpdate(FinchQuaternion qhmd);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchUpdateError FinchHmdTransformUpdate(FinchQuaternion qhmd, FinchVector3 phmd);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchUpdateError FinchExternUpdate(
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] rightHand,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] leftHand,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] rightUpperArm,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] leftUpperArm);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchUpdateError FinchExternHmdRotationUpdate(
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] rightHand,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] leftHand,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] rightUpperArm,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] leftUpperArm,
                FinchQuaternion qhmd);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchUpdateError FinchExternHmdTransformUpdate(
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] rightHand,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] leftHand,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] rightUpperArm,
                [In] [MarshalAs(UnmanagedType.LPArray)] byte[] leftUpperArm,
                FinchQuaternion qhmd,
                FinchVector3 phmd);


            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern byte FinchNodeConnect(FinchNodeType node, string address, byte autoConnect);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchNodeDisconnect(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchNodeSwap(FinchNodeType first, FinchNodeType second);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchNodeSuspend(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern void FinchNodeResumed(FinchNodeType node);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern uint FinchGetNodeRawData(FinchNodeType node, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchIOError FinchNodeWriteData(FinchNodeType node, [In] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);

            [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern FinchIOError FinchNodeAsyncWriteData(FinchNodeType node, [In] [MarshalAs(UnmanagedType.LPArray)] byte[] data, uint length);
        }

        private static string GetMathSettingsPath()
        {
#if UNITY_EDITOR
            string path = Application.dataPath + @"/" + ControllerType + "MathSettings.json";
#else
            string path = Application.persistentDataPath + @"/" + ControllerType + "MathSettings.json";
#endif
            return path;
        }

        private static string GetCalibrationPath()
        {
#if UNITY_EDITOR
            string path = Application.dataPath + @"/" + ControllerType + "Calibration.json";
#else
            string path = Application.persistentDataPath + @"/" + ControllerType + "Calibration.json";
#endif
            return path;
        }
    }

    public static class FinchCoreInteropConvert
    {
        public static Vector2 ToUnity(this FinchVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToUnity(this FinchVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Quaternion ToUnity(this FinchQuaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static FinchVector2 ToFinch(this Vector2 uv)
        {
            return new FinchVector2(uv.x, uv.y);
        }

        public static FinchVector3 ToFinch(this Vector3 uv)
        {
            return new FinchVector3(uv.x, uv.y, uv.z);
        }

        public static FinchQuaternion ToFinch(this Quaternion uq)
        {
            return new FinchQuaternion(uq.x, uq.y, uq.z, uq.w);
        }
    }
}