using UnityEngine;
using Finch;

public class FollowingWorld : MonoBehaviour
{
    public Transform FollowingObject;

    [Tooltip("Leave this field empty if you want to use data of UnityEngine.Camera.main")]
    public Transform[] ObjectsThatShouldBeNearCamera;

    [Tooltip("Objects that should be on specified place relatively to the head only on application start")]
    public Transform Environment;

    public float NormalEnvironmentY;

    private const float SlerpInterval = 0.6f;
    private const float DistanceHeadToFollowingObject = 2.2f;
    private Vector3 cameraPosChangedForAnchor;
    private float[] initialYContainersShifts;

    private void Start()
    {
        initialYContainersShifts = new float[ObjectsThatShouldBeNearCamera.Length];
        for (int i = 0; i < initialYContainersShifts.Length; ++i)
            initialYContainersShifts[i] = ObjectsThatShouldBeNearCamera[i].position.y;

        Environment.position = new Vector3(Environment.position.x, FinchVR.MainCamera.position.y + NormalEnvironmentY, Environment.position.z);
    }

    void Update()
    {
        Vector3 lerpResult = Vector3.Lerp(FollowingObject.position, Vector3.ProjectOnPlane(FinchVR.MainCamera.forward, Vector3.up).normalized * DistanceHeadToFollowingObject, Time.deltaTime / SlerpInterval);

        cameraPosChangedForAnchor = new Vector3(FinchVR.MainCamera.position.x, FollowingObject.position.y, FinchVR.MainCamera.position.z);
        FollowingObject.position = new Vector3(lerpResult.x, FinchVR.MainCamera.position.y, lerpResult.z);
        FollowingObject.LookAt(cameraPosChangedForAnchor);
        FollowingObject.Rotate(Vector3.up, 180);
        for (int i = 0; i < ObjectsThatShouldBeNearCamera.Length; ++i)
        {
            Vector3 v = new Vector3(ObjectsThatShouldBeNearCamera[i].rotation.eulerAngles.x, FinchVR.MainCamera.eulerAngles.y, ObjectsThatShouldBeNearCamera[i].rotation.eulerAngles.z);
            ObjectsThatShouldBeNearCamera[i].position = new Vector3(ObjectsThatShouldBeNearCamera[i].position.x, FinchVR.MainCamera.position.y + initialYContainersShifts[i], ObjectsThatShouldBeNearCamera[i].position.z);
            ObjectsThatShouldBeNearCamera[i].eulerAngles = v;
        }
    }
}