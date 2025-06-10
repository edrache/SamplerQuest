using UnityEngine;

namespace SamplerQuest.Physics
{
    public class BallController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float initialSpeed = 5f;
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float speedIncrease = 0.5f;
        
        [Header("References")]
        [SerializeField] private SamplerQuest.Audio.Sampler.SamplerController samplerController;
        [SerializeField] private string wallHitNote = "D4";
        
        private Rigidbody rb;
        private Vector3 startPosition;
        private bool isMoving = false;
        private Vector3 lastVelocity;
        
        private void Awake()
        {
            Debug.Log("BallController: Awake called");
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                Debug.LogWarning("BallController: Rigidbody component was missing. Added automatically.");
            }
            
            // Configure Rigidbody
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            Debug.Log("BallController: Rigidbody configured");
        }
        
        private void Start()
        {
            Debug.Log("BallController: Start called");
            startPosition = transform.position;
            Debug.Log($"BallController: Start position set to {startPosition}");
        }
        
        private void Update()
        {
            if (!isMoving && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("BallController: Space pressed, launching ball");
                LaunchBall();
            }
        }
        
        private void FixedUpdate()
        {
            if (isMoving && rb != null)
            {
                lastVelocity = rb.linearVelocity;
                
                // Ensure minimum velocity
                if (rb.linearVelocity.magnitude < initialSpeed)
                {
                    Vector3 direction = rb.linearVelocity.normalized;
                    if (direction == Vector3.zero)
                    {
                        direction = new Vector3(Random.Range(-1f, 1f), 1f, 0f).normalized;
                    }
                    rb.linearVelocity = direction * initialSpeed;
                    Debug.Log($"BallController: Velocity corrected to {rb.linearVelocity.magnitude}");
                }
            }
        }
        
        private void LaunchBall()
        {
            if (rb == null)
            {
                Debug.LogError("BallController: Rigidbody is null!");
                return;
            }
            
            isMoving = true;
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1f, 0f).normalized;
            rb.linearVelocity = randomDirection * initialSpeed;
            Debug.Log($"BallController: Ball launched with velocity {rb.linearVelocity}");
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (rb == null)
            {
                Debug.LogError("BallController: Rigidbody is null during collision!");
                return;
            }
            
            Debug.Log($"BallController: Collision with {collision.gameObject.name}");
            
            if (collision.gameObject.CompareTag("Wall"))
            {
                Debug.Log("BallController: Wall collision detected");
                // Play wall hit sound
                if (samplerController != null)
                {
                    samplerController.PlayNote("default", wallHitNote);
                }
            }
            
            // Calculate reflection
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflection = Vector3.Reflect(lastVelocity, normal);
            
            // Apply reflection with speed increase
            float currentSpeed = Mathf.Min(lastVelocity.magnitude * (1f + speedIncrease), maxSpeed);
            rb.linearVelocity = reflection.normalized * currentSpeed;
            
            Debug.Log($"BallController: New velocity after collision: {rb.linearVelocity.magnitude}");
        }
        
        public void ResetBall()
        {
            Debug.Log("BallController: ResetBall called");
            if (rb == null)
            {
                Debug.LogWarning("BallController: Rigidbody is null during reset, attempting to get component");
                rb = GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                    Debug.LogWarning("BallController: Rigidbody component was missing. Added automatically.");
                }
            }
            
            isMoving = false;
            transform.position = startPosition;
            rb.linearVelocity = Vector3.zero;
            lastVelocity = Vector3.zero;
            Debug.Log($"BallController: Ball reset to position {startPosition}");
        }
    }
} 