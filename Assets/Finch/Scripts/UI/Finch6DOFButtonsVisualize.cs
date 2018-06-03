using UnityEngine;
using Finch;

/// <summary>
/// Visualizes Finch 6DOF (Shift) element pressings.
/// </summary>
public class Finch6DOFButtonsVisualize : MonoBehaviour
{
    public enum SixDOFType
    {
        WithStick,
        WithTouchpad
    }

    /// <summary>
    /// Controller chirality
    /// </summary>  
    public FinchChirality Chirality;

    /// <summary>
    /// Corresponding touchpoint visualizer.
    /// </summary>
    public FinchTouchPointSixDOF TouchPoint;

    /// <summary>
    /// 
    /// </summary>
    public SixDOFType Type;

    public GameObject ButtonOne;
    public GameObject ButtonTwo;
    public GameObject Calibration;
    public GameObject IndexTrigger;
    public GameObject GripButton;
    public GameObject Stick;

    public GameObject HighlightedButtonOne;
    public GameObject HighlightedButtonTwo;
    public GameObject HighlightedCalibration;
    public GameObject HighlightedIndexTrigger;
    public GameObject HighlightedGripButton;
    public GameObject HighlightedStick;

    private const float StickChangeCoeff = 18f;
    private const float IndexTriggerChangeCoeffWithStick = 18f;
    private const float IndexTriggerChangeCoeffWithTouchpad = 1f;
    private const float ButtonCoeff = 0.002f;
    private const float StickEps = 0.2f;
    private const float GripChangeCoeff = 0.00001f;

    private Vector3 transformButtonOne;
    private Vector3 transformButtonTwo;
    private Vector3 transformCalibration;
    private Vector3 transformIndexTrigger;
    private Vector3 transformGripButton;
    private Vector3 transformStickPosition;
    private Quaternion transformStickRotation;
    private Quaternion transformIndexRotation;
    private FinchController controller;

    void Awake()
    {
        transformButtonOne = ButtonOne.transform.localPosition;
        transformButtonTwo = ButtonTwo.transform.localPosition;
        transformCalibration = Calibration.transform.localPosition;
        transformIndexTrigger = IndexTrigger.transform.localPosition;
        transformGripButton = GripButton.transform.localPosition;
        transformStickRotation = Stick.transform.localRotation;
        transformIndexRotation = IndexTrigger.transform.localRotation;
        transformStickPosition = Stick.transform.localPosition;

        if (TouchPoint != null)
            TouchPoint.Chirality = Chirality;
    }

    void Update()
    {
        controller = Chirality == FinchChirality.Left ? FinchVR.LeftController : FinchVR.RightController;
        VisualizeButtons();
        VisualizeTriggers();
        VisualizeStick();
    }

    private void VisualizeButtons()
    {
        if (controller.GetPressUp(FinchControllerElement.ButtonZero))
        {
            HighlightedCalibration.transform.localPosition = transformCalibration;
            Calibration.SetActive(true);
            HighlightedCalibration.SetActive(false);
        }
        else if (controller.GetPressDown(FinchControllerElement.ButtonZero))
        {
            HighlightedCalibration.SetActive(true);
            Calibration.SetActive(false);
            HighlightedCalibration.transform.localPosition += new Vector3(0, 0, -ButtonCoeff);
        }

        if (controller.GetPressUp(FinchControllerElement.ButtonOne))
        {
            HighlightedButtonOne.transform.localPosition = transformButtonOne;
            ButtonOne.SetActive(true);
            HighlightedButtonOne.SetActive(false);
        }
        else if (controller.GetPressDown(FinchControllerElement.ButtonOne))
        {
            ButtonOne.SetActive(false);
            HighlightedButtonOne.SetActive(true);
            HighlightedButtonOne.transform.localPosition -= new Vector3(0, 0, ButtonCoeff);
        }

        if (controller.GetPressUp(FinchControllerElement.ButtonTwo))
        {
            HighlightedButtonTwo.transform.localPosition = transformButtonTwo;
            ButtonTwo.SetActive(true);
            HighlightedButtonTwo.SetActive(false);
        }
        else if (controller.GetPressDown(FinchControllerElement.ButtonTwo))
        {
            HighlightedButtonTwo.SetActive(true);
            ButtonTwo.SetActive(false);
            HighlightedButtonTwo.transform.localPosition -= new Vector3(0, 0, ButtonCoeff);
        }
    }

