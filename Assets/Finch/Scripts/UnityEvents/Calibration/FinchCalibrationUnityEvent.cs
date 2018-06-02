using UnityEngine.Events;

public class FinchCalibrationUnityEvent : FinchUnityEvent
{
    public UnityEvent FinchEvent;

    protected override void Start()
    {
        if (FinchEvent == null)
            FinchEvent = new UnityEvent();
    }

    protected override void Update()
    {
        base.Update();
    }
}