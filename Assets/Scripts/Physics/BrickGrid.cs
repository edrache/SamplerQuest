using UnityEngine;
using System.Collections.Generic;

namespace SamplerQuest.Physics
{
    public class BrickGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int rows = 5;
        [SerializeField] private int columns = 8;
        [SerializeField] private float spacing = 0.2f;
        [SerializeField] private GameObject brickPrefab;
        
        [Header("Brick Notes")]
        [SerializeField] private string[] noteSequence = new string[] 
        {
            "C4", "D4", "E4", "F4", "G4", "A4", "B4", "C5"
        };
        
        private List<BrickController> activeBricks = new List<BrickController>();
        
        private void Start()
        {
            GenerateGrid();
        }
        
        private void GenerateGrid()
        {
            float startX = -(columns * (brickPrefab.transform.localScale.x + spacing)) / 2f;
            float startY = 4f; // Starting height
            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = new Vector3(
                        startX + col * (brickPrefab.transform.localScale.x + spacing),
                        startY - row * (brickPrefab.transform.localScale.y + spacing),
                        0f
                    );
                    
                    GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, transform);
                    BrickController brickController = brick.GetComponent<BrickController>();
                    
                    if (brickController != null)
                    {
                        // Assign note based on column
                        string note = noteSequence[col % noteSequence.Length];
                        // TODO: Set note through inspector or serialized field
                        
                        activeBricks.Add(brickController);
                    }
                }
            }
        }
        
        public void ResetGrid()
        {
            // Clear existing bricks
            foreach (var brick in activeBricks)
            {
                if (brick != null)
                {
                    Destroy(brick.gameObject);
                }
            }
            activeBricks.Clear();
            
            // Generate new grid
            GenerateGrid();
        }
        
        public int GetActiveBrickCount()
        {
            return activeBricks.Count;
        }
    }
} 