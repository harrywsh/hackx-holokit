using Finch;

public class ButtonTwoUnityEvent : FinchButtonUnityEvent
{
    protected override void Start()
    {
        base.Start();
        button = FinchControllerElement.ButtonTwo;
    }
}