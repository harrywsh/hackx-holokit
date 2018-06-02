using Finch;

public class TouchUnityEvent : FinchButtonUnityEvent
{
    protected override void Start()
    {
        base.Start();
        button = FinchControllerElement.Touchpad;
    }
}