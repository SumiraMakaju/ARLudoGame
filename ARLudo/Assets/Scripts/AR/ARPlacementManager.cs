using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TMPro;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace ARLudo.AR
{
    public class ARPlacementManager : MonoBehaviour
    {
        public ARRaycastManager raycastManager;
        public ARPlaneManager planeManager;
        public ARAnchorManager anchorManager;
        public ARPlacementReticle reticle;
        public GameObject boardContainer;
        public Camera arCamera;
        public TMP_Text scanningPromptText;
        public GameObject repositionButton;

        public float minScale = 0.2f;
        public float maxScale = 1.5f;

        private bool isPlaced = false;
        private Pose currentPose;
        private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private float initialDistance;
        private Vector3 initialScale;
        private float initialAngle;
        private Quaternion initialRotation;

        public bool IsPlaced => isPlaced;

        void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        void Start()
        {
            if (boardContainer != null)
            {
                boardContainer.SetActive(false);
            }

            if (repositionButton != null)
            {
                repositionButton.SetActive(false);
            }

            UpdatePrompt("Move phone to scan any flat surface (table or floor)");
        }

        void Update()
        {
            if (!isPlaced)
            {
                UpdatePlacementPose();
                UpdateReticle();
                HandlePlacementInput();
            }
            else
            {
                HandleGestures();
            }
        }

        void UpdatePlacementPose()
        {
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            TrackableType types = TrackableType.PlaneWithinPolygon | TrackableType.PlaneWithinBounds;
            
            if (raycastManager.Raycast(screenCenter, hits, types))
            {
                currentPose = hits[0].pose;
                Vector3 cameraForward = arCamera.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                currentPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }

        void UpdateReticle()
        {
            if (hits.Count > 0)
            {
                reticle.transform.position = currentPose.position;
                reticle.transform.rotation = currentPose.rotation;
                reticle.SetVisible(true);
                UpdatePrompt("Surface detected! Tap screen to place board");
            }
            else
            {
                reticle.SetVisible(false);
                UpdatePrompt("Move phone to scan any flat surface (table or floor)");
            }
        }

        void HandlePlacementInput()
        {
            if (hits.Count == 0) return;

            if (Touch.activeTouches.Count == 1 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Vector2 touchPos = Touch.activeTouches[0].screenPosition;
                if (!IsPointerOverUI(touchPos))
                {
                    PlaceBoard();
                }
            }
        }

        void PlaceBoard()
        {
            boardContainer.transform.position = currentPose.position;
            boardContainer.transform.rotation = currentPose.rotation;
            boardContainer.SetActive(true);

            if (anchorManager != null)
            {
                ARAnchor anchor = anchorManager.AttachAnchor((ARPlane)hits[0].trackable, currentPose);
                if (anchor != null)
                {
                    boardContainer.transform.SetParent(anchor.transform, true);
                }
            }

            isPlaced = true;
            reticle.SetVisible(false);
            SetPlanesActive(false);

            if (scanningPromptText != null)
            {
                scanningPromptText.gameObject.SetActive(false);
            }

            if (repositionButton != null)
            {
                repositionButton.SetActive(true);
            }
        }

        void HandleGestures()
        {
            if (Touch.activeTouches.Count == 2)
            {
                var touch0 = Touch.activeTouches[0];
                var touch1 = Touch.activeTouches[1];

                if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Began || touch1.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    initialDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
                    initialScale = boardContainer.transform.localScale;

                    Vector2 diff = touch1.screenPosition - touch0.screenPosition;
                    initialAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    initialRotation = boardContainer.transform.rotation;
                }
                else if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Moved || touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    float currentDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
                    if (Mathf.Abs(initialDistance) > 0.001f)
                    {
                        float factor = currentDistance / initialDistance;
                        Vector3 targetScale = initialScale * factor;
                        float clamped = Mathf.Clamp(targetScale.x, minScale, maxScale);
                        boardContainer.transform.localScale = new Vector3(clamped, clamped, clamped);
                    }

                    Vector2 currentDiff = touch1.screenPosition - touch0.screenPosition;
                    float currentAngle = Mathf.Atan2(currentDiff.y, currentDiff.x) * Mathf.Rad2Deg;
                    float angleDelta = currentAngle - initialAngle;
                    boardContainer.transform.rotation = initialRotation * Quaternion.Euler(0, -angleDelta, 0);
                }
            }
        }

        public void RepositionBoard()
        {
            if (boardContainer.transform.parent != null && boardContainer.transform.parent.GetComponent<ARAnchor>() != null)
            {
                Destroy(boardContainer.transform.parent.gameObject);
                boardContainer.transform.SetParent(null);
            }

            boardContainer.SetActive(false);
            isPlaced = false;
            reticle.SetVisible(true);
            SetPlanesActive(true);

            if (scanningPromptText != null)
            {
                scanningPromptText.gameObject.SetActive(true);
            }

            if (repositionButton != null)
            {
                repositionButton.SetActive(false);
            }
        }

        void UpdatePrompt(string message)
        {
            if (scanningPromptText != null && !isPlaced)
            {
                scanningPromptText.text = message;
            }
        }

        void SetPlanesActive(bool active)
        {
            if (planeManager == null) return;
            planeManager.enabled = active;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(active);
            }
        }

        bool IsPointerOverUI(Vector2 screenPosition)
        {
            UnityEngine.EventSystems.PointerEventData eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventData.position = screenPosition;
            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);
            }
            return results.Count > 0;
        }
    }
}