    private void VisualizeTriggers()
    {
        if (controller.GetPressUp(FinchControllerElement.ButtonGrip))
        {
            HighlightedGripButton.transform.localPosition = transformGripButton;
            GripButton.SetActive(true);
            HighlightedGripButton.SetActive(false);
        }
        else if (controller.GetPressDown(FinchControllerElement.ButtonGrip))
        {
            HighlightedGripButton.SetActive(true);
            GripButton.SetActive(false);
            HighlightedGripButton.transform.localPosition -= new Vector3(GripChangeCoeff, 0, 0);
        }

        if (controller.GetPressUp(FinchControllerElement.IndexTrigger))
        {
            HighlightedIndexTrigger.SetActive(false);
            IndexTrigger.SetActive(true);
        }
        else if (controller.GetPressDown(FinchControllerElement.IndexTrigger))
        {
            HighlightedIndexTrigger.SetActive(true);
            IndexTrigger.SetActive(false);
        }

        if (controller.GetPress(FinchControllerElement.IndexTrigger))
        {
            if (Type == SixDOFType.WithStick)
            {
                float tiltAroundXIndex = controller.GetIndexTrigger() * IndexTriggerChangeCoeffWithStick;
                Quaternion targetIndex = Quaternion.Euler(tiltAroundXIndex, 0, 0);
                HighlightedIndexTrigger.transform.localRotation = Quaternion.Slerp(transformIndexRotation, targetIndex, 1);
            }
            else if (Type == SixDOFType.WithTouchpad)
            {
                float tiltAroundXIndex = controller.GetIndexTrigger() * IndexTriggerChangeCoeffWithTouchpad;
                Quaternion targetIndex = Quaternion.Euler(tiltAroundXIndex, 0, 0);
                HighlightedIndexTrigger.transform.localRotation = Quaternion.Slerp(transformIndexRotation, targetIndex, 1);
            }
        }
    }

    private void VisualizeStick()
    {
        if (controller.GetPressUp(FinchControllerElement.ButtonThumb))
        {
            HighlightedStick.transform.localPosition = transformStickPosition;
            if (Mathf.Abs(controller.GetTouchAxes().x) < StickEps || Mathf.Abs(controller.GetTouchAxes().y) < StickEps)
            {
                Stick.SetActive(true);
                HighlightedStick.SetActive(false);
            }
        }
        else if (controller.GetPressDown(FinchControllerElement.ButtonThumb))
        {
            HighlightedStick.SetActive(true);
            Stick.SetActive(false);
            if (Type == SixDOFType.WithStick)
                HighlightedStick.transform.localPosition += new Vector3(0, 0, -ButtonCoeff);
        }

        if (Type == SixDOFType.WithStick)
        {
            if (Mathf.Abs(controller.GetTouchAxes().x) > StickEps || Mathf.Abs(controller.GetTouchAxes().y) > StickEps)
            {
                Stick.SetActive(false);
                HighlightedStick.SetActive(true);
                int Qq = -1;
                if (Chirality == FinchChirality.Right)
                    Qq *= -1;

                float tiltAroundX = controller.GetTouchAxes().x * StickChangeCoeff * Qq;
                float tiltAroundY = controller.GetTouchAxes().y * StickChangeCoeff;
                Quaternion target = Quaternion.Euler(tiltAroundY, tiltAroundX, 0);
                HighlightedStick.transform.localRotation = Quaternion.Slerp(transformStickRotation, target, 1);
            }
            else if (!controller.GetPress(FinchControllerElement.ButtonThumb))
            {
                Stick.SetActive(true);
                HighlightedStick.SetActive(false);
            }
        }
    }
}