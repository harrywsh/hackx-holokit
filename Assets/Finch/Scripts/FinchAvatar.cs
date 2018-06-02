using UnityEngine;

namespace Finch
{
    public enum FingerMovement
    {
        None,
        OnlyRotations,
        Full
    }

    public enum ModelType
    {
        Default,
        Babe
    }

    /// <summary>
    /// Proceeds character skeletal model animation according to controller position. Allows to visualize not only controllers, but also head, body, arms etc.
    /// </summary>
    public class FinchAvatar : MonoBehaviour
    {
        ///<summary>
        /// Skeletal model animator. Every Update animator's transforms receive data from Finch devices.
        ///</summary>
        public Animator ModelAnimator;

        /// <summary>
        /// Camera transform. Matches FinchBone.Head position.
        /// </summary>
        public Transform Camera;

        /// <summary>
        /// If OnlyRotations is true, animator's transforms receive only rotations data, otherwise animator's transforms receive rotations and positions data.
        /// </summary>
        public bool OnlyRotations;

        /// <summary>
        /// Transform, which rotation affects to model head rotation.
        /// </summary>
        public Transform HeadVisualTransform;

        /// <summary>
        /// Type of finger movements
        /// </summary>
        public FingerMovement Fingers;

        /// <summary>
        /// Type of the model used, it affects bones calculations
        /// </summary>
        [Header("BoneCorrespondence")]
        public ModelType Model;

        protected PlayerState state;

        private static readonly FinchBone[] ChestLikeBones = new FinchBone[]
        {
            FinchBone.LeftUpperLeg,
            FinchBone.RightUpperLeg
        };

        protected void Update()
        {
            state = FinchVR.State;
            UpdateTransforms();
        }

        protected void UpdateTransforms()
        {
            if (Model == ModelType.Default)
            {
                for (int i = 0; i < PlayerState.Bones.BoneCount; ++i)
                    HandleBone(PlayerState.Bones[i], false, OnlyRotations);
            }
            else if (Model == ModelType.Babe)
            {
                for (int i = 0; i < PlayerState.Bones.BoneCount; ++i)
                {
                    FinchBone currentBone = PlayerState.Bones[i];
                    switch (currentBone)
                    {
                        case FinchBone.LeftShoulder:
                        case FinchBone.RightShoulder:
                            {
                                HandleBone(currentBone, false, OnlyRotations);
                                break;
                            }
                        case FinchBone.LeftThumbProximal:
                        case FinchBone.LeftThumbIntermediate:
                        case FinchBone.LeftThumbDistal:
                        case FinchBone.RightThumbProximal:
                        case FinchBone.RightThumbIntermediate:
                        case FinchBone.RightThumbDistal:
                            {
                                switch (Fingers)
                                {
                                    case FingerMovement.Full:
                                        {
                                            HandleBone(currentBone, false, OnlyRotations);
                                            break;
                                        }
                                    case FingerMovement.OnlyRotations:
                                        {
                                            HumanBodyBones avatarBone = (HumanBodyBones)currentBone;

                                            Transform boneTransform = ModelAnimator.GetBoneTransform(avatarBone);
                                            if (boneTransform == null)
                                                return;

                                            Quaternion boneRotation = state.GetRotation(currentBone);

                                            Quaternion babeCrutchThumbDelta = new Quaternion(-0.707107f, 0.0f, 0.0f, 0.707107f);
                                            boneTransform.rotation = GetRotationFromInnerCS(boneRotation) * babeCrutchThumbDelta;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                break;
                            }
                        default:
                            {
                                if (PlayerState.BoneDictionary.IsFingerBone(currentBone))
                                {
                                    switch (Fingers)
                                    {
                                        case FingerMovement.Full:
                                            HandleBone(currentBone, false, OnlyRotations);
                                            break;
                                        case FingerMovement.OnlyRotations:
                                            HandleBone(currentBone, false, true);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    HandleBone(currentBone, false, OnlyRotations);
                                }
                                break;
                            }
                    }
                }
            }

            foreach (FinchBone b in ChestLikeBones)
                HandleBone(b, true, OnlyRotations);

            Vector3 headLocalPosition;
            if (FinchSettings.DeviceType == FinchControllerType.Dash)
                headLocalPosition = state.GetPosition(FinchBone.Head);
            else
                headLocalPosition = state.GetPosition(FinchBone.RightEye); //Center position of both eyes

            Quaternion headLocalRotation = state.GetRotation(FinchBone.Head);

            if (FinchSettings.HeadUpdateType != FinchHeadUpdateType.RotationAndPositionUpdate)
            {
                if (Camera != null && !OnlyRotations)
                    Camera.transform.position = GetPositionFromInnerCS(headLocalPosition);
                if (HeadVisualTransform != null)
                    HeadVisualTransform.rotation = GetRotationFromInnerCS(headLocalRotation);
            }
            else
            {
                if (HeadVisualTransform != null)
                    HeadVisualTransform.rotation = GetRotationFromInnerCS(headLocalRotation);
            }
        }

        private void HandleBone(FinchBone bone, bool isChestLike, bool onlyRotation)
        {
            HumanBodyBones avatarBone = (HumanBodyBones)bone;
            if (avatarBone >= HumanBodyBones.LastBone)
                return;

            if (!isChestLike && PlayerState.Bones[bone] == -1)
                return;

            Transform boneTransform = ModelAnimator.GetBoneTransform(avatarBone);
            if (boneTransform == null)
                return;

            Quaternion boneRotation = (isChestLike) ? state.GetRotation(FinchBone.Chest) : state.GetRotation(bone);

            boneTransform.rotation = GetRotationFromInnerCS(boneRotation);
            if (!onlyRotation && !isChestLike)
                boneTransform.position = GetPositionFromInnerCS(state.GetPosition(bone));
        }

        private Quaternion GetRotationFromInnerCS(Quaternion innerCSRotation)
        {
            return transform.rotation * innerCSRotation;
        }

        private Vector3 GetPositionFromInnerCS(Vector3 innerCSPosition)
        {
            return transform.position + transform.lossyScale.x * (transform.rotation * innerCSPosition);
        }

        private float GetBoneAngleAboveHorizontInDegrees(FinchBone bone, FinchChirality chirality)
        {
            Quaternion rotation = state.GetRotation(bone);
            Vector3 localForward;
            switch (chirality)
            {
                case FinchChirality.Left:
                    {
                        localForward = Vector3.left;
                        break;
                    }
                case FinchChirality.Right:
                    {
                        localForward = Vector3.right;
                        break;
                    }
                default:
                    {
                        localForward = Vector3.forward;
                        break;
                    }
            }
            Vector3 forward = rotation * localForward;
            float cosAngle = Vector3.Dot(forward, Vector3.up);
            return Mathf.Asin(cosAngle) * Mathf.Rad2Deg; //cos alpha = sin 90 - alpha
        }

        private static float GetShoulderAngleInDegrees(float upperArmAngleInDegrees)
        {
            const float minimalUpperArmAngle = -15f;
            const float k = 3.1f;
            float result = (upperArmAngleInDegrees > minimalUpperArmAngle) ? k * Mathf.Sqrt(upperArmAngleInDegrees - minimalUpperArmAngle) : 0.0f;
            return result;
        }

        private static Quaternion GetShoulderRotation(FinchChirality chirality, float angle, PlayerState state)
        {
            float zRotSign = (chirality == FinchChirality.Right) ? 1 : -1;
            Quaternion dq = Quaternion.Euler(0f, 0f, zRotSign * angle);
            Quaternion result = state.GetRotation(FinchBone.Chest) * dq;
            return result;
        }
    }
}