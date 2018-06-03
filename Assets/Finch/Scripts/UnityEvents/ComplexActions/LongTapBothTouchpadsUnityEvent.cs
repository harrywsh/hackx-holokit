using UnityEngine.Events;
using Finch;
using UnityEngine;

public class LongTapBothTouchpadsUnityEvent : FinchUnityEvent
{
    public FinchControllerElement Button = FinchControllerElement.Touchpad;
    public UnityEvent LongTapBothTouchpads;

    private float longTapTimeMs = 1000;
    private System.Diagnostics.Stopwatch stopWatch;
    private bool bothTouching = false;
    private bool alreadyInvoked = false;

    protected override void Update()
    {
        ButtonEventType = FinchButtonEventType.Any;
        base.Update();

        bool startedTouching = !bothTouching &&
                               ((LeftController.GetPress(Button) && RightController.GetPressDown(Button))
                                || (RightController.GetPress(Button) && LeftController.GetPressDown(Button)));
        bool eventCanBeInvoked = !alreadyInvoked && bothTouching;
        bool stoppedTouching = bothTouching && (LeftController.GetPressUp(Button) || RightController.GetPressUp(Button));

        if (startedTouching)
        {
            bothTouching = true;
            stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
        }
        else if (eventCanBeInvoked)
        {
            if (stopWatch.ElapsedMilliseconds >= longTapTimeMs)
            {
                stopWatch.Stop();
                alreadyInvoked = true;
                LongTapBothTouchpads.Invoke();

                if (Vibration)
                {
                    RightController.HapticPulse(VibrationTimeMs);
                    LeftController.HapticPulse(VibrationTimeMs);
                }
            }
        }
        else if (stoppedTouching)
        {
            bothTouching = false;
            alreadyInvoked = false;
            stopWatch.Stop();
        }
    }
}