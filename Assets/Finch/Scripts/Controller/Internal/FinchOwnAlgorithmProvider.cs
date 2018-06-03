using UnityEngine;

namespace Finch
{
    public class FinchOwnAlgorithmProvider : FinchProvider
    {
        const float eyeForwardShift = 0.08f;

        public FinchOwnAlgorithmProvider(FinchControllerType deviceType) : base(deviceType) { }

        public override void ReadState(PlayerState outState)
        {
            base.ReadState(outState);

            for (int i = 0; i < PlayerState.Bones.BoneCount; ++i)
            {
                FinchBone bone = PlayerState.Bones[i];
                outState.Rotations[i] = FinchCore.GetBoneRotation(bone);
                outState.Positions[i] = FinchCore.GetBoneCoordinate(bone);
                outState.Available[i] = FinchCore.IsBoneAvailable(bone);
                outState.BonesLengths[i] = FinchCore.GetBoneLength(bone);
            }

            outState.Positions[PlayerState.Bones[FinchBone.Head]] = outState.Positions[PlayerState.Bones[FinchBone.Neck]]
                                                                    + outState.Rotations[PlayerState.Bones[FinchBone.Hips]] * Vector3.up * outState.BonesLengths[PlayerState.Bones[FinchBone.Neck]];

            outState.Positions[PlayerState.Bones[FinchBone.RightEye]] = outState.Positions[PlayerState.Bones[FinchBone.Head]]
                                                                        + outState.Rotations[PlayerState.Bones[FinchBone.Head]] * Vector3.up * outState.BonesLengths[PlayerState.Bones[FinchBone.Head]]
                                                                        + outState.Rotations[PlayerState.Bones[FinchBone.Head]] * Vector3.forward * eyeForwardShift;
        }
    }
}