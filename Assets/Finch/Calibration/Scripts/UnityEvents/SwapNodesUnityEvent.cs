using UnityEngine;
using Finch;
using UnityEngine.Events;

public class SwapNodesUnityEvent : MonoBehaviour
{
    public UnityEvent SwapNodesEvent;

    private bool waitForRedefineEnd = false;

    void Start()
    {
        if (SwapNodesEvent == null)
            SwapNodesEvent = new UnityEvent();

        if (FinchVR.IsChiralityRedefining())
            waitForRedefineEnd = true;
    }

    void Update()
    {
        if (SwapNodesEvent != null)
        {
            if (!waitForRedefineEnd && FinchVR.IsChiralityRedefining())
            {
                waitForRedefineEnd = true;
                return;
            }

            if (waitForRedefineEnd && !FinchVR.IsChiralityRedefining())
            {
                waitForRedefineEnd = false;
                SwapNodesEvent.Invoke();
            }
        }
    }
}