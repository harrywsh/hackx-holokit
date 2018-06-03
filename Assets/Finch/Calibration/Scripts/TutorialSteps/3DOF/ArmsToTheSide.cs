using Finch;

public class ArmsToTheSide : TutorialStep
{
    void Update()
    {
        if ((FinchVR.RightController.GetPress(FinchControllerElement.ButtonOne) && FinchVR.LeftController.GetPressDown(FinchControllerElement.ButtonOne))
            || (FinchVR.LeftController.GetPress(FinchControllerElement.ButtonOne) && FinchVR.RightController.GetPressDown(FinchControllerElement.ButtonOne)))
        {
            FinchVR.Calibrate(FinchChirality.Left);
            FinchVR.Calibrate(FinchChirality.Right);
            FinchVR.StartChiralityRedefine();
        }
    }
}