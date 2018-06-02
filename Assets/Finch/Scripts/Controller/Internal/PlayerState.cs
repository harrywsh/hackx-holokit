using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Keeps controller data from last update.
    /// </summary>
    public class PlayerState
    {
        /// <summary>
        /// The list of proceeding skeletal model bones.
        /// </summary>
        public static readonly BoneDictionary Bones = new BoneDictionary();

        /// <summary>
        /// Identity state. All coordinates are zero, all rotations are identity, all buttons are unpressed.
        /// </summary>
        public static readonly PlayerState Identity = new PlayerState();

        /// <summary>
        /// Bones rotations.
        /// </summary>
        public readonly Quaternion[] Rotations = new Quaternion[Bones.BoneCount];

        /// <summary>
        /// Bones name joints positions.
        /// </summary>
        public readonly Vector3[] Positions = new Vector3[Bones.BoneCount];

        /// <summary>
        /// Bone leading node avaliable.
        /// </summary>
        public readonly bool[] Available = new bool[Bones.BoneCount];

        /// <summary>
        /// Bones lengths. Bones start from their name joint. For example, bone between elbow and wrist is LowerArm (Elbow) bone.
        /// </summary>
        public readonly float[] BonesLengths = new float[Bones.BoneCount];

        /// <summary>
        /// Low-level representation of the BeginEvents. Correspondent bit value of array with chirality index is equal 1 at the first frame when condition is realized.
        /// </summary>
        public readonly ushort[] ElementsBeginEvents = new ushort[(int)FinchChirality.Last];

        /// <summary>
        /// Low-level representation of the current FinchControllerElement state.
        /// </summary>
        public readonly ushort[] ElementsState = new ushort[(int)FinchChirality.Last];

        /// <summary>
        /// Low-level representation of the EndEvents. Correspondent bit value of array with chirality index is equal 1 at the first frame when condition is not realized.
        /// </summary>
        public readonly ushort[] ElementsEndEvents = new ushort[(int)FinchChirality.Last];

        /// <summary>
        /// Touchpad state. Array element value is true, if touchpad element of correspondent chirality controller is touched, otherwise false.
        /// </summary>
        public readonly bool[] IsTouching = new bool[(int)FinchChirality.Last];

        /// <summary>
        /// Touchpad elements touchposes.
        /// </summary>
        public readonly Vector2[] TouchAxes = new Vector2[(int)FinchChirality.Last];

        /// <summary>
        /// IndexTriggers values.
        /// </summary>
        public readonly float[] IndexTrigger = new float[(int)FinchChirality.Last];

        /// <summary>
        /// Array element value is true at the first frame when condition is realized.
        /// </summary>
        public readonly bool[] CalibrationButtonPressed = new bool[(int)FinchChirality.Last];

        /// <summary>
        /// Gyroscopes values in node local CS.
        /// </summary>
        public readonly Vector3[] Gyro = new Vector3[(int)FinchNodeType.Last];

        /// <summary>
        /// Accelerometers values in node local CS.
        /// </summary>
        public readonly Vector3[] Accel = new Vector3[(int)FinchNodeType.Last];

        /// <summary>
        /// Nodes state.
        /// </summary>
        public FinchNodesState NodesState;

        /// <summary>
        /// An error message, if this error was occurred at the PlayerState receiving moment.
        /// </summary>
        public string ErrorDetails = string.Empty;

        /// <summary>
        /// Returns bone rotation.
        /// </summary>
        /// <param name="bone">Certain bone</param>
        /// <returns>Rotation quaternion</returns>
        public Quaternion GetRotation(FinchBone bone)
        {
            int index = Bones[bone];
            return index == -1 ? Quaternion.identity : Rotations[index];
        }

        /// <summary>
        /// Returns bone position.
        /// </summary>
        /// <param name="bone">Certain bone</param>
        /// <returns>Position coordinates</returns>
        public Vector3 GetPosition(FinchBone bone)
        {
            int index = Bones[bone];
            return index == -1 ? Vector3.zero : Positions[index];
        }

        /// <summary>
        /// Returns true, if bone leading node avaliable
        /// </summary>
        /// <param name="bone">Certain bone</param>
        /// <returns>Bone leading node state</returns>
        public bool GetAvaliable(FinchBone bone)
        {
            int index = Bones[bone];
            return index == -1 ? false : Available[index];
        }

        /// <summary>
        /// Returns bone length. Bones start from their name joint. For example, bone between elbow and wrist is LowerArm (Elbow) bone.
        /// </summary>
        /// <param name="bone">Certain bone. Note: bones start from their name joint</param>
        /// <returns>Bone length</returns>
        public float GetBoneLength(FinchBone bone)
        {
            int index = Bones[bone];
            return index == -1 ? 0f : BonesLengths[index];
        }

        /// <summary>
        /// Fills this state by another PlayerState values
        /// </summary>
        /// <param name="sourceState">State which keeps copiing data</param>
        public void CopyFrom(PlayerState sourceState)
        {
            for (int i = 0; i < Bones.BoneCount; ++i)
            {
                Rotations[i] = sourceState.Rotations[i];
                Positions[i] = sourceState.Positions[i];
                Available[i] = sourceState.Available[i];
                BonesLengths[i] = sourceState.BonesLengths[i];
            }

            for (int i = 0; i < (int)FinchChirality.Last; ++i)
            {
                ElementsBeginEvents[i] = sourceState.ElementsBeginEvents[i];
                ElementsState[i] = sourceState.ElementsEndEvents[i];
                ElementsEndEvents[i] = sourceState.ElementsEndEvents[i];
                IsTouching[i] = sourceState.IsTouching[i];
                TouchAxes[i] = sourceState.TouchAxes[i];
                IndexTrigger[i] = sourceState.IndexTrigger[i];
                CalibrationButtonPressed[i] = sourceState.CalibrationButtonPressed[i];
            }

            for (int i = 0; i < (int)FinchNodeType.Last; ++i)
            {
                Gyro[i] = sourceState.Gyro[i];
                Accel[i] = sourceState.Accel[i];
            }

            NodesState = sourceState.NodesState;
        }

        /// <summary>
        /// Keeps list of proceeding skeletal model bones.
        /// </summary>
        public class BoneDictionary
        {
            public BoneDictionary()
            {
                for (int i = 0; i < boneIndex.Length; i++)
                    boneIndex[i] = -1;
                for (int i = 0; i < boneArray.Length; ++i)
                    boneIndex[(int)boneArray[i]] = i;
            }

            /// <summary>
            /// Number of bones processed in the skeletal model.
            /// </summary>
            public int BoneCount
            {
                get { return boneArray.Length; }
            }

            /// <summary>
            /// Ordinal index of processed bone starting from zero.
            /// </summary>
            /// <param name="bone">Processed bone</param>
            /// <returns>Ordinal index of processed bone starting from zero</returns>
            public int this[FinchBone bone]
            {
                get { return bone < FinchBone.Last ? boneIndex[(int)bone] : -1; }
            }

            /// <summary>
            /// Bone by index, the index must have a value from zero to BoneCount.
            /// </summary>
            /// <param name="index">Ordinal index of processed bone starting from zero</param>
            /// <returns>Processed bone</returns>
            public FinchBone this[int index]
            {
                get { return index < boneArray.Length ? boneArray[index] : FinchBone.Unknown; }
            }

            /// <summary>
            /// Bone corresponding to node.
            /// </summary>
            /// <param name="nodeType">Type of the node (physical device)</param>
            /// <returns>Corresponding bone</returns>
            public FinchBone this[FinchNodeType nodeType]
            {
                get
                {
                    switch (nodeType)
                    {
                        case FinchNodeType.RightHand:
                            return FinchBone.RightHand;
                        case FinchNodeType.LeftHand:
                            return FinchBone.LeftHand;
                        case FinchNodeType.RightUpperArm:
                            return FinchBone.RightUpperArm;
                        case FinchNodeType.LeftUpperArm:
                            return FinchBone.LeftUpperArm;
                        default:
                            return FinchBone.Unknown;
                    }
                }
            }

            public static bool IsFingerBone(FinchBone bone)
            {
                foreach (FinchBone nonFingerBone in nonFingersBoneArray)
                    if (bone == nonFingerBone)
                        return false;

                return true;
            }

            private readonly int[] boneIndex = new int[(int)FinchBone.Last];

            private static readonly FinchBone[] boneArray =
            {
                FinchBone.Hips,
                FinchBone.Chest,
                FinchBone.Neck,
                FinchBone.Head,
                FinchBone.RightEye, //It's a cyclop eye :)
                FinchBone.LeftShoulder,
                FinchBone.RightShoulder,
                FinchBone.LeftUpperArm,
                FinchBone.RightUpperArm,
                FinchBone.LeftLowerArm,
                FinchBone.RightLowerArm,
                FinchBone.LeftHand,
                FinchBone.RightHand,
                FinchBone.LeftHandCenter,
                FinchBone.RightHandCenter,
                FinchBone.LeftThumbProximal,
                FinchBone.LeftThumbIntermediate,
                FinchBone.LeftThumbDistal,
                FinchBone.LeftThumbTip,
                FinchBone.LeftIndexProximal,
                FinchBone.LeftIndexIntermediate,
                FinchBone.LeftIndexDistal,
                FinchBone.LeftIndexTip,
                FinchBone.LeftMiddleProximal,
                FinchBone.LeftMiddleIntermediate,
                FinchBone.LeftMiddleDistal,
                FinchBone.LeftMiddleTip,
                FinchBone.LeftRingProximal,
                FinchBone.LeftRingIntermediate,
                FinchBone.LeftRingDistal,
                FinchBone.LeftRingTip,
                FinchBone.LeftLittleProximal,
                FinchBone.LeftLittleIntermediate,
                FinchBone.LeftLittleDistal,
                FinchBone.LeftLittleTip,
                FinchBone.RightThumbProximal,
                FinchBone.RightThumbIntermediate,
                FinchBone.RightThumbDistal,
                FinchBone.RightThumbTip,
                FinchBone.RightIndexProximal,
                FinchBone.RightIndexIntermediate,
                FinchBone.RightIndexDistal,
                FinchBone.RightIndexTip,
                FinchBone.RightMiddleProximal,
                FinchBone.RightMiddleIntermediate,
                FinchBone.RightMiddleDistal,
                FinchBone.RightMiddleTip,
                FinchBone.RightRingProximal,
                FinchBone.RightRingIntermediate,
                FinchBone.RightRingDistal,
                FinchBone.RightRingTip,
                FinchBone.RightLittleProximal,
                FinchBone.RightLittleIntermediate,
                FinchBone.RightLittleDistal,
                FinchBone.RightLittleTip
            };

            private static readonly FinchBone[] nonFingersBoneArray =
            {
                FinchBone.Hips,
                FinchBone.Chest,
                FinchBone.Neck,
                FinchBone.Head,
                FinchBone.RightEye, //It's a cyclop eye :)
                FinchBone.LeftShoulder,
                FinchBone.RightShoulder,
                FinchBone.LeftUpperArm,
                FinchBone.RightUpperArm,
                FinchBone.LeftLowerArm,
                FinchBone.RightLowerArm,
                FinchBone.LeftHand,
                FinchBone.RightHand,
                FinchBone.LeftHandCenter,
                FinchBone.RightHandCenter,
            };
        }
    }
}