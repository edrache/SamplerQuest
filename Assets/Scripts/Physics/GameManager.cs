using UnityEngine;
using UnityEngine.Events;
using SamplerQuest.Audio.Sampler;

namespace SamplerQuest.Physics
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        [SerializeField] private int lives = 3;
        [SerializeField] private int score = 0;
        [SerializeField] private float ballSpeed = 10f;
        [SerializeField] private float paddleSpeed = 10f;
        
        [Header("References")]
        [SerializeField] private PaddleController paddle;
        [SerializeField] private BallController ball;
        [SerializeField] private BrickGrid brickGrid;
        [SerializeField] private SamplerController samplerController;
        [SerializeField] private NoteManager noteManager;
        [SerializeField] private SampleData defaultSample;
        
        [Header("Events")]
        public UnityEvent<int> onScoreChanged;
        public UnityEvent<int> onLivesChanged;
        public UnityEvent onGameOver;
        public UnityEvent onGameWon;
        
        private void Awake()
        {
            Debug.Log("GameManager: Awake called");
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("GameManager: Instance set");
            }
            else
            {
                Debug.LogWarning("GameManager: Instance already exists, destroying duplicate");
                Destroy(gameObject);
            }
            
            // Validate references
            if (paddle == null) Debug.LogError("GameManager: Paddle reference is missing!");
            if (ball == null) Debug.LogError("GameManager: Ball reference is missing!");
            if (brickGrid == null) Debug.LogError("GameManager: BrickGrid reference is missing!");
            if (samplerController == null) Debug.LogError("GameManager: SamplerController reference is missing!");
        }
        
        private void Start()
        {
            Debug.Log("GameManager: Start called");
            if (noteManager != null && samplerController != null)
            {
                noteManager.RegisterSampler(samplerController);
            }
            
            if (samplerController != null && defaultSample != null)
            {
                samplerController.LoadSample(defaultSample);
            }
            
            ResetGame();
        }
        
        private void OnDestroy()
        {
            if (noteManager != null && samplerController != null)
            {
                noteManager.UnregisterSampler(samplerController);
            }
        }
        
        public void OnBrickDestroyed(BrickController brick)
        {
            score += 10;
            onScoreChanged?.Invoke(score);
            Debug.Log($"GameManager: Brick destroyed, score increased to {score}");
            
            // Play brick hit sound
            if (samplerController != null && defaultSample != null)
            {
                string mappedNote = brick.GetHitNote();
                if (noteManager != null)
                {
                    mappedNote = noteManager.MapNoteToScale(mappedNote);
                }
                samplerController.PlayNote(defaultSample.sampleName, mappedNote);
            }
            
            if (brickGrid.GetActiveBrickCount() <= 1) // Last brick was just destroyed
            {
                Debug.Log("GameManager: All bricks destroyed, game won!");
                onGameWon?.Invoke();
            }
        }
        
        public void OnBallLost()
        {
            lives--;
            onLivesChanged?.Invoke(lives);
            Debug.Log($"GameManager: Ball lost, {lives} lives remaining");
            
            if (lives <= 0)
            {
                Debug.Log("GameManager: No lives remaining, game over!");
                onGameOver?.Invoke();
            }
            else
            {
                ResetBall();
            }
        }
        
        public void ResetGame()
        {
            Debug.Log("GameManager: Resetting game");
            score = 0;
            lives = 3;
            onScoreChanged?.Invoke(score);
            onLivesChanged?.Invoke(lives);
            
            ResetBall();
            brickGrid.ResetGrid();
            Debug.Log("GameManager: Game reset complete");
        }
        
        private void ResetBall()
        {
            Debug.Log("GameManager: Resetting ball");
            ball.ResetBall();
            paddle.ResetPosition();
        }
        
        public float GetBallSpeed() => ballSpeed;
        public float GetPaddleSpeed() => paddleSpeed;
    }
} 