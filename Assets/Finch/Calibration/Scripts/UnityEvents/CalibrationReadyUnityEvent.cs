using UnityEngine;
using UnityEngine.Events;

public class CalibrationReadyUnityEvent : MonoBehaviour
{
    public UnityEvent CalibrationReady;

    void Start()
    {
        if (CalibrationReady == null)
            CalibrationReady = new UnityEvent();
    }
}