using UnityEngine;

namespace SamplerQuest.Physics
{
    public class BrickController : MonoBehaviour
    {
        [Header("Brick Settings")]
        [SerializeField] private string hitNote = "E4";
        [SerializeField] private float length = 1f;
        
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
    }
} 