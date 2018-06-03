using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Fading : MonoBehaviour
{
    public float Duration = 0.5f;
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogWarning("Fading script can't find RawImage component!");
            return;
        }
        Fade(0, 0);
        Appear();
    }

    public void Fade(float targetAlpha, float fadeDuration)
    {
        rawImage.CrossFadeAlpha(0, fadeDuration, false);
    }

    public void Fade()
    {
        Fade(1f, Duration);
    }

    public void Appear()
    {
        rawImage.CrossFadeAlpha(1, Duration, false);
    }
}