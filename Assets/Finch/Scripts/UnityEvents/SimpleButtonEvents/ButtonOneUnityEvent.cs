using Finch;

public class ButtonOneUnityEvent : FinchButtonUnityEvent
{
    protected override void Start()
    {
        base.Start();
        button = FinchControllerElement.ButtonOne;
    }
}