using UnityEngine;

namespace SamplerQuest
{
    [RequireComponent(typeof(Rigidbody))]
    public class ForceImpulser : MonoBehaviour
    {
        [Header("Force Settings")]
        [SerializeField] private Vector3 forceDirection = Vector3.up;
        [SerializeField] private float forceMagnitude = 10f;
        [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

        [Header("Timing Settings")]
        [SerializeField] private float minInterval = 1f;
        [SerializeField] private float maxInterval = 3f;
        [SerializeField] private bool startOnAwake = true;

        private Rigidbody rb;
        private float nextImpulseTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("No Rigidbody component found!");
                enabled = false;
                return;
            }

            if (startOnAwake)
            {
                ScheduleNextImpulse();
            }
        }

        private void Update()
        {
            if (Time.time >= nextImpulseTime)
            {
                ApplyForce();
                ScheduleNextImpulse();
            }
        }

        private void ApplyForce()
        {
            if (rb != null)
            {
                Vector3 force = forceDirection.normalized * forceMagnitude;
                rb.AddForce(force, forceMode);
            }
        }

        private void ScheduleNextImpulse()
        {
            float interval = Random.Range(minInterval, maxInterval);
            nextImpulseTime = Time.time + interval;
        }

        public void ApplyForceNow()
        {
            ApplyForce();
            ScheduleNextImpulse();
        }

        private void OnValidate()
        {
            // Ensure intervals are valid
            if (minInterval > maxInterval)
            {
                maxInterval = minInterval;
            }

            // Ensure force direction is not zero
            if (forceDirection == Vector3.zero)
            {
                forceDirection = Vector3.up;
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw force direction
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, forceDirection.normalized * 2f);
        }
    }
} 