using UnityEngine;
using System.Collections.Generic;

namespace SamplerQuest.Audio.Sampler
{
    public class SamplerKeyboardController : MonoBehaviour
    {
        [Header("Sample Settings")]
        [SerializeField] private SampleData sampleData;
        
        [Header("Keyboard Settings")]
        [SerializeField] private string startNote = "C4";
        [SerializeField] private int octaveRange = 2;
        
        private SamplerController samplerController;
        private Dictionary<KeyCode, string> keyToNoteMap = new Dictionary<KeyCode, string>();
        private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
        
        private void Awake()
        {
            samplerController = FindAnyObjectByType<SamplerController>();
            if (samplerController == null)
            {
                Debug.LogError("SamplerController not found in scene!");
                return;
            }
            
            InitializeKeyboard();
        }
        
        private void Start()
        {
            if (sampleData != null)
            {
                samplerController.LoadSample(sampleData);
            }
        }
        
        private void InitializeKeyboard()
        {
            // Define keyboard layout (QWERTY)
            KeyCode[] whiteKeys = {
                KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J,
                KeyCode.K, KeyCode.L, KeyCode.Semicolon, KeyCode.Quote
            };
            
            KeyCode[] blackKeys = {
                KeyCode.W, KeyCode.E, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.O, KeyCode.P
            };
            
            string[] whiteNotes = { "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F" };
            string[] blackNotes = { "C#", "D#", "F#", "G#", "A#", "C#", "D#" };
            
            // Create mapping for multiple octaves
            for (int octave = 0; octave < octaveRange; octave++)
            {
                int currentOctave = int.Parse(startNote.Substring(startNote.Length - 1)) + octave;
                
                // Map white keys
                for (int i = 0; i < whiteKeys.Length; i++)
                {
                    if (i < whiteNotes.Length)
                    {
                        keyToNoteMap[whiteKeys[i]] = $"{whiteNotes[i]}{currentOctave}";
                    }
                }
                
                // Map black keys
                for (int i = 0; i < blackKeys.Length; i++)
                {
                    if (i < blackNotes.Length)
                    {
                        keyToNoteMap[blackKeys[i]] = $"{blackNotes[i]}{currentOctave}";
                    }
                }
            }
        }
        
        private void Update()
        {
            if (samplerController == null) return;
            
            // Check for key presses
            foreach (var kvp in keyToNoteMap)
            {
                if (Input.GetKeyDown(kvp.Key) && !pressedKeys.Contains(kvp.Key))
                {
                    pressedKeys.Add(kvp.Key);
                    samplerController.PlayNote(sampleData.sampleName, kvp.Value);
                }
                else if (Input.GetKeyUp(kvp.Key) && pressedKeys.Contains(kvp.Key))
                {
                    pressedKeys.Remove(kvp.Key);
                    samplerController.StopNote(kvp.Value);
                }
            }
        }
    }
} 