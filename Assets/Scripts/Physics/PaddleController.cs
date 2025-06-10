using UnityEngine;

namespace SamplerQuest.Physics
{
    public class PaddleController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float maxDistance = 8f;
        
        [Header("References")]
        [SerializeField] private SamplerQuest.Audio.Sampler.SamplerController samplerController;
        [SerializeField] private string paddleHitNote = "C4";
        
        private float horizontalInput;
        private Vector3 startPosition;
        
        private void Start()
        {
            startPosition = transform.position;
        }
        
        private void Update()
        {
            // Get input from Rewired (placeholder for now)
            horizontalInput = Input.GetAxis("Horizontal");
            
            // Calculate new position
            float newX = transform.position.x + horizontalInput * moveSpeed * Time.deltaTime;
            newX = Mathf.Clamp(newX, -maxDistance, maxDistance);
            
            // Update position
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                // Play paddle hit sound
                if (samplerController != null)
                {
                    samplerController.PlayNote("default", paddleHitNote);
                }
            }
        }
        
        public void ResetPosition()
        {
            transform.position = startPosition;
        }
    }
} 