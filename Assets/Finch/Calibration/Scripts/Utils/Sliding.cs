using System.Collections;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    public enum RotationDirection
    {
        Left,
        Right
    }

    public RotationDirection Direction;
    public float RotationAngle = 20f;
    public float SlidingAmplitude = 0.1f;
    public GameObject Anchor;
    public Fading FadingScript;

    private bool isFreezing = false;
    private bool isWaiting = false;
    private bool fadeWasInited = false;

    private Vector3 startRotation, endRotation;
    private Vector3 startCoordinates, endCoordinates;

    private float currentT;
    private const float SlerpInterval = 2f;

    private void Start()
    {
        startRotation = transform.rotation.eulerAngles;
        float rotationZ = Direction == RotationDirection.Left ? startRotation.z + RotationAngle : startRotation.z - RotationAngle;
        endRotation = new Vector3(transform.rotation.x, transform.rotation.y, rotationZ);

        startCoordinates = Anchor.transform.position;
        float coordX = Direction == RotationDirection.Left ? startCoordinates.x - SlidingAmplitude : startCoordinates.x + SlidingAmplitude;
        endCoordinates = new Vector3(coordX, startCoordinates.y, startCoordinates.z);
    }

    private void OnEnable()
    {
        StartCoroutine(Rotate());
    }

    private void OnDisable()
    {
        StopCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            currentT += Time.deltaTime / SlerpInterval;
            if (isWaiting)
            {
                if (currentT >= 0.3f)
                {
                    isWaiting = false;
                    FadingScript.Appear();
                }
            }
            else
            {
                if (!isFreezing)
                {
                    if (currentT >= 1)
                    {
                        isFreezing = !isFreezing;
                        currentT = 0;
                        fadeWasInited = false;
                    }
                    else
                    {
                        transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, currentT);
                        transform.position = Vector3.Lerp(startCoordinates, endCoordinates, currentT);
                    }
                }
                else
                {
                    if (!fadeWasInited && currentT >= 0.2)
                    {
                        fadeWasInited = true;
                        FadingScript.Fade();
                    }
                    if (currentT >= 0.5)
                    {
                        isFreezing = false;
                        currentT = 0;
                        fadeWasInited = false;
                        isWaiting = true;
                    }
                }
            }
            yield return null;
        }
    }
}