using UnityEngine;
using Finch;

public class HandsRecalibration : MonoBehaviour
{
    public GameObject[] ObjectsToShow;
    public AudioSource StartCalibrationSound;
    public AudioSource EndCalibrationSound;
    public AudioSource TickSound;

    public bool IsActivated { get; private set; }

    private System.Diagnostics.Stopwatch stopWatch;
    private const int Time = 1000;

    private int currentStep = 0;

    void Start()
    {
        foreach (GameObject go in ObjectsToShow)
            go.SetActive(false);
    }

    void Update()
    {
        if (IsActivated && stopWatch.ElapsedMilliseconds >= Time)
        {
            stopWatch.Stop();
            ObjectsToShow[currentStep].SetActive(false);
            ++currentStep;
            if (currentStep < ObjectsToShow.Length)
            {
                ObjectsToShow[currentStep].SetActive(true);

                if (TickSound != null)
                    TickSound.Play();

                if (currentStep == 3)
                    FinchVR.Calibrate(FinchChirality.Both);

                stopWatch.Reset();
                stopWatch.Start();
            }
            else
            {
                EndCalibration();
            }
        }
    }

    public void StartCalibration()
    {
        if (FinchSettings.DeviceType == FinchControllerType.Hand)
        {
            if (!IsActivated)
            {
                IsActivated = true;
                StartCalibrationSound.Play();
                currentStep = 0;
                ObjectsToShow[currentStep].SetActive(true);
                stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
            }
        }
    }

    public void EndCalibration()
    {
        IsActivated = false;
        EndCalibrationSound.Play();
    }
}