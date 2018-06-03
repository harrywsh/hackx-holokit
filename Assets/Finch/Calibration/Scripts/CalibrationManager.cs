using UnityEngine;

public class CalibrationManager : MonoBehaviour
{
    public TutorialStep[] Steps;
    public CalibrationReadyUnityEvent CalibrationReady;

    private int currentStepInd;
    private bool isCalibrationEnded = false;

    void Start()
    {
        currentStepInd = 0;
        UnityEngine.XR.InputTracking.Recenter();
        foreach (TutorialStep ts in Steps)
            ts.CalbrationManager = this;

        Steps[currentStepInd].Initialize();
    }

    void Update()
    {
        if (currentStepInd < Steps.Length)
            Steps[currentStepInd].CustomUpdate();
    }

    public void NextStep()
    {
        if (!isCalibrationEnded)
        {
            Steps[currentStepInd].HideAll();
            currentStepInd++;
            if (currentStepInd >= Steps.Length)
            {
                CalibrationReady.CalibrationReady.Invoke();
                isCalibrationEnded = true;
                return;
            }
            Steps[currentStepInd].Initialize();
        }
    }

    public void RestartFrom(int indexOfStep) //starts from zero
    {
        Steps[currentStepInd].HideAll();
        currentStepInd = indexOfStep;
        Steps[currentStepInd].Initialize();
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}