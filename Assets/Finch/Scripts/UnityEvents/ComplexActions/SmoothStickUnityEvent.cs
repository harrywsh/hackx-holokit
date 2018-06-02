using UnityEngine.Events;
using Finch;
using UnityEngine;

/// <summary>
/// This event could be used for changing some value using stick up and down positions. For example, when stick position is up, event is invoked by timer. It is suitable for flipping, adding values, etc
/// </summary>
public class SmoothStickUnityEvent : FinchUnityEvent
{
    public UnityEvent StickUp;
    public UnityEvent StickDown;

    private Vector2 leftStick;
    private Vector2 rightStick;
    private float epsSwipe = 0.3f;
    private float firstIntervalChangingHeightSec = 0.3f;
    private float intervalChangingHeightSec = 0.1f;
    private float currentTime;
    private short timesCount = 0;

    protected override void Start()
    {
        if (StickUp == null)
            StickUp = new UnityEvent();
        if (StickDown == null)
            StickDown = new UnityEvent();
    }

    protected override void Update()
    {
        base.Update();
        leftStick = FinchVR.LeftController.GetTouchAxes();
        rightStick = FinchVR.RightController.GetTouchAxes();

        bool rightUp = rightStick.x >= -epsSwipe && rightStick.x <= epsSwipe && rightStick.y >= (1 - epsSwipe);
        bool rightDown = rightStick.x >= -epsSwipe && rightStick.x <= epsSwipe && rightStick.y <= -(1 - epsSwipe);
        bool leftUp = leftStick.x >= -epsSwipe && leftStick.x <= epsSwipe && leftStick.y >= (1 - epsSwipe);
        bool leftDown = leftStick.x >= -epsSwipe && leftStick.x <= epsSwipe && leftStick.y <= -(1 - epsSwipe);

        currentTime += Time.deltaTime;

        if (rightUp || leftUp)
        {
            if ((timesCount <= 1 && currentTime >= firstIntervalChangingHeightSec) || (timesCount > 1 && currentTime >= intervalChangingHeightSec))
            {
                StickUp.Invoke();

                if (Vibration)
                {
                    if (rightUp)
                        RightController.HapticPulse(VibrationTimeMs);
                    if (leftUp)
                        LeftController.HapticPulse(VibrationTimeMs);
                }

                currentTime = 0;
                ++timesCount;
            }
        }
        else if (rightDown || leftDown)
        {
            if ((timesCount <= 1 && currentTime >= firstIntervalChangingHeightSec) || (timesCount > 1 && currentTime >= intervalChangingHeightSec))
            {
                StickDown.Invoke();
                if (Vibration && rightDown)
                    RightController.HapticPulse(VibrationTimeMs);
                if (Vibration && leftDown)
                    LeftController.HapticPulse(VibrationTimeMs);

                currentTime = 0;
                ++timesCount;
            }
        }

        if (!(rightUp || leftUp) && !(rightDown || leftDown))
            timesCount = 0;
    }
}