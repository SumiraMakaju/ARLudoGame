using UnityEngine;
using System.Collections;
using ARLudo.Core;

namespace ARLudo.Visuals
{
    public class PawnVisual : MonoBehaviour
    {
        public float moveSpeed = 6f;
        public float hopHeight = 0.08f;
        public float hoverHeight = 0.04f;
        public float pulseSpeed = 6f;

        public LudoPawn Data { get; private set; }
        public bool IsMoving { get; private set; }

        private MeshRenderer[] meshRenderers;
        private Material[] materialInstances;
        private Color baseColor;
        private Color glowColor;
        private bool isSelectable = false;
        private Vector3 restingLocalPos;

        void Start()
        {
            restingLocalPos = transform.localPosition;
        }

        public void Initialize(LudoPawn data, Color baseUnityColor)
        {
            Data = data;
            baseColor = baseUnityColor;
            
            float h, s, v;
            Color.RGBToHSV(baseUnityColor, out h, out s, out v);
            glowColor = Color.HSVToRGB(h, s, 1f) * 2.5f;

            meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
            materialInstances = new Material[meshRenderers.Length];
            
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i] != null)
                {
                    materialInstances[i] = meshRenderers[i].material;
                    materialInstances[i].color = baseColor;
                    
                    if (materialInstances[i].HasProperty("_BaseColor"))
                    {
                        materialInstances[i].SetColor("_BaseColor", baseColor);
                    }
                    if (materialInstances[i].HasProperty("_Color"))
                    {
                        materialInstances[i].SetColor("_Color", baseColor);
                    }
                }
            }
        }

        void Update()
        {
            if (isSelectable && !IsMoving)
            {
                float bob = Mathf.PingPong(Time.time * 0.15f, hoverHeight);
                transform.localPosition = new Vector3(restingLocalPos.x, restingLocalPos.y + bob, restingLocalPos.z);

                float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
                Color currentGlow = Color.Lerp(baseColor, glowColor, pulse);

                if (materialInstances != null)
                {
                    foreach (var mat in materialInstances)
                    {
                        if (mat != null)
                        {
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", currentGlow);
                        }
                    }
                }
            }
        }

        public void SetHighlight(bool active)
        {
            isSelectable = active;

            if (!active)
            {
                transform.localPosition = restingLocalPos;
                if (materialInstances != null)
                {
                    foreach (var mat in materialInstances)
                    {
                        if (mat != null)
                        {
                            mat.SetColor("_EmissionColor", Color.black);
                            mat.DisableKeyword("_EMISSION");
                        }
                    }
                }
            }
        }

        public void TeleportTo(Vector3 position)
        {
            transform.position = position;
        }

        public void MoveTo(Vector3 worldTarget, System.Action onComplete = null)
        {
            if (IsMoving) return;
            StartCoroutine(HopTo(worldTarget, onComplete));
        }

        IEnumerator HopTo(Vector3 target, System.Action onComplete)
        {
            IsMoving = true;
            SetHighlight(false);

            if (ARLudoAudioManager.Instance != null)
            {
                ARLudoAudioManager.Instance.PlayHop();
            }

            Vector3 start = transform.position;
            float dist = Vector3.Distance(start, target);
            float duration = Mathf.Max(0.15f, dist / moveSpeed);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 current = Vector3.Lerp(start, target, t);
                current.y += Mathf.Sin(t * Mathf.PI) * hopHeight;
                transform.position = current;

                yield return null;
            }

            transform.position = target;
            restingLocalPos = transform.localPosition;
            IsMoving = false;

            onComplete?.Invoke();
        }
    }
}