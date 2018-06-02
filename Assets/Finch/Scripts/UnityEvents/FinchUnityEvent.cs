using UnityEngine;
using Finch;

public class FinchUnityEvent : MonoBehaviour
{
    public enum FinchButtonEventType
    {
        Right,
        Left,
        Any,
        BothAtOneTime
    }

    public bool Vibration;
    public uint VibrationTimeMs = 50;
    public FinchButtonEventType ButtonEventType = FinchButtonEventType.Any;

    protected FinchController LeftController;
    protected FinchController RightController;

    protected virtual void Start() { }

    protected virtual void Update()
    {
        RightController = FinchVR.RightController;
        LeftController = FinchVR.LeftController;
    }
}