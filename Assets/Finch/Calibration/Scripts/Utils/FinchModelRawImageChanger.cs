using UnityEngine;
using Finch;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class FinchModelRawImageChanger : MonoBehaviour
{
    public Texture BasicTexture;
    public Texture ProTexture;
    public Finch3DOFModel Controller3DOFModel = Finch3DOFModel.Basic;

    private RawImage targetRawImage;

    void OnEnable()
    {
        targetRawImage = GetComponent<RawImage>();
        if (targetRawImage == null)
        {
            Debug.LogWarning("FinchModelRawImageChanger script can't find RawImage component!");
            return;
        }
        targetRawImage.texture = Controller3DOFModel == Finch3DOFModel.Pro ? ProTexture : BasicTexture;
    }

    void Update()
    {
        if (targetRawImage != null)
        {
            if (targetRawImage.texture == BasicTexture && Controller3DOFModel == Finch3DOFModel.Pro)
                targetRawImage.texture = ProTexture;
            else if (targetRawImage.texture == ProTexture && Controller3DOFModel == Finch3DOFModel.Basic)
                targetRawImage.texture = BasicTexture;
        }
    }
}