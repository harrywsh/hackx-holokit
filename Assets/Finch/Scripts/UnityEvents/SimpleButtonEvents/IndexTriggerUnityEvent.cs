using Finch;

public class IndexTriggerUnityEvent : FinchButtonUnityEvent
{
    protected override void Start()
    {
        base.Start();
        button = FinchControllerElement.IndexTrigger;
    }
}