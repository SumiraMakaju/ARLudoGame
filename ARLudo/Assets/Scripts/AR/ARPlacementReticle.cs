using UnityEngine;

namespace ARLudo.AR
{
    public class ARPlacementReticle : MonoBehaviour
    {
        public GameObject visualRoot;
        public float rotationSpeed = 30f;

        void Update()
        {
            if (visualRoot != null && visualRoot.activeSelf)
            {
                visualRoot.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (visualRoot != null)
            {
                visualRoot.SetActive(isVisible);
            }
        }
    }
}