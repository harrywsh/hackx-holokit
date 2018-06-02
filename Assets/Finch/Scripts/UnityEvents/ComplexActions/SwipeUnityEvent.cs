using UnityEngine.Events;
using Finch;
using UnityEngine;

public class SwipeUnityEvent : FinchUnityEvent
{
    public FinchChirality Chiraliy;

    public UnityEvent SwipeRight;
    public UnityEvent SwipeLeft;
    public UnityEvent SwipeUp;
    public UnityEvent SwipeDown;

    private float timeToSwapSec = 0.3f;
    private float currentTimeLeft;
    private float currentTimeRight;

    private bool swapInitiatedLeft;
    private bool swapInitiatedRight;
    private bool previousWasZeroLeft;
    private bool previousWasZeroRight;

    private float epsZero = 0.1f;
    private float epsSwipe = 0.3f;

    protected override void Start()
    {
        if (SwipeRight == null)
            SwipeRight = new UnityEvent();
        if (SwipeLeft == null)
            SwipeLeft = new UnityEvent();
        if (SwipeUp == null)
            SwipeUp = new UnityEvent();
        if (SwipeDown == null)
            SwipeDown = new UnityEvent();
    }

    protected override void Update()
    {
        base.Update();
        if (Chiraliy == FinchChirality.Both || Chiraliy == FinchChirality.Right)
            calculate(FinchChirality.Right);
        if (Chiraliy == FinchChirality.Both || Chiraliy == FinchChirality.Left)
            calculate(FinchChirality.Left);
    }

    private void calculate(FinchChirality chirality)
    {
        Vector2 axis;
        float currentTime;

        if (chirality == FinchChirality.Right)
        {
            axis = FinchVR.RightController.GetTouchAxes();
            currentTimeRight += Time.deltaTime;
            currentTime = currentTimeRight;
        }
        else if (chirality == FinchChirality.Left)
        {
            axis = FinchVR.LeftController.GetTouchAxes();
            currentTimeLeft += Time.deltaTime;
            currentTime = currentTimeLeft;
        }
        else
            return;

        //zero
        bool zero = axis.x <= epsZero && axis.x >= -epsZero && axis.y <= epsZero && axis.y >= -epsZero;
        //stick swipe to right-left
        bool swipeToRight = axis.x >= (1 - epsSwipe) && axis.y >= -epsSwipe && axis.y <= epsSwipe;
        bool swipeToLeft = axis.x <= -(1 - epsSwipe) && axis.y >= -epsSwipe && axis.y <= epsSwipe;

        //stick swipe to up-down
        bool swipeToUp = axis.x >= -epsSwipe && axis.x <= epsSwipe && axis.y >= (1 - epsSwipe);
        bool swipeToDown = axis.x >= -epsSwipe && axis.x <= epsSwipe && axis.y <= -(1 - epsSwipe);

        bool previousWasZero = chirality == FinchChirality.Right ? previousWasZeroRight : previousWasZeroLeft;

        if (zero)
        {
            if (chirality == FinchChirality.Right)
            {
                currentTimeRight = 0;
                previousWasZeroRight = true;
            }
            else
            {
                currentTimeLeft = 0;
                previousWasZeroLeft = true;
            }
            return;
        }

        if (currentTime >= timeToSwapSec)
        {
            currentTime = 0;
            if (chirality == FinchChirality.Right)
            {
                currentTimeRight = 0;
                previousWasZeroRight = previousWasZero = false;
            }
            else
            {
                currentTimeLeft = 0;
                previousWasZeroLeft = previousWasZero = false;
            }
        }

        if (previousWasZero)
        {
            bool invoked = false;
            if (swipeToRight)
            {
                SwipeRight.Invoke();
                invoked = true;
            }
            if (swipeToLeft)
            {
                SwipeLeft.Invoke();
                invoked = true;
            }
            if (swipeToUp)
            {
                SwipeUp.Invoke();
                invoked = true;
            }
            if (swipeToDown)
            {
                SwipeDown.Invoke();
                invoked = true;
            }

            if (invoked)
            {
                if (chirality == FinchChirality.Right)
                {
                    previousWasZeroRight = false;
                    currentTimeRight = 0;
                    if (Vibration)
                        RightController.HapticPulse(VibrationTimeMs);
                }
                else
                {
                    previousWasZeroLeft = false;
                    currentTimeLeft = 0;
                    if (Vibration)
                        LeftController.HapticPulse(VibrationTimeMs);
                }
            }
        }
    }
}