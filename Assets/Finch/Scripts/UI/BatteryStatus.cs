using UnityEngine;
using Finch;

public class BatteryStatus : MonoBehaviour
{
    private enum BatteryState
    {
        None,
        From0to25,
        From25to50,
        From50to75,
        From75to100
    }

    public MeshRenderer BatteryObject;

    [Header("Four materials from less to more charge")]
    public Material[] BatteryMaterials = new Material[4];

    private BatteryState currentState;
    private bool wasConnected = false;
    private const float EPS = 1.5f;

    void Start()
    {
        currentState = BatteryState.None;
        BatteryObject.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!wasConnected && FinchVR.MainController.IsHandNodeConnected())
        {
            float charge = FinchVR.GetBatteryCharge(FinchNodeType.RightHand);
            currentState = charge < 75 ? (charge < 50 ? (charge < 25 ? BatteryState.From0to25 : BatteryState.From25to50) : BatteryState.From50to75) : BatteryState.From75to100;
            SetMaterialByState();
            BatteryObject.gameObject.SetActive(true);
            wasConnected = true;
            return;
        }
        else if (wasConnected && !FinchVR.MainController.IsHandNodeConnected())
        {
            BatteryObject.gameObject.SetActive(false);
            currentState = BatteryState.None;
            wasConnected = false;
            return;
        }

        if (FinchVR.MainController.IsHandNodeConnected())
            CheckTransition();
    }

    private void CheckTransition()
    {
        float charge = FinchVR.GetBatteryCharge(FinchNodeType.RightHand);
        if (currentState == BatteryState.From0to25 && charge >= 25 + EPS)
        {
            currentState = BatteryState.From25to50;
            SetMaterialByState();
        }
        else if (currentState == BatteryState.From25to50)
        {
            if (charge >= 50 + EPS)
            {
                currentState = BatteryState.From50to75;
                SetMaterialByState();
            }
            else if (charge <= 25 - EPS)
            {
                currentState = BatteryState.From0to25;
                SetMaterialByState();
            }
        }
        else if (currentState == BatteryState.From50to75)
        {
            if (charge >= 75 + EPS)
            {
                currentState = BatteryState.From75to100;
                SetMaterialByState();
            }
            else if (charge <= 50 - EPS)
            {
                currentState = BatteryState.From25to50;
                SetMaterialByState();
            }
        }
        else if (currentState == BatteryState.From75to100 && charge <= 75 - EPS)
        {
            currentState = BatteryState.From50to75;
            SetMaterialByState();
        }
    }

    private void SetMaterialByState()
    {
        if (currentState == BatteryState.From0to25)
            BatteryObject.material = BatteryMaterials[0];
        else if (currentState == BatteryState.From25to50)
            BatteryObject.material = BatteryMaterials[1];
        else if (currentState == BatteryState.From50to75)
            BatteryObject.material = BatteryMaterials[2];
        else if (currentState == BatteryState.From75to100)
            BatteryObject.material = BatteryMaterials[3];
    }
}