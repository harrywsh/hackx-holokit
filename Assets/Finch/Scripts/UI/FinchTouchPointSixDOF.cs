using UnityEngine;
using Finch;

namespace Finch
{
    /// <summary>
    /// Visualizes touch point. Script owner GameObject must have MeshRenderer component and translates according to controller touchpos.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class FinchTouchPointSixDOF : MonoBehaviour
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
        private static readonly Vector3 TouchpadPointDimensions = new Vector3(8f, 0.1f, 8f);

        private const float TouchpadRadius = 11f;
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
                float y = TouchpadPointYOffset + TouchpadRadius * controller.GetTouchAxes().y;
                float z = 3.8f;
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

            elapsedScaleTimeSeconds += Time.deltaTime;
        }
    }
}