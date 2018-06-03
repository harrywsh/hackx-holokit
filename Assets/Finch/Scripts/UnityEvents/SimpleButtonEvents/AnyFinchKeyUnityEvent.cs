using Finch;
using UnityEngine.Events;

public class AnyFinchKeyUnityEvent : FinchUnityEvent
{
    public bool ExceptCalibrationButton = true;

    public UnityEvent FinchUpEvent;
    public UnityEvent FinchDownEvent;

    protected override void Start()
    {
        if (FinchUpEvent == null)
            FinchUpEvent = new UnityEvent();

        if (FinchDownEvent == null)
            FinchDownEvent = new UnityEvent();
    }

    protected override void Update()
    {
        base.Update();

        if (ButtonEventType == FinchButtonEventType.Right || ButtonEventType == FinchButtonEventType.Any)
            detectPress(FinchChirality.Right);

        if (ButtonEventType == FinchButtonEventType.Left || ButtonEventType == FinchButtonEventType.Any)
            detectPress(FinchChirality.Left);
    }

    private void detectPress(FinchChirality chirality)
    {
        FinchController controller = chirality == FinchChirality.Right ? RightController : LeftController;
        FinchNodeType nodeType = chirality == FinchChirality.Right ? FinchNodeType.RightHand : FinchNodeType.LeftHand;
        bool wasInvoked = false;

        if (FinchUpEvent != null
            && (!ExceptCalibrationButton && (FinchVR.State.CalibrationButtonPressed[(int)chirality] || FinchVR.WasCalibratedRight))
            || anyWasPressedUp(chirality, controller))
        {
            FinchUpEvent.Invoke();
            wasInvoked = true;
        }

        if (FinchDownEvent != null && anyWasPressedDown(FinchChirality.Right, controller))
        {
            FinchDownEvent.Invoke();
            wasInvoked = true;
        }

        if (Vibration && wasInvoked)
            FinchVR.HapticPulse(nodeType, VibrationTimeMs);
    }

    private bool anyWasPressedDown(FinchChirality chirality, FinchController controller)
    {
        for (int i = 0; i <= (int)FinchControllerElement.ButtonGrip; ++i)
            if (controller.GetPressDown((FinchControllerElement)i))
                return true;

        if (controller.GetPressDown(FinchControllerElement.ButtonThumb))
            return true;

        return false;
    }

    private bool anyWasPressedUp(FinchChirality chirality, FinchController controller)
    {
        for (int i = 0; i <= (int)FinchControllerElement.ButtonGrip; ++i)
            if (controller.GetPressUp((FinchControllerElement)i))
                return true;

        if (controller.GetPressUp(FinchControllerElement.ButtonThumb))
            return true;

        return false;
    }
}