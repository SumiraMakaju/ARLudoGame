using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARLudo.AR
{
    public class ARLightEstimator : MonoBehaviour
    {
        public ARCameraManager cameraManager;
        public Light targetDirectionalLight;

        void OnEnable()
        {
            if (cameraManager != null)
            {
                cameraManager.frameReceived += OnFrameReceived;
            }
        }

        void OnDisable()
        {
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnFrameReceived;
            }
        }

        void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (targetDirectionalLight == null) return;

            if (args.lightEstimation.averageBrightness.HasValue)
            {
                targetDirectionalLight.intensity = args.lightEstimation.averageBrightness.Value;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                targetDirectionalLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            }

            if (args.lightEstimation.colorCorrection.HasValue)
            {
                targetDirectionalLight.color = args.lightEstimation.colorCorrection.Value;
            }
        }
    }
}