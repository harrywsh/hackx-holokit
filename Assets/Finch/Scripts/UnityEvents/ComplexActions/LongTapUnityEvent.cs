using Finch;
using UnityEngine.Events;

public class LongTapUnityEvent : FinchUnityEvent
{
    public UnityEvent LongTapEvent;

    private float longTapTimeMs = 1000;
    private System.Diagnostics.Stopwatch stopWatch;

    private readonly bool[] isPressed = new bool[2];
    private readonly bool[] alreadyInvoked = new bool[2];

    protected override void Update()
    {
        base.Update();
        if (ButtonEventType == FinchButtonEventType.Right || ButtonEventType == FinchButtonEventType.Any)
            DetectLongTap(ButtonEventType);

        if (ButtonEventType == FinchButtonEventType.Left)
            DetectLongTap(ButtonEventType);
    }

    private void DetectLongTap(FinchButtonEventType buttonEventType)
    {
        FinchController controller = buttonEventType == FinchButtonEventType.Right ? RightController : LeftController;
        int ind = buttonEventType == FinchButtonEventType.Right ? 0 : 1;

        if (controller.GetPress(FinchControllerElement.Touchpad))
        {
            if (!isPressed[ind])
            {
                isPressed[ind] = true;
                stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
            }
            else
            {
                if (!alreadyInvoked[ind] && stopWatch.ElapsedMilliseconds >= longTapTimeMs)
                {
                    stopWatch.Stop();
                    alreadyInvoked[ind] = true;
                    LongTapEvent.Invoke();

                    if (Vibration)
                        FinchVR.HapticPulse(ind == 0 ? FinchNodeType.RightHand : FinchNodeType.LeftHand, VibrationTimeMs);
                }
            }
        }
        else
        {
            if (isPressed[ind])
            {
                isPressed[ind] = alreadyInvoked[ind] = false;
                stopWatch.Stop();
            }
        }
    }
}