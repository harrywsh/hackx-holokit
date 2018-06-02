using Finch;
using UnityEngine.Events;

public class FinchButtonUnityEvent : FinchUnityEvent
{
    public UnityEvent FinchDownEvent;
    public UnityEvent FinchUpEvent;

    protected FinchControllerElement button;

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


        bool wasInvoked = false;
        if (ButtonEventType == FinchButtonEventType.BothAtOneTime)
        {
            if ((RightController.GetPress(button) && LeftController.GetPressDown(button))
                || (LeftController.GetPress(button) && RightController.GetPressDown(button)))
            {
                if (FinchDownEvent != null)
                {
                    FinchDownEvent.Invoke();
                    wasInvoked = true;
                }
                else if (FinchUpEvent != null)
                {
                    FinchUpEvent.Invoke();
                    wasInvoked = true;
                }

                if (wasInvoked && Vibration)
                {
                    RightController.HapticPulse(VibrationTimeMs);
                    LeftController.HapticPulse(VibrationTimeMs);
                }
            }
        }
    }

    private void detectPress(FinchChirality chirality)
    {
        FinchController controller = chirality == FinchChirality.Right ? RightController : LeftController;
        FinchNodeType nodeType = chirality == FinchChirality.Right ? FinchNodeType.RightHand : FinchNodeType.LeftHand;
        bool wasInvoked = false;

        if (FinchDownEvent != null && controller.GetPressDown(button))
        {
            FinchDownEvent.Invoke();
            wasInvoked = true;
        }

        if (FinchUpEvent != null && RightController.GetPressUp(button))
        {
            FinchUpEvent.Invoke();
            wasInvoked = true;
        }

        if (Vibration && wasInvoked)
            FinchVR.HapticPulse(nodeType, VibrationTimeMs);
    }
}