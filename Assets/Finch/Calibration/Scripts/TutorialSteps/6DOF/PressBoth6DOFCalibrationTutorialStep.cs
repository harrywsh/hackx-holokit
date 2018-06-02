using UnityEngine;
using Finch;

public class PressBoth6DOFCalibrationTutorialStep : PressCalibrationTutorialStep
{
    public void Update()
    {
        if (FinchVR.State.CalibrationButtonPressed[(int)FinchChirality.Right] && FinchVR.State.CalibrationButtonPressed[(int)FinchChirality.Left])
        {
            FinchVR.HapticPulse(FinchNodeType.RightHand, 50);
            FinchVR.HapticPulse(FinchNodeType.LeftHand, 50);
            EndStepAndGoNext();
        }
    }
}