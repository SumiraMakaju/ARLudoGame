using System;
using System.Collections;
using UnityEngine;

namespace ARLudo.Dice
{
    public class DiceController : MonoBehaviour
    {
        public Transform throwPoint;
        public float throwForce = 0.3f;
        public float throwTorque = 2f;
        public float settleThreshold = 0.05f;
        public float settleDelay = 0.3f;

        public event Action<int> OnDiceResult;
        public event Action OnDiceThrown;
        public bool IsRolling { get; private set; }

        private Rigidbody rb;

        private readonly Vector3[] faceNormals = {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };
        private readonly int[] faceValues = { 1, 5, 3, 4, 2, 6 };

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void ThrowDice()
        {
            if (IsRolling) return;
            IsRolling = true;

            gameObject.SetActive(true);
            transform.position = throwPoint != null ? throwPoint.position : transform.position + Vector3.up * 0.05f;
            transform.rotation = UnityEngine.Random.rotation;

            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 force = Vector3.down * throwForce + UnityEngine.Random.insideUnitSphere * throwForce * 0.2f;
            rb.AddForce(force, ForceMode.Impulse);

            Vector3 torque = UnityEngine.Random.insideUnitSphere * throwTorque;
            rb.AddTorque(torque, ForceMode.Impulse);

            OnDiceThrown?.Invoke();
            StartCoroutine(WaitForSettle());
        }

        private IEnumerator WaitForSettle()
        {
            yield return new WaitForSeconds(0.5f);

            float stuckTimer = 0f;
            while (rb.angularVelocity.magnitude > settleThreshold || rb.linearVelocity.magnitude > settleThreshold)
            {
                stuckTimer += 0.1f;
                if (stuckTimer > 5f)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(settleDelay);

            rb.isKinematic = true;
            int result = ReadTopFace();
            IsRolling = false;
            OnDiceResult?.Invoke(result);
        }

        private int ReadTopFace()
        {
            float maxDot = -1f;
            int topFaceIndex = 0;

            for (int i = 0; i < faceNormals.Length; i++)
            {
                Vector3 worldNormal = transform.TransformDirection(faceNormals[i]);
                float dot = Vector3.Dot(worldNormal, Vector3.up);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    topFaceIndex = i;
                }
            }

            Debug.Log($"Dice result: {faceValues[topFaceIndex]}");
            return faceValues[topFaceIndex];
        }

        public void HideDice()
        {
            rb.isKinematic = true;
            gameObject.SetActive(false);
        }

        public void ShowDice()
        {
            gameObject.SetActive(true);
        }
    }
}