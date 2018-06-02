using System.Collections;
using UnityEngine;

public class Bouncing : MonoBehaviour
{
    public float Amplitude = 0.1f;

    private bool isMovingUp = true;
    private float startPositionY;
    private float endPositionY;
    private const float SlerpInterval = 0.5f;
    private float currentT;

    private void OnEnable()
    {
        startPositionY = transform.localPosition.y;
        endPositionY = startPositionY + Amplitude;
        currentT = 0;
        StartCoroutine(Bounce());
    }

    private void OnDisable()
    {
        StopCoroutine(Bounce());
    }


    void Start()
    {
        startPositionY = transform.localPosition.y;
        endPositionY = startPositionY + Amplitude;
        currentT = 0;
    }

    IEnumerator Bounce()
    {
        while (true)
        {
            if (isMovingUp)
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(startPositionY, endPositionY, currentT), transform.localPosition.z);
            else
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(endPositionY, startPositionY, currentT), transform.localPosition.z);

            currentT += Time.deltaTime / SlerpInterval;
            if (currentT >= 1)
            {
                isMovingUp = !isMovingUp;
                currentT = 0;
            }
            yield return null;
        }
    }
}