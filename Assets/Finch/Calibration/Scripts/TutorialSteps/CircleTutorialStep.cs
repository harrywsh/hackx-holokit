using UnityEngine;
using UnityEngine.UI;
using Finch;

public class CircleTutorialStep : TutorialStep
{
    public GameObject LookingCircle;
    public GameObject LookingCircleFiller;
    public RawImage LookingCircleRawImage;
    public Texture LookingCircleMaterial;
    public Texture LookingCircleReadyMaterial;
    public FollowingWorld FollowingWorldScript;
    public GameObject Aim;

    [Header("Audio")]
    public AudioSource CircleOK;

    public AudioSource CircleHover;
    public AudioSource LetsCalibrate;
    public AudioSource LookAtTheCircle;

    private const float TimeInterval = 0.01f;
    private const float ResizeInterval = 0.01f;
    private bool isPreviousLooking = false;
    private float currentResize = 0f;
    private float totalSize;

    void Start()
    {
        LetsCalibrate.PlayDelayed(0.5f);
        LookAtTheCircle.PlayDelayed(LookAtTheCircle.clip.length);

        totalSize = LookingCircle.transform.localScale.y;
    }

    public void Update()
    {
        base.CustomUpdate();

        RaycastHit seen;
        Ray raydirection = new Ray(FinchVR.MainCamera.position, FinchVR.MainCamera.forward);
        if (Physics.Raycast(raydirection, out seen, 3, 1) && seen.collider != null)
        {
            if (seen.collider.gameObject.tag.Equals("LookingCircle"))
            {
                Aim.SetActive(true);
                currentResize += ResizeInterval * Time.deltaTime / TimeInterval;
                if (currentResize >= totalSize)
                {
                    CircleOK.Play();
                    EndStepAndGoNext();
                    return;
                }
                if (!isPreviousLooking)
                {
                    CircleHover.Play();
                    isPreviousLooking = true;
                    currentResize = 0;
                    LookingCircleRawImage.texture = LookingCircleReadyMaterial;
                    LookingCircleFiller.transform.localScale = new Vector3(0, 0, LookingCircleFiller.transform.localScale.z);
                }

                LookingCircleFiller.transform.localScale = new Vector3(currentResize, currentResize, LookingCircleFiller.transform.localScale.z);
            }
            else
            {
                Aim.SetActive(false);
                if (isPreviousLooking)
                {
                    isPreviousLooking = false;
                    currentResize = 0;
                    LookingCircleRawImage.texture = LookingCircleMaterial;
                    LookingCircleFiller.transform.localScale = new Vector3(LookingCircleFiller.transform.localScale.x, 0, 0);
                }
            }
        }
        else
        {
            Aim.SetActive(false);
            if (isPreviousLooking)
            {
                isPreviousLooking = false;
                currentResize = 0;
                LookingCircleRawImage.texture = LookingCircleMaterial;
                LookingCircleFiller.transform.localScale = new Vector3(LookingCircleFiller.transform.localScale.x, 0, 0);
            }
        }
    }

    public override void EndStepAndGoNext()
    {
        FollowingWorldScript.enabled = false;
        Aim.SetActive(false);
        base.EndStepAndGoNext();
    }
}