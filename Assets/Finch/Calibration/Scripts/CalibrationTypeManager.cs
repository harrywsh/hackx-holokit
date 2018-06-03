using UnityEngine;
using Finch;

public class CalibrationTypeManager : MonoBehaviour
{
    public GameObject OneControllerCalibration;
    public GameObject TwoControllersCalibration;
    private bool isFirstCall = true;

    void Update()
    {
        if (isFirstCall)
        {
            OneControllerCalibration.SetActive(FinchSettings.ControllersCount == FinchControllersCount.One);
            TwoControllersCalibration.SetActive(FinchSettings.ControllersCount != FinchControllersCount.One);
            isFirstCall = false;
        }
    }
}