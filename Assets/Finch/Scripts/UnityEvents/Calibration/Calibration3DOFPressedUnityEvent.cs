using Finch;

public class Calibration3DOFPressedUnityEvent : FinchCalibrationUnityEvent
{
    protected override void Update()
    {
        base.Update();
        if ((ButtonEventType == FinchButtonEventType.Left || ButtonEventType == FinchButtonEventType.Any) && FinchVR.WasCalibratedLeft)
            FinchEvent.Invoke();

        if ((ButtonEventType == FinchButtonEventType.Right || ButtonEventType == FinchButtonEventType.Any) && FinchVR.WasCalibratedRight)
            FinchEvent.Invoke();
    }
}