using UnityEngine;
using System.Collections;

namespace HoloKit
{
    public class HoloKitAirplay : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            Display.onDisplaysUpdated += OnDisplayUpdated;

        }

        public void OnDisplayUpdated()
        {
            if (Display.displays.Length > 1)
            {
                Display secondDisplay = Display.displays[1];
                secondDisplay.SetRenderingResolution(Display.main.renderingWidth, Display.main.renderingHeight);
                GetComponent<Camera>().SetTargetBuffers(secondDisplay.colorBuffer, secondDisplay.depthBuffer);
            }
            else
            {
                GetComponent<Camera>().SetTargetBuffers(Display.main.colorBuffer, Display.main.depthBuffer);

                //cameraCenter.SetTargetBuffers(Display.main.colorBuffer, Display.main.depthBuffer);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}