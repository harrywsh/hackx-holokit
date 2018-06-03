using Finch;

public class PressBoth3DOFCalibrationTutorialStep : PressCalibrationTutorialStep
{
    private bool leftWasPressed = false;
    private bool rightWasPressed = false;

    public void Update()
    {
        if (FinchVR.State.CalibrationButtonPressed[(int)FinchChirality.Right])
        {
            FinchVR.HapticPulse(FinchNodeType.RightHand, 50);
            rightWasPressed = true;
        }
        if (FinchVR.State.CalibrationButtonPressed[(int)FinchChirality.Left])
        {
            FinchVR.HapticPulse(FinchNodeType.LeftHand, 50);
            leftWasPressed = true;
        }

        if (leftWasPressed && rightWasPressed)
            EndStepAndGoNext();
    }
}