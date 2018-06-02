using UnityEngine;
using Finch;

namespace Finch
{
    /// <summary>
    /// Visualizes touch point. Script owner GameObject must have MeshRenderer component and translates according to controller touchpos.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class FinchTouchPoint : MonoBehaviour
    {
        /// <summary>
        /// Controller chirality.
        /// </summary>
        public FinchChirality Chirality;

        /// <summary>
        /// Touchpad material.
        /// </summary>
        public Material TouchOpaque;

        // Units are in meters.
        private static readonly Vector3 TouchpadPointDimensions = new Vector3(0.014f, 0.0001f, 0.014f);

        private const float TouchpadRadius = 0.0121f;
        private const float TouchpadPointYOffset = 0.0475f;
        private const float TouchpadPointElevation = 0.0025f;
        private const float TouchpadPointScaleDurationSeconds = 0.15f;

        private Renderer touchRenderer;
        private float elapsedScaleTimeSeconds;
        private bool wasTouching;

        void Start()
        {
            touchRenderer = GetComponent<MeshRenderer>();
            elapsedScaleTimeSeconds = TouchpadPointScaleDurationSeconds;
        }

        void Update()
        {
            FinchController controller = FinchVR.GetFinchController(Chirality);

            if (touchRenderer == null || controller == null)
                return;

            touchRenderer.material = TouchOpaque;

            if (controller.IsTouching())
            {
                if (!wasTouching)
                {
                    wasTouching = true;
                    elapsedScaleTimeSeconds = 0.0f;
                }

                Vector3 scale = Vector3.Lerp(Vector3.zero,
                                             TouchpadPointDimensions,
                                             elapsedScaleTimeSeconds / TouchpadPointScaleDurationSeconds);

                transform.localScale = scale;

                float x = TouchpadRadius * controller.GetTouchAxes().x;
                float y = 0.01f;
                float z = TouchpadPointYOffset + TouchpadRadius * controller.GetTouchAxes().y;
                transform.localPosition = new Vector3(x, y, z);
            }
            else
            {
                if (wasTouching)
                {
                    wasTouching = false;
                    elapsedScaleTimeSeconds = 0.0f;
                }

                Vector3 scale = Vector3.Lerp(TouchpadPointDimensions,
                                             Vector3.zero,
                                             elapsedScaleTimeSeconds / TouchpadPointScaleDurationSeconds);

                transform.localScale = scale;
            }

            if (controller.GetPress(FinchControllerElement.ButtonThumb))
            {
                Vector3 scale = TouchpadPointDimensions;
                scale.x = TouchpadRadius * 4;
                scale.z = TouchpadRadius * 4;

                transform.localScale = scale;

                float x = 0f;
                float y = 0.01f;
                float z = TouchpadPointYOffset;
                transform.localPosition = new Vector3(x, y, z);
            }

            elapsedScaleTimeSeconds += Time.deltaTime;
        }
    }
}