using Finch;

public class ButtonGripUnityEvent : FinchButtonUnityEvent
{
    protected override void Start()
    {
        base.Start();
        button = FinchControllerElement.ButtonGrip;
    }
}