using UnityEngine;
using UnityEngine.UI;

public class WaitForHandsCalibrationStep : TutorialStep
{
    public RawImage ObjectToChangeRawImage;
    public Texture[] TexturesToChangeByTimer;
    public float IntervalSec = 1;
    public AudioSource CalibrationOK;
    public AudioSource TickSound;

    private System.Diagnostics.Stopwatch stopWatch;
    private float intervalMs;

    private int currentStep = 0;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (TexturesToChangeByTimer.Length > 0 && stopWatch.ElapsedMilliseconds > intervalMs)
        {
            stopWatch.Stop();
            ++currentStep;
            if (currentStep < TexturesToChangeByTimer.Length)
            {
                stopWatch.Reset();
                stopWatch.Start();
                ObjectToChangeRawImage.texture = TexturesToChangeByTimer[currentStep];

                if (TickSound != null)
                    TickSound.Play();

                if (currentStep == TexturesToChangeByTimer.Length - 1)
                    Finch.FinchVR.Calibrate(Finch.FinchChirality.Both);
            }
            else
            {
                CalibrationOK.Play();
                EndStepAndGoNext();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        if (TexturesToChangeByTimer.Length > 0)
        {
            intervalMs = IntervalSec * 1000;
            stopWatch = new System.Diagnostics.Stopwatch();
            ObjectToChangeRawImage.texture = TexturesToChangeByTimer[0];
            stopWatch.Start();
            currentStep = 0;
        }
    }
}