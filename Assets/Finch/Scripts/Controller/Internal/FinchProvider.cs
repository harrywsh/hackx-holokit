using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Finch
{
    public class FinchProvider : IFinchProvider
    {
        protected const int LongPressTimeMs = 1000;
        protected const int CalibrationDelayTimeMs = 3000;

        protected static Stopwatch[] calibrationLongPressStopWatch;
        protected static Stopwatch handsCalibrationDelayStopWatch;
        private static bool[] calibrationButtonState;

        protected string errorDetails = string.Empty;

        public FinchProvider(FinchControllerType deviceType)
        {
            FinchInitError err = FinchCore.Init(deviceType);
            if (err != FinchInitError.None)
            {
                errorDetails = "Error creating/initializing Dash controller API: " + err;
                Debug.LogError(errorDetails);
                return;
            }
            FinchCore.LoadCalibrations();
            InitializeWatches();
            calibrationButtonState = new bool[(int)FinchChirality.Last];
        }

        public virtual void Exit()
        {
            FinchCore.Exit();
        }

        public virtual void ReadState(PlayerState outState)
        {
            if (!string.IsNullOrEmpty(errorDetails))
            {
                outState.ErrorDetails = errorDetails;
                Debug.LogError(errorDetails);
                return;
            }

            FinchUpdateError err = FinchUpdateError.NotInitialized;
            if (FinchSettings.HeadUpdateType == FinchHeadUpdateType.RotationUpdate)
                err = FinchCore.Update(FinchVR.MainCamera != null ? FinchVR.MainCamera.rotation.ToFinch() : Camera.main.transform.rotation.ToFinch());
            else if (FinchSettings.HeadUpdateType == FinchHeadUpdateType.RotationAndPositionUpdate)
                err = FinchCore.Update(FinchVR.MainCamera.rotation.ToFinch(), FinchVR.MainCamera.position.ToFinch());
            else
                err = FinchCore.Update();

            if (err != FinchUpdateError.None)
            {
                outState.ErrorDetails = "Error update Dash controller data: " + err;
                Debug.LogError(outState.ErrorDetails);
            }

            for (int i = 0; i < (int)FinchChirality.Last; ++i)
            {
                outState.ElementsBeginEvents[i] = FinchCore.GetEvents((FinchChirality)i, FinchEventType.Begin);
                outState.ElementsState[i] = FinchCore.GetEvents((FinchChirality)i, FinchEventType.Process);
                outState.ElementsEndEvents[i] = FinchCore.GetEvents((FinchChirality)i, FinchEventType.End);
                outState.IsTouching[i] = FinchCore.GetEvent((FinchChirality)i, FinchControllerElement.Touchpad, FinchEventType.Process);
                outState.TouchAxes[i] = FinchCore.GetTouchpadAxes((FinchChirality)i);
                outState.IndexTrigger[i] = FinchCore.GetIndexTrigger((FinchChirality)i);
            }

            for (int i = 0; i < (int)FinchNodeType.Last; ++i)
            {
                outState.Gyro[i] = FinchCore.GetBoneLocalAngularVelocity(PlayerState.Bones[(FinchNodeType)i]);
                outState.Accel[i] = FinchCore.GetBoneLocalAcceleration(PlayerState.Bones[(FinchNodeType)i]);
            }

            outState.NodesState = FinchCore.NodesState;

            if (FinchCore.ControllerType == FinchControllerType.Hand)
            {
                UpdateLongPressDetection(FinchControllerElement.Touchpad, true, outState);
                UpdateCalibrationDelay(outState);
            }
            else
                UpdateLongPressDetection(FinchControllerElement.ButtonZero, false, outState);
        }

        public void HapticPulse(FinchNodeType type, uint millisecond)
        {
            if (FinchCore.NodesState.GetState(type, FinchNodesStateType.Connected))
                FinchCore.HapticPulse(type, millisecond);
        }

        public void HapticPulse(FinchNodeType type, params VibrationPackage[] milliseconds)
        {
            FinchCore.HapticPulseByPattern(type, milliseconds);
        }

        public void ChangeDevice(FinchControllerType deviceType)
        {
            FinchCore.Init(deviceType);
        }

        public void ChangeBodyRotationMode(FinchBodyRotationMode bodyRotationMode)
        {
            FinchCore.BodyRotationMode = bodyRotationMode;
        }

        public void Calibrate(FinchChirality chirality, FinchRecenterMode recenterMode)
        {
            FinchCore.OnePoseAxisCalibration(chirality, recenterMode);
            Recenter(chirality, recenterMode);
            FinchCore.SaveCalibrations();
        }

        public void Recenter(FinchChirality chirality, FinchRecenterMode recenterMode)
        {
            FinchCore.Recenter(chirality, recenterMode);
        }

        public float GetBatteryCharge(FinchNodeType nodeType)
        {
            return FinchCore.GetNodeCharge(nodeType);
        }

        public void SwapNodes(FinchNodeType firstNode, FinchNodeType secondNode)
        {
            FinchCore.Interop.FinchNodeSwap(firstNode, secondNode);
        }

        public FinchControllerModel GetControllerModel(FinchNodeType nodeType)
        {
            string modelName = FinchCore.GetNodeModelNumber(nodeType);
            switch (modelName)
            {
                case "Dash":
                    return FinchControllerModel.Dash;
                case "Shift":
                    return FinchControllerModel.Shift;
                case "Dash M4":
                    return FinchControllerModel.DashM4;
                default:
                    return FinchControllerModel.Unknown;
            }
        }

        public void StartChiralityRedefine()
        {
            FinchCore.ChiralityRedefine(true);
        }

        public bool IsChiralityRedefining()
        {
            return FinchCore.IsChiralityRedefining;
        }

        public void SetBoneLength(FinchBone bone, float length)
        {
            FinchCore.SetBoneLength(bone, length);
        }

        protected static void InitializeWatches()
        {
            calibrationLongPressStopWatch = new Stopwatch[(int)FinchChirality.Last];
            handsCalibrationDelayStopWatch = new Stopwatch();

            for (FinchChirality i = 0; i < FinchChirality.Last; ++i)
                calibrationLongPressStopWatch[(int)i] = new Stopwatch();
        }

        protected static void UpdateCalibrationDelay(PlayerState outState)
        {
            for (FinchChirality i = 0; i < FinchChirality.Last; ++i)
                outState.CalibrationButtonPressed[(int)i] = false;

            if (calibrationButtonState[(int)FinchChirality.Left] && calibrationButtonState[(int)FinchChirality.Right])
            {
                if (!handsCalibrationDelayStopWatch.IsRunning)
                {
                    handsCalibrationDelayStopWatch.Reset();
                    handsCalibrationDelayStopWatch.Start();
                }
            }

            if (handsCalibrationDelayStopWatch.ElapsedMilliseconds >= CalibrationDelayTimeMs)
            {
                for (FinchChirality i = 0; i < FinchChirality.Last; ++i)
                    outState.CalibrationButtonPressed[(int)i] = true;

                handsCalibrationDelayStopWatch.Reset();
            }
        }

        protected static void UpdateLongPressDetection(FinchControllerElement element, bool handsDevice, PlayerState outState)
        {
            for (FinchChirality i = 0; i < FinchChirality.Last; ++i)
            {
                if (handsDevice)
                    calibrationButtonState[(int)i] = false;
                else
                    outState.CalibrationButtonPressed[(int)i] = false;

                if (FinchCore.GetEvent(i, element, FinchEventType.Begin))
                    calibrationLongPressStopWatch[(int)i].Start();

                if (FinchCore.GetEvent(i, element, FinchEventType.End))
                    calibrationLongPressStopWatch[(int)i].Reset();

                if (calibrationLongPressStopWatch[(int)i].ElapsedMilliseconds >= LongPressTimeMs)
                {
                    if (handsDevice)
                        calibrationButtonState[(int)i] = true;
                    else
                    {
                        outState.CalibrationButtonPressed[(int)i] = true;
                        calibrationLongPressStopWatch[(int)i].Reset();
                    }
                }
            }
        }
    }
}