using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BlinkingMeshRenderer : MonoBehaviour
{
    public Material OffMaterial;
    public Material OnMaterial;

    private MeshRenderer meshRenderer;
    private float currentT;
    private const float SlerpInterval = 0.8f;
    private bool isOn = true;

    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("BlinkingMeshRenderer script can't find MeshRenderer component!");
            return;
        }
        meshRenderer.material = OnMaterial;
        StartCoroutine(Blink());
    }

    private void OnDisable()
    {
        StopCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            currentT += Time.deltaTime / SlerpInterval;

            if (currentT >= 1)
            {
                if (isOn)
                {
                    meshRenderer.material = OffMaterial;
                    isOn = false;
                }
                else
                {
                    meshRenderer.material = OnMaterial;
                    isOn = true;
                }
                currentT = 0;
            }
            yield return null;
        }
    }
}