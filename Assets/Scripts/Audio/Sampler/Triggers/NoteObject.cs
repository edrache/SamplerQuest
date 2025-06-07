using UnityEngine;

namespace SamplerQuest.Audio.Sampler
{
    public class NoteObject : MonoBehaviour
    {
        [Header("Note Settings")]
        [SerializeField, Range(0f, 1f)] private float velocity = 1f;

        public float Velocity => velocity;

        private void Start()
        {
            Debug.Log($"[{gameObject.name}] Initialized with velocity: {velocity}");
            
            // Check if object has required components
            if (GetComponent<Collider>() == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing Collider component!");
            }
            
            if (GetComponent<Rigidbody>() == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Missing Rigidbody component! Object won't move physically.");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"[{gameObject.name}] Collision Enter with: {collision.gameObject.name}");
        }

        private void OnCollisionExit(Collision collision)
        {
            Debug.Log($"[{gameObject.name}] Collision Exit with: {collision.gameObject.name}");
        }
    }
} 