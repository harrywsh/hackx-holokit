using UnityEngine;
using Finch;

public class LookingAtTheCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(FinchVR.MainCamera);
    }
}