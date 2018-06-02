using Finch;

public class ThumbButtonUnityEvent : FinchButtonUnityEvent
{
    protected override void Start()
    {
        base.Start();
        button = FinchControllerElement.ButtonThumb;
    }
}