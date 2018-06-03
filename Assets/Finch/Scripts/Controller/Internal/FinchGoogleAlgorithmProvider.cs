// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

namespace Finch
{
    public class FinchGoogleAlgorithmProvider : FinchProvider
    {
        public FinchGoogleAlgorithmProvider(FinchControllerType deviceType) : base(deviceType) { }

        private readonly Quaternion[] shoulderOrientations = new Quaternion[2];
        private readonly Quaternion[] elbowOrientations = new Quaternion[2];
        private readonly Quaternion[] wristOrientations = new Quaternion[2];
        private readonly Vector3[] elbowPositions = new Vector3[2];
        private readonly Vector3[] wristPositions = new Vector3[2];

        /// Rest position parameters for arm model (meters).
        private static readonly Vector3 ElbowPosition = new Vector3(0.195f, -0.5f, -0.075f);

        private static readonly Vector3 WristPosition = new Vector3(0.0f, 0.0f, 0.25f);
        private static readonly Vector3 ArmExtensionOffset = new Vector3(-0.13f, 0.14f, 0.08f);

        /// Strength of the acceleration filter (unitless).
        private const float GravityCalibStrength = 0.999f;

        /// Strength of the velocity suppression (unitless).
        private const float VelocityFilterSuppress = 0.99f;

        /// Strength of the velocity suppression during low acceleration (unitless).
        private const float LowAccelVelocitySuppress = 0.9f;

        /// Strength of the acceleration suppression during low velocity (unitless).
        private const float LowVelocityAccelSuppress = 0.5f;

        /// The minimum allowable accelerometer reading before zeroing (m/s^2).
        private const float MinAccel = 1.0f;

        /// The expected force of gravity (m/s^2).
        private const float GravityForce = 9.807f;

        /// Amount of normalized alpha transparency to change per second.
        private const float DeltaAlpha = 4.0f;

        /// Angle ranges the for arm extension offset to start and end (degrees).
        private const float MinExtensionAngle = 7.0f;

        private const float MaxExtensionAngle = 60.0f;

        /// Increases elbow bending as the controller moves up (unitless).
        private const float ExtensionWeight = 0.4f;

        /// Height of the elbow  (m).
        private float addedElbowHeight = 0.3f;

        /// Depth of the elbow  (m).
        private float addedElbowDepth = 0.3f;

        /// Determines if the accelerometer should be used.
        private bool useAccelerometer = false;

        private static readonly Quaternion FPoseLeft = new Quaternion(0.5f, -0.5f, -0.5f, 0.5f);
        private static readonly Quaternion FPoseRight = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);

        public override void ReadState(PlayerState outState)
        {
            base.ReadState(outState);

            if (outState.NodesState.GetState(FinchNodeType.LeftHand, FinchNodesStateType.Connected))
                HandleCalculations(FinchChirality.Left);
            if (outState.NodesState.GetState(FinchNodeType.RightHand, FinchNodesStateType.Connected))
                HandleCalculations(FinchChirality.Right);

            FillControllerState(outState);
        }

        /// <summary>
        /// Handle calculations of arm transforms. Returns false, if chirality is not corrected.
        /// </summary>
        /// <param name="chirality"></param>
        /// <returns></returns>
        private bool HandleCalculations(FinchChirality chirality)
        {
            int index = -1;
            FinchBone handBone = FinchBone.Last;

            // Multiplier for handedness such that 1 = Right, 0 = Center, -1 = left.
            Vector3 handedMultiplier = new Vector3(1, 1, 1);

            if (chirality == FinchChirality.Left)
            {
                index = 0;
                handBone = FinchBone.LeftHand;
                handedMultiplier.x = -handedMultiplier.x;
            }
            else if (chirality == FinchChirality.Right)
            {
                index = 1;
                handBone = FinchBone.RightHand;
            }
            else
                return false;

            Quaternion fpose = (chirality == FinchChirality.Left) ? FPoseLeft : FPoseRight;
            shoulderOrientations[index] = FinchCore.GetBoneRotation(FinchBone.Chest);
            Quaternion controllerOrientation = FinchCore.GetBoneRotation(handBone) * fpose;
            controllerOrientation = Quaternion.Inverse(shoulderOrientations[index]) * controllerOrientation;

            // Get the relative positions of the joints
            elbowPositions[index] = ElbowPosition + new Vector3(0.0f, addedElbowHeight, addedElbowDepth);
            elbowPositions[index] = Vector3.Scale(elbowPositions[index], handedMultiplier);
            wristPositions[index] = Vector3.Scale(WristPosition, handedMultiplier);
            Vector3 armExtensionOffset = Vector3.Scale(ArmExtensionOffset, handedMultiplier);

            // Extract just the x rotation angle
            Vector3 controllerForward = controllerOrientation * Vector3.forward;
            float xAngle = 90.0f - Vector3.Angle(controllerForward, Vector3.up);

            // Remove the z rotation from the controller
            Quaternion xyRotation = Quaternion.FromToRotation(Vector3.forward, controllerForward);

            // Offset the elbow by the extension
            float normalizedAngle = (xAngle - MinExtensionAngle) / (MaxExtensionAngle - MinExtensionAngle);
            float extensionRatio = Mathf.Clamp(normalizedAngle, 0.0f, 1.0f);
            if (!useAccelerometer)
            {
                elbowPositions[index] += armExtensionOffset * extensionRatio;
            }

            // Calculate the lerp interpolation factor
            float totalAngle = Quaternion.Angle(xyRotation, Quaternion.identity);
            float lerpSuppresion = 1.0f - Mathf.Pow(totalAngle / 180.0f, 6);
            float lerpValue = lerpSuppresion * (0.4f + 0.6f * extensionRatio * ExtensionWeight);

            // Apply the absolute rotations to the joints
            Quaternion lerpRotation = Quaternion.Lerp(Quaternion.identity, xyRotation, lerpValue);
            elbowOrientations[index] = shoulderOrientations[index] * Quaternion.Inverse(lerpRotation) * controllerOrientation;
            wristOrientations[index] = shoulderOrientations[index] * controllerOrientation;

            // Determine the relative positions
            elbowPositions[index] = shoulderOrientations[index] * elbowPositions[index];
            wristPositions[index] = elbowPositions[index] + elbowOrientations[index] * wristPositions[index];

            elbowOrientations[index] = elbowOrientations[index] * Quaternion.Inverse(fpose);
            wristOrientations[index] = wristOrientations[index] * Quaternion.Inverse(fpose);
            shoulderOrientations[index] = (chirality == FinchChirality.Left ? FinchCore.GetBoneRotation(FinchBone.LeftUpperArm) : FinchCore.GetBoneRotation(FinchBone.RightUpperArm));

            Vector3 headCoords = FinchCore.GetBoneCoordinate(FinchBone.Head);
            Vector3 chestCoords = FinchCore.GetBoneCoordinate(FinchBone.Chest);
            Vector3 headOffset = headCoords - chestCoords;

            elbowPositions[index] += headOffset;
            wristPositions[index] += headOffset;

            return true;
        }

        private void FillControllerState(PlayerState outState)
        {
            for (int index = 0; index < PlayerState.Bones.BoneCount; ++index)
            {
                FinchBone bone = PlayerState.Bones[index];
                switch (bone)
                {
                    case FinchBone.LeftUpperArm:
                    case FinchBone.LeftLowerArm:
                    case FinchBone.LeftHand:
                    case FinchBone.LeftHandCenter:
                    case FinchBone.RightUpperArm:
                    case FinchBone.RightLowerArm:
                    case FinchBone.RightHand:
                    case FinchBone.RightHandCenter:
                        {
                            outState.Rotations[index] = GetBoneRotation(bone);
                            outState.Positions[index] = GetBoneCoordinate(bone);
                            outState.Available[index] = true;
                            outState.BonesLengths[index] = FinchCore.GetBoneLength(bone);
                            break;
                        }
                    default:
                        {
                            outState.Rotations[index] = FinchCore.GetBoneRotation(bone);
                            outState.Positions[index] = FinchCore.GetBoneCoordinate(bone);
                            outState.Available[index] = FinchCore.IsBoneAvailable(bone);
                            outState.BonesLengths[index] = FinchCore.GetBoneLength(bone);
                            break;
                        }
                }
            }
        }

        private Quaternion GetBoneRotation(FinchBone bone)
        {
            switch (bone)
            {
                case FinchBone.LeftUpperArm:
                    return shoulderOrientations[0];
                case FinchBone.LeftLowerArm:
                    return elbowOrientations[0];
                case FinchBone.LeftHand:
                    return wristOrientations[0];
                case FinchBone.LeftHandCenter:
                    return wristOrientations[0];
                case FinchBone.RightUpperArm:
                    return shoulderOrientations[1];
                case FinchBone.RightLowerArm:
                    return elbowOrientations[1];
                case FinchBone.RightHand:
                    return wristOrientations[1];
                case FinchBone.RightHandCenter:
                    return wristOrientations[1];
                default:
                    return Quaternion.identity;
            }
        }

        private Vector3 GetBoneCoordinate(FinchBone bone)
        {
            switch (bone)
            {
                case FinchBone.LeftUpperArm:
                    return FinchCore.GetBoneCoordinate(FinchBone.LeftUpperArm);
                case FinchBone.LeftLowerArm:
                    return elbowPositions[0];
                case FinchBone.LeftHandCenter:
                    return wristPositions[0];
                case FinchBone.LeftHand:
                    return GetHandPositionByControllerPosition(wristPositions[0], wristOrientations[0], Vector3.left, FinchCore.GetBoneLength(FinchBone.LeftHand));
                case FinchBone.RightUpperArm:
                    return FinchCore.GetBoneCoordinate(FinchBone.RightUpperArm);
                case FinchBone.RightLowerArm:
                    return elbowPositions[1];
                case FinchBone.RightHand:
                    return GetHandPositionByControllerPosition(wristPositions[1], wristOrientations[1], Vector3.right, FinchCore.GetBoneLength(FinchBone.RightHand));
                case FinchBone.RightHandCenter:
                    return wristPositions[1];
                default:
                    return Vector3.zero;
            }
        }

        private Vector3 GetHandPositionByControllerPosition(Vector3 controllerPosition, Quaternion controllerRotation, Vector3 tPoseForward, float handBoneLength)
        {
            return controllerPosition - controllerRotation * tPoseForward * handBoneLength;
        }
    }
}