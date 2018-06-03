using Finch;

public class Calibration6DOFPressedUnityEvent : FinchCalibrationUnityEvent
{
    protected override void Update()
    {
        base.Update();
        if (FinchVR.WasCalibratedRight && FinchVR.WasCalibratedLeft)
            FinchEvent.Invoke();
    }
}