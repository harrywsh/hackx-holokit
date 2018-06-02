using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BlinkingRawImage : MonoBehaviour
{
    public Texture OffTexture;
    public Texture OnTexture;
    public float IntervalSec;

    private RawImage rawImage;
    private bool isOn = true;

    private System.Diagnostics.Stopwatch timer;

    void OnEnable()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogWarning("BlinkingRawImage script can't find RawImage component!");
            return;
        }
        rawImage.texture = OnTexture;

        timer = new System.Diagnostics.Stopwatch();
        timer.Start();
    }

    public void Update()
    {
        if (timer.ElapsedMilliseconds > (IntervalSec * 1000))
        {
            rawImage.texture = isOn ? OnTexture : OffTexture;
            isOn = !isOn;

            timer.Reset();
            timer.Start();
        }
    }

    private void OnDisable()
    {
        timer.Stop();
    }
}