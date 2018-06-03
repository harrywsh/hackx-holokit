using UnityEngine;
using UnityEngine.Events;

public class TimerUnityEvent : MonoBehaviour
{
    public float TimeInSeconds;
    public UnityEvent TimerEvent;

    private static System.Diagnostics.Stopwatch stopWatch;

    void Start()
    {
        if (TimerEvent == null)
            TimerEvent = new UnityEvent();
    }

    void Update()
    {
        if (TimerEvent != null && stopWatch.ElapsedMilliseconds >= TimeInSeconds * 1000)
        {
            stopWatch.Stop();
            TimerEvent.Invoke();
        }
    }

    public void StartTimer()
    {
        stopWatch = new System.Diagnostics.Stopwatch();
        if (TimerEvent != null)
            stopWatch.Start();
    }
}