using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MovingArms : MonoBehaviour
{
    public enum Direction
    {
        Clockwise,
        CounterClockwise
    }

    private enum AnimationState
    {
        ArmsMoving,
        ButtonAppearing,
        LineIncreasing,
        TextAppearing
    }

    public Direction RotationDirection;
    public SkinnedMeshRenderer HandRenderer;
    public Material GreenHandsMaterial;
    public Material WhiteHandsMaterial;

    public GameObject CalibrationButton;

    public GameObject LineGameObject;
    public Transform LineStart;
    public Transform LineEnd;

    public GameObject Hint;

    private AnimationState state = AnimationState.ArmsMoving;
    private LineRenderer line;
    private const float RotationMaxAngle = 56f;
    private float currentT;
    private const float InitialSlerpInterval = 2.5f;
    private float slerpInterval;
    private Vector3 startRotation, endRotation;

    private BlinkingMeshRenderer blinkingButton;

    private void Awake()
    {
        line = LineGameObject.GetComponent<LineRenderer>();
        if (line == null)
        {
            Debug.LogWarning("MovingArms script can't find LineRenderer component!");
            return;
        }

        blinkingButton = CalibrationButton.GetComponent<BlinkingMeshRenderer>();

        startRotation = transform.rotation.eulerAngles;
        endRotation = new Vector3(startRotation.x, startRotation.y + (RotationDirection == Direction.Clockwise ? RotationMaxAngle : -RotationMaxAngle), startRotation.z);
    }

    void OnEnable()
    {
        if (line != null)
        {
            ResetValues();
            StartCoroutine(Rotate());
        }
    }

    private void OnDisable()
    {
        StopCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (currentT <= 1)
        {
            currentT += Time.deltaTime / slerpInterval;
            if (currentT >= 1 && state == AnimationState.ArmsMoving)
            {
                ++state;
                currentT = 0;
                HandRenderer.material = GreenHandsMaterial;
            }
            else if (currentT >= 0.3f && state == AnimationState.ButtonAppearing)
            {
                if (blinkingButton != null)
                    blinkingButton.enabled = true;
                currentT = 0;
                state = AnimationState.LineIncreasing;
                slerpInterval = 0.5f;

                line.SetPosition(0, LineStart.position);
                line.SetPosition(1, Vector3.Slerp(LineStart.position, LineEnd.position, 0.1f));
                LineGameObject.SetActive(true);
            }
            else if (currentT >= 0.8f && state == AnimationState.LineIncreasing)
            {
                currentT = 0;
                state = AnimationState.TextAppearing;
            }

            if (state == AnimationState.ArmsMoving)
                transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, currentT);
            else if (state == AnimationState.LineIncreasing)
                line.SetPosition(1, Vector3.Slerp(LineStart.position, LineEnd.position, currentT));
            else if (state == AnimationState.TextAppearing)
            {
                Hint.transform.LookAt(Finch.FinchVR.MainCamera);
                Hint.SetActive(true);
                currentT = 2;
            }

            yield return null;
        }
    }

    private void ResetValues()
    {
        transform.eulerAngles = startRotation;
        HandRenderer.material = WhiteHandsMaterial;
        if (blinkingButton != null)
            blinkingButton.enabled = false;
        currentT = 0;
        state = AnimationState.ArmsMoving;
        slerpInterval = InitialSlerpInterval;

        LineGameObject.SetActive(false);
        Hint.SetActive(false);
    }
}