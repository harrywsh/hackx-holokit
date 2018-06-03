using UnityEngine;
using Finch;

public class AfterCalibrationTutorialStep : TutorialStep
{
    public TimerUnityEvent TimerObject;

    public AudioSource CalibrateDuringTheGame;

    public override void Initialize()
    {
        base.Initialize();
        CalibrateDuringTheGame.PlayDelayed(AudioOnInitialize.clip.length);
        TimerObject.StartTimer();
    }

    public void RestartCalibrationFromStepOne()
    {
        this.CalbrationManager.RestartFrom(1);
    }
}