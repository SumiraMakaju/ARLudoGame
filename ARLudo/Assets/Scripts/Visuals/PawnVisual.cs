using System.Collections;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.Visuals
{
    public class PawnVisual : MonoBehaviour
    {
        public LudoPawn Data { get; private set; }
        public bool IsMoving { get; private set; }
        public float hopDuration = 0.35f;
        public float hopHeight = 0.06f;

        private Renderer meshRenderer;

        public void Initialize(LudoPawn pawnData, Color color)
        {
            Data = pawnData;
            meshRenderer = GetComponentInChildren<Renderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = new Material(meshRenderer.material);
                meshRenderer.material.color = color;
            }
        }

        public void MoveTo(Vector3 destination, System.Action onComplete = null)
        {
            if (IsMoving) return;
            StartCoroutine(HopRoutine(destination, onComplete));
        }

        public void TeleportTo(Vector3 position) => transform.position = position;

        public void SetHighlight(bool on)
        {
            if (meshRenderer == null) return;
            if (on)
            {
                meshRenderer.material.EnableKeyword("_EMISSION");
                meshRenderer.material.SetColor("_EmissionColor", meshRenderer.material.color * 0.5f);
            }
            else
            {
                meshRenderer.material.DisableKeyword("_EMISSION");
                meshRenderer.material.SetColor("_EmissionColor", Color.black);
            }
        }

        private IEnumerator HopRoutine(Vector3 dest, System.Action onComplete)
        {
            IsMoving = true;
            Vector3 start = transform.position;
            float elapsed = 0f;
            while (elapsed < hopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hopDuration);
                Vector3 pos = Vector3.Lerp(start, dest, Mathf.SmoothStep(0, 1, t));
                pos.y += hopHeight * 4f * t * (1f - t);
                transform.position = pos;
                yield return null;
            }
            transform.position = dest;
            IsMoving = false;
            onComplete?.Invoke();
        }
    }
}