using UnityEngine;

namespace SamplerQuest.Physics
{
    public class BrickController : MonoBehaviour
    {
        [Header("Brick Settings")]
        [SerializeField] private string hitNote = "E4";
        [SerializeField] private float length = 1f;
        
        [Header("Note Randomization")]
        [SerializeField] private bool useRandomNote = false;
        [SerializeField] private string[] noteRange = new string[] { "C4", "D4", "E4", "F4", "G4", "A4", "B4", "C5" };
        [SerializeField] private bool useSequentialNotes = false;
        [SerializeField] private int sequentialIndex = 0;
        
        private static int globalSequentialIndex = 0;
        
        private void Start()
        {
            if (useRandomNote)
            {
                if (noteRange.Length > 0)
                {
                    if (useSequentialNotes)
                    {
                        // Use sequential notes from the range
                        hitNote = noteRange[globalSequentialIndex % noteRange.Length];
                        globalSequentialIndex++;
                        sequentialIndex = globalSequentialIndex;
                    }
                    else
                    {
                        // Randomly select from the range
                        hitNote = noteRange[Random.Range(0, noteRange.Length)];
                    }
                }
                else
                {
                    Debug.LogWarning($"BrickController {name}: Note range is empty, using default note {hitNote}");
                }
            }
            
            Debug.Log($"BrickController {name}: Initialized with note {hitNote}");
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                // Notify game manager about brick destruction
                GameManager.Instance?.OnBrickDestroyed(this);
                
                // Destroy the brick
                Destroy(gameObject);
            }
        }
        
        public float GetLength()
        {
            return length;
        }
        
        public string GetHitNote()
        {
            return hitNote;
        }
        
        public float GetNoteDuration()
        {
            // Convert brick length to note duration in seconds
            // You can adjust this formula based on your needs
            return length * 0.5f; // 0.5 seconds per unit of length
        }
        
        // Static method to reset sequential index (useful for new levels)
        public static void ResetSequentialIndex()
        {
            globalSequentialIndex = 0;
        }
    }
} 