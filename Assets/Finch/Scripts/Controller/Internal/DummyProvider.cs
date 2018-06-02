using System.Linq;
using UnityEngine;

namespace Finch
{
    public class DummyProvider : IFinchProvider
    {
        private static readonly FinchBone[] RightBones = {FinchBone.LeftUpperArm, FinchBone.LeftLowerArm, FinchBone.LeftHand, FinchBone.LeftHandCenter};
        private static readonly FinchBone[] LeftBones = {FinchBone.RightUpperArm, FinchBone.RightLowerArm, FinchBone.RightHand, FinchBone.RightHandCenter};
        private static readonly FinchBone[] LeftArmBones = {FinchBone.Chest, FinchBone.LeftShoulder, FinchBone.LeftUpperArm, FinchBone.LeftLowerArm, FinchBone.LeftHand, FinchBone.LeftHandCenter};
        private static readonly FinchBone[] RightArmBones = {FinchBone.Chest, FinchBone.RightShoulder, FinchBone.RightUpperArm, FinchBone.RightLowerArm, FinchBone.RightHand, FinchBone.RightHandCenter};
        private static readonly FinchBone[] NeckBones = {FinchBone.Chest, FinchBone.Neck, FinchBone.Head};

        private static readonly PlayerState DefaultState = GetDefaultState();

        public void Exit()
        {
            WarningDummy("FinchExit");
        }

        public void ReadState(PlayerState outState)
        {
            outState.CopyFrom(DefaultState);
        }

        public void HapticPulse(FinchNodeType type, uint millisecond)
        {
            WarningDummy("HapticPulse");
        }

        public void HapticPulse(FinchNodeType type, params VibrationPackage[] milliseconds)
        {
            WarningDummy("HapticPulse");
        }

        public void ChangeDevice(FinchControllerType deviceType)
        {
            WarningDummy("ChangeDevice");
        }

        public void ChangeBodyRotationMode(FinchBodyRotationMode bodyRotationMode)
        {
            WarningDummy("ChangeBodyRotationMode");
        }

        public void Calibrate(FinchChirality chirality, FinchRecenterMode recenterMode)
        {
            WarningDummy("Calibration");
        }

        public void Recenter(FinchChirality chirality, FinchRecenterMode recenterMode)
        {
            WarningDummy("Recenter");
        }

        public float GetBatteryCharge(FinchNodeType nodeType)
        {
            return 100f;
        }

        public void SwapNodes(FinchNodeType firstNode, FinchNodeType secondNode)
        {
            WarningDummy("SwapNodes");
        }

        public FinchControllerModel GetControllerModel(FinchNodeType nodeType)
        {
            return FinchControllerModel.Unknown;
        }

        public void StartChiralityRedefine()
        {
            WarningDummy("StartChiralityRedefine");
        }

        public bool IsChiralityRedefining()
        {
            return false;
        }

        public void SetBoneLength(FinchBone bone, float lenhth)
        {
            WarningDummy("SetBoneLength");
        }

        private void WarningDummy(string functionName)
        {
            Debug.LogWarning("Dummy Provider is active now, " + functionName + " will do nothing.");
        }

        private static PlayerState GetDefaultState()
        {
            PlayerState state = PlayerState.Identity;
            state.NodesState = new FinchNodesState
            {
                Flag = uint.MaxValue
            };

            state.Rotations[PlayerState.Bones[FinchBone.Chest]] = new FinchQuaternion(0f, 0f, 0f, 1f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.Head]] = new FinchQuaternion(0f, 0f, 0f, 1f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.LeftShoulder]] = new FinchQuaternion(0f, 0f, 0f, 1f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.RightShoulder]] = new FinchQuaternion(0f, 0f, 0f, 1f).ToUnity();

            state.Rotations[PlayerState.Bones[FinchBone.RightUpperArm]] = new FinchQuaternion(0.593858f, -0.293152f, 0.245984f, 0.707733f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.RightLowerArm]] = new FinchQuaternion(0.5f, -0.5f, 0.5f, 0.5f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.RightHand]] = new FinchQuaternion(0.5f, -0.5f, 0.5f, 0.5f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.RightHandCenter]] = new FinchQuaternion(0.5f, -0.5f, 0.5f, 0.5f).ToUnity();

            state.Rotations[PlayerState.Bones[FinchBone.LeftUpperArm]] = new FinchQuaternion(-0.593858f, -0.293152f, -0.245984f, 0.707733f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.LeftLowerArm]] = new FinchQuaternion(-0.5f, -0.5f, -0.5f, 0.5f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.LeftHand]] = new FinchQuaternion(-0.5f, -0.5f, -0.5f, 0.5f).ToUnity();
            state.Rotations[PlayerState.Bones[FinchBone.LeftHandCenter]] = new FinchQuaternion(-0.5f, -0.5f, -0.5f, 0.5f).ToUnity();

            state.BonesLengths[PlayerState.Bones[FinchBone.Neck]] = 0.11f;
            state.BonesLengths[PlayerState.Bones[FinchBone.Head]] = 0.12f;

            state.BonesLengths[PlayerState.Bones[FinchBone.RightShoulder]] = 0.2f;
            state.BonesLengths[PlayerState.Bones[FinchBone.RightUpperArm]] = 0.33f;
            state.BonesLengths[PlayerState.Bones[FinchBone.RightLowerArm]] = 0.29f;
            state.BonesLengths[PlayerState.Bones[FinchBone.RightHand]] = 0.0816f;

            state.BonesLengths[PlayerState.Bones[FinchBone.LeftShoulder]] = 0.2f;
            state.BonesLengths[PlayerState.Bones[FinchBone.LeftUpperArm]] = 0.33f;
            state.BonesLengths[PlayerState.Bones[FinchBone.LeftLowerArm]] = 0.29f;
            state.BonesLengths[PlayerState.Bones[FinchBone.LeftHand]] = 0.0816f;

            FinchVector3[] boneLocalPositions = new FinchVector3[PlayerState.Bones.BoneCount];

            boneLocalPositions[PlayerState.Bones[FinchBone.Neck]] = new FinchVector3(0f, 0f, 1f);
            boneLocalPositions[PlayerState.Bones[FinchBone.Head]] = new FinchVector3(0f, 0f, 1f);

            boneLocalPositions[PlayerState.Bones[FinchBone.LeftUpperArm]] = new FinchVector3(0f, 1f, 0f);
            boneLocalPositions[PlayerState.Bones[FinchBone.LeftLowerArm]] = new FinchVector3(0f, 1f, 0f);
            boneLocalPositions[PlayerState.Bones[FinchBone.LeftHand]] = new FinchVector3(0f, 1f, 0f);
            boneLocalPositions[PlayerState.Bones[FinchBone.LeftHandCenter]] = new FinchVector3(0f, 0.05f / 0.08f, 0f);

            boneLocalPositions[PlayerState.Bones[FinchBone.RightUpperArm]] = new FinchVector3(0f, -1f, 0f);
            boneLocalPositions[PlayerState.Bones[FinchBone.RightLowerArm]] = new FinchVector3(0f, -1f, 0f);
            boneLocalPositions[PlayerState.Bones[FinchBone.RightHand]] = new FinchVector3(0f, -1f, 0f);
            boneLocalPositions[PlayerState.Bones[FinchBone.RightHandCenter]] = new FinchVector3(0.05f / 0.08f, 0f, 0f);

            UpdateCoordinates(state, LeftArmBones, boneLocalPositions);
            UpdateCoordinates(state, RightArmBones, boneLocalPositions);
            UpdateCoordinates(state, NeckBones, boneLocalPositions);

            return state;
        }

        private static void UpdateCoordinates(PlayerState state, FinchBone[] boneChain, FinchVector3[] boneLocalPositions)
        {
            Vector3 position = Vector3.zero;
            for (int i = 1; i < boneChain.Length; ++i)
            {
                FinchBone parent = boneChain[i - 1];
                FinchBone child = boneChain[i];
                float length = state.BonesLengths[PlayerState.Bones[parent]];
                Quaternion parentRot = state.Rotations[PlayerState.Bones[parent]];
                position += length * (parentRot * boneLocalPositions[PlayerState.Bones[child]].ToUnity());
                state.Positions[PlayerState.Bones[child]] = position;
            }
        }
    }
}