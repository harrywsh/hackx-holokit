using UnityEngine;
using Finch;

public class PressCalibrationTutorialStep : TutorialStep
{
    public AudioSource CalibrationOKAudio;

    public override void EndStepAndGoNext()
    {
        CalibrationOKAudio.Play();
        base.EndStepAndGoNext();
    }
}