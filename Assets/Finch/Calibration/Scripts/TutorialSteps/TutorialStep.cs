using UnityEngine;

public class TutorialStep : MonoBehaviour
{
    public GameObject[] TutorialObjects;
    public AudioSource AudioOnInitialize;

    [HideInInspector]
    public CalibrationManager CalbrationManager;

    void Start()
    {
        Initialize();
    }

    public virtual void CustomUpdate() { }

    public virtual void Initialize()
    {
        gameObject.SetActive(true);

        if (AudioOnInitialize != null)
            AudioOnInitialize.Play();

        foreach (GameObject go in TutorialObjects)
            go.SetActive(true);
    }

    public virtual void HideAll()
    {
        foreach (GameObject go in TutorialObjects)
            go.SetActive(false);
        gameObject.SetActive(false);
    }

    public virtual void EndStepAndGoNext()
    {
        CalbrationManager.NextStep();
    }
}