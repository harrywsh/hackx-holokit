using System.Collections;
using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Initializes Finch controllers correspondingly to the settings. Provides Finch Unity API
    /// </summary>
    public class FinchVR : MonoBehaviour
    {
        [Header("Build Settings")]
        [Tooltip("This settings are initilized once at application start")]
        public FinchDataSource DataSource = FinchDataSource.Controller;

        public FinchControllerType DeviceType;
        public FinchPoseTrackingAlgorithm PoseTrackingAlgorithm = FinchPoseTrackingAlgorithm.FinchVR;

        /// <summary>
        /// Instance of the Left Finch controller
        /// </summary>
        public static readonly FinchController LeftController = new FinchController(FinchChirality.Left);

        /// <summary>
        /// Instance of the Right Finch controller
        /// </summary>
        public static readonly FinchController RightController = new FinchController(FinchChirality.Right);

        /// <summary>
        /// Chirality of main controller depends on Preferred Handedness setting
        /// </summary>
        public static FinchController MainController { get; private set; }

        /// <summary>
        /// Chirality of optional controller depends on Preferred Handedness setting. If preferred handedness is Right, then optional controller will be Left and vice versa
        /// </summary>
        public static FinchController OptionalController { get; private set; }

        /// <summary>
        /// Keeps controller data from last update 
        /// </summary>
        public static readonly PlayerState State = new PlayerState();

        /// <summary>
        /// This event is raising every time data is updating
        /// </summary>
        public static event OnFinchUpdateEvent OnFinchUpdate;

        public delegate void OnFinchUpdateEvent();

        /// <summary>
        /// Represents information about was left Finch controller calibrated last frame or not
        /// </summary>
        public static bool WasCalibratedLeft;

        /// <summary>
        /// Represents information about was right Finch controller calibrated last frame or not
        /// </summary>
        public static bool WasCalibratedRight;

        /// <summary>
        /// Transform of the camera or object, representing movements of the head. If this field is null, Camera.main.transform will be used
        /// </summary>
        public static Transform MainCamera { get; private set; }

        private static bool wasCalibratedNextUpdateLeft, wasCalibratedNextUpdateRight;
        private static IFinchProvider finchProvider;
        private static bool tryToAssignControllersNextUpdate = true;

        private IEnumerator finchUpdate;
        private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private bool destroyByDuplicate = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                destroyByDuplicate = true;
                Destroy(gameObject);
            }

            FinchSettings.OnPreferredHandednessChange += OnPreferredHandednessUpdate;
            FinchSettings.InitBuildSettings(DataSource, DeviceType, PoseTrackingAlgorithm);

            if (finchProvider == null)
                finchProvider = FinchProviderFactory.CreateControllerProvider();

            AssignControllers(true);
        }

        private void Start()
        {
            if (finchProvider != null)
                finchProvider.ChangeBodyRotationMode(FinchSettings.BodyRotationMode);
        }

        private void LateUpdate()
        {
            if (wasCalibratedNextUpdateRight)
            {
                wasCalibratedNextUpdateRight = false;
                WasCalibratedRight = true;
            }
            else
                WasCalibratedRight = false;

            if (wasCalibratedNextUpdateLeft)
            {
                wasCalibratedNextUpdateLeft = false;
                WasCalibratedLeft = true;
            }
            else
                WasCalibratedLeft = false;
        }

        private static void UpdateFinch()
        {
            if (finchProvider != null)
                finchProvider.ReadState(State);

            switch (FinchSettings.DeviceType)
            {
                case FinchControllerType.Dash:
                    {
                        for (int i = 0; i < (int)FinchChirality.Last; ++i)
                            if (State.CalibrationButtonPressed[i])
                            {
                                Calibrate((FinchChirality)i);

                                if (FinchSettings.RecenterMode == FinchRecenterMode.Forward)
                                    UnityEngine.XR.InputTracking.Recenter();
                            }
                        break;
                    }
                case FinchControllerType.Shift:
                    {
                        if ((LeftController.GetPress(FinchControllerElement.ButtonZero) && RightController.GetPressDown(FinchControllerElement.ButtonZero))
                            || (RightController.GetPress(FinchControllerElement.ButtonZero) && LeftController.GetPressDown(FinchControllerElement.ButtonZero)))
                        {
                            Calibrate(FinchChirality.Right);
                            Calibrate(FinchChirality.Left);

                            if (FinchSettings.RecenterMode == FinchRecenterMode.Forward)
                                UnityEngine.XR.InputTracking.Recenter();
                        }
                        break;
                    }
            }

            if (tryToAssignControllersNextUpdate)
                tryToAssignControllersNextUpdate = !AssignControllers();
        }

        private void OnEnable()
        {
            finchUpdate = EndOfFrame();
            StartCoroutine(finchUpdate);
        }

        private void OnDisable()
        {
            if (destroyByDuplicate)
                return;

            StopCoroutine(finchUpdate);
        }

        private void OnDestroy()
        {
            if (destroyByDuplicate)
                return;

            FinchSettings.OnPreferredHandednessChange -= OnPreferredHandednessUpdate;

            if (finchProvider != null)
            {
                finchProvider.Exit();
                finchProvider = null;
            }
        }

        private IEnumerator EndOfFrame()
        {
            while (true)
            {
                // This must be done at the end of the frame to ensure that all GameObjects had a chance
                // to read transient controller state (e.g. events, etc) for the current frame before
                // it gets reset.
                yield return waitForEndOfFrame;
                UpdateFinch();

                if (OnFinchUpdate != null)
                    OnFinchUpdate();
            }
        }

        /// <summary>
        /// Returns FinchController of specified chirality
        /// </summary>
        /// <param name="chirality">Right or left</param>
        /// <returns></returns>
        public static FinchController GetFinchController(FinchChirality chirality)
        {
            switch (chirality)
            {
                case FinchChirality.Left:
                    return LeftController;
                case FinchChirality.Right:
                    return RightController;
                default:
                    return null;
            }
        }

        /// <summary>Gets state of nodes</summary>
        /// <returns>State of nodes</returns>
        public static FinchNodesState GetNodesState()
        {
            return State.NodesState;
        }

        /// <summary>Gets charge of the battery of specified physical device (percents)</summary>
        /// <param name="nodeType">Type of the node (physical device)</param>
        /// <returns>Charge of the battery of specified physical device (percents)</returns>
        public static float GetBatteryCharge(FinchNodeType nodeType)
        {
            if (finchProvider != null)
                return finchProvider.GetBatteryCharge(nodeType);
            return 0f;
        }

        /// <summary>
        /// Sends a command to the node to vibrate
        /// </summary>
        /// <param name="nodeType">Type of the node (physical device)</param>
        /// <param name="millisecond">Duration of vibration in milliseconds</param>
        public static void HapticPulse(FinchNodeType nodeType, uint millisecond)
        {
            if (finchProvider != null)
                finchProvider.HapticPulse(nodeType, millisecond);
        }

        /// <summary>
        /// Sends a command to the node to vibrate in specified pattern
        /// </summary>
        /// <param name="nodeType">Type of the node (physical device)</param>
        /// /// <param name="milliseconds">Vibration pattern. First parameter represents duration of vibration, second duration of period without vibration, third duration of vibration, etc. All durations are in milliseconds</param>
        public static void HapticPulse(FinchNodeType nodeType, params VibrationPackage[] milliseconds)
        {
            if (finchProvider != null)
                finchProvider.HapticPulse(nodeType, milliseconds);
        }

        /// <summary>
        /// Calibrates specified hand. FinchSettings.RecenterMode setting
        /// </summary>
        /// <param name="chirality">Right or left</param>
        public static void Calibrate(FinchChirality chirality)
        {
            if (finchProvider != null)
            {
                if (chirality == FinchChirality.Right)
                {
                    HapticPulse(FinchNodeType.RightHand, 50);
                    wasCalibratedNextUpdateRight = true;
                }
                else if (chirality == FinchChirality.Left)
                {
                    HapticPulse(FinchNodeType.LeftHand, 50);
                    wasCalibratedNextUpdateLeft = true;
                }
                finchProvider.Calibrate(chirality, FinchSettings.RecenterMode);
            }
        }

        /// <summary>
        /// Recenters arm of specified chirality using FinchSettings.RecenterMode setting
        /// </summary>
        /// <param name="chirality">Right or left</param>
        public static void Recenter(FinchChirality chirality)
        {
            if (finchProvider != null)
                finchProvider.Recenter(chirality, FinchSettings.RecenterMode);
        }

        /// <summary>Returns model of the connected controller</summary>
        /// <returns>Model of the connected controller</returns>
        public static FinchControllerModel GetControllerModel(FinchNodeType nodeType)
        {
            if (finchProvider != null)
                return finchProvider.GetControllerModel(nodeType);
            return FinchControllerModel.Unknown;
        }

        /// <summary>
        /// Starts the process of redefining chirality of 3DOF controllers
        /// </summary>
        public static void StartChiralityRedefine()
        {
            if (finchProvider != null)
                finchProvider.StartChiralityRedefine();
        }

        public static bool IsChiralityRedefining()
        {
            return finchProvider != null ? finchProvider.IsChiralityRedefining() : false;
        }

        /// <summary>
        /// Sets length of specified bone. Affects on FinchCore and settings that saved to the local PC/smartphone. Bones will be changed for all applications that use FinchCore
        /// </summary>
        /// <param name="bone">Specified bone of the body</param>
        /// <param name="lengthSm">Length of the bone in santimeters</param>
        public static void SetBoneLength(FinchBone bone, float lengthSm)
        {
            finchProvider.SetBoneLength(bone, lengthSm);
        }

        public static void SetCamera(Transform camera)
        {
            MainCamera = camera;
        }

        private static void OnPreferredHandednessUpdate(FinchChirality newHandedness)
        {
            AssignControllers();
        }

        private static bool AssignControllers(bool isOnAwake = false)
        {
            FinchChirality handedness = FinchSettings.PreferredHandedness;
            FinchControllersCount controllersCount = FinchSettings.ControllersCount;
            bool leftConnected = LeftController.IsHandNodeConnected();
            bool rightConnected = RightController.IsHandNodeConnected();

            if (handedness >= FinchChirality.Last)
            {
                FinchSettings.PreferredHandedness = handedness = FinchChirality.Right;
                Debug.Log("Setting \"Preferred Handedness\" was not set correctly, setting to default value \"Right\"");
            }

            if ((finchProvider != null) && !isOnAwake && (controllersCount == FinchControllersCount.One) && (leftConnected ^ rightConnected))
            {
                if ((handedness == FinchChirality.Left) && !leftConnected)
                    finchProvider.SwapNodes(FinchNodeType.RightHand, FinchNodeType.LeftHand);
                else if ((handedness == FinchChirality.Right) && !rightConnected)
                    finchProvider.SwapNodes(FinchNodeType.RightHand, FinchNodeType.LeftHand);
            }

            if (handedness == FinchChirality.Left)
            {
                MainController = LeftController;
                OptionalController = RightController;
            }
            else
            {
                MainController = RightController;
                OptionalController = LeftController;
            }
            return leftConnected || rightConnected;
        }
    }
}