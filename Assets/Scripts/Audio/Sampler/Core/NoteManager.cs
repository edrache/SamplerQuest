using UnityEngine;
using System.Collections.Generic;

namespace SamplerQuest.Audio.Sampler
{
    public class NoteManager : MonoBehaviour
    {
        [System.Serializable]
        public class Note
        {
            public string name;
            public int octave;
            public float frequency;
        }

        [System.Serializable]
        public class Scale
        {
            public string name;
            public int[] intervals; // Intervals in semitones from root note
        }

        [Header("Note Settings")]
        [SerializeField] private string rootNote = "C4";
        [SerializeField] private int baseOctave = 4;
        
        [Header("Scale Settings")]
        [SerializeField] private Scale currentScale;
        [SerializeField] private List<Scale> availableScales = new List<Scale>();
        [SerializeField] private ScaleType scaleType = ScaleType.Major;
        [SerializeField] private bool mapToHigherNote = true; // If true, map to higher note, if false, map to lower note

        [Header("Sampler Settings")]
        [SerializeField] private List<SamplerController> connectedSamplers = new List<SamplerController>();

        private Dictionary<string, float> noteFrequencies = new Dictionary<string, float>();
        private List<Note> currentScaleNotes = new List<Note>();

        private void Awake()
        {
            InitializeNoteFrequencies();
            InitializeScales();
            UpdateCurrentScale();
        }

        private void InitializeNoteFrequencies()
        {
            // A4 = 440Hz
            float a4 = 440f;
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            
            for (int octave = 0; octave < 8; octave++)
            {
                for (int i = 0; i < 12; i++)
                {
                    float frequency = a4 * Mathf.Pow(2f, (octave - 4) + (i - 9) / 12f);
                    string noteName = $"{noteNames[i]}{octave}";
                    noteFrequencies[noteName] = frequency;
                }
            }
        }

        private void InitializeScales()
        {
            // Major scale
            availableScales.Add(new Scale
            {
                name = "Major",
                intervals = new int[] { 0, 2, 4, 5, 7, 9, 11 }
            });

            // Minor scale
            availableScales.Add(new Scale
            {
                name = "Minor",
                intervals = new int[] { 0, 2, 3, 5, 7, 8, 10 }
            });

            // Pentatonic scale
            availableScales.Add(new Scale
            {
                name = "Pentatonic",
                intervals = new int[] { 0, 2, 4, 7, 9 }
            });

            // Set initial scale based on scaleType
            currentScale = availableScales.Find(s => s.name == scaleType.ToString());
            if (currentScale == null)
            {
                currentScale = availableScales[0]; // Default to Major if not found
            }
        }

        public void SetRootNote(string note, int octave)
        {
            rootNote = note;
            baseOctave = octave;
            UpdateCurrentScale();
        }

        public void SetScale(ScaleType type)
        {
            Debug.Log($"Setting scale to: {type}");
            scaleType = type;
            currentScale = availableScales.Find(s => s.name == type.ToString());
            if (currentScale == null)
            {
                Debug.LogWarning($"Scale {type} not found, defaulting to Major");
                currentScale = availableScales[0]; // Default to Major if not found
            }
            UpdateCurrentScale();
        }

        public void SetMapDirection(bool mapToHigher)
        {
            mapToHigherNote = mapToHigher;
        }

        public string MapNoteToScale(string inputNote)
        {
            if (string.IsNullOrEmpty(inputNote)) return inputNote;

            // Extract note name and octave
            string noteName = inputNote.Substring(0, inputNote.Length - 1);
            int octave = int.Parse(inputNote.Substring(inputNote.Length - 1));

            // Find the note in the current scale
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            int inputNoteIndex = System.Array.IndexOf(noteNames, noteName);

            if (inputNoteIndex == -1)
            {
                Debug.LogError($"Invalid input note: {noteName}");
                return inputNote;
            }

            // Find the nearest note in the scale
            int nearestNoteIndex = -1;
            int minDistance = int.MaxValue;

            foreach (var note in currentScaleNotes)
            {
                string scaleNoteName = note.name.Substring(0, note.name.Length - 1);
                int scaleNoteIndex = System.Array.IndexOf(noteNames, scaleNoteName);
                
                int distance = mapToHigherNote ? 
                    (scaleNoteIndex - inputNoteIndex + 12) % 12 : 
                    (inputNoteIndex - scaleNoteIndex + 12) % 12;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNoteIndex = scaleNoteIndex;
                }
            }

            if (nearestNoteIndex == -1)
            {
                Debug.LogWarning($"Could not find nearest note for {inputNote} in current scale");
                return inputNote;
            }

            string mappedNote = $"{noteNames[nearestNoteIndex]}{octave}";
            Debug.Log($"Mapped note {inputNote} to {mappedNote} in scale {currentScale.name}");
            return mappedNote;
        }

        public void RegisterSampler(SamplerController sampler)
        {
            if (sampler != null && !connectedSamplers.Contains(sampler))
            {
                connectedSamplers.Add(sampler);
            }
        }

        public void UnregisterSampler(SamplerController sampler)
        {
            if (sampler != null)
            {
                connectedSamplers.Remove(sampler);
            }
        }

        public List<SamplerController> GetConnectedSamplers()
        {
            return new List<SamplerController>(connectedSamplers);
        }

        private void UpdateCurrentScale()
        {
            Debug.Log($"Updating current scale. Root note: {rootNote}, Base octave: {baseOctave}");
            currentScaleNotes.Clear();
            int rootIndex = GetNoteIndex(rootNote);
            
            if (rootIndex == -1)
            {
                Debug.LogError($"Invalid root note: {rootNote}");
                return;
            }

            Debug.Log($"Root index: {rootIndex}, Scale intervals: {string.Join(", ", currentScale.intervals)}");
            
            foreach (int interval in currentScale.intervals)
            {
                int noteIndex = (rootIndex + interval) % 12;
                int octaveOffset = (rootIndex + interval) / 12;
                string noteName = GetNoteName(noteIndex);
                int octave = baseOctave + octaveOffset;
                
                string fullNoteName = $"{noteName}{octave}";
                Debug.Log($"Adding note to scale: {fullNoteName} (index: {noteIndex}, octave: {octave})");
                
                currentScaleNotes.Add(new Note
                {
                    name = fullNoteName,
                    octave = octave,
                    frequency = noteFrequencies[fullNoteName]
                });
            }
        }

        private int GetNoteIndex(string note)
        {
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            string noteName = note.Substring(0, note.Length - 1); // Remove octave
            int index = System.Array.IndexOf(noteNames, noteName);
            if (index == -1)
            {
                Debug.LogError($"Invalid note name: {noteName}");
            }
            return index;
        }

        private string GetNoteName(int index)
        {
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            // Ensure index is within bounds by using modulo
            index = ((index % 12) + 12) % 12; // This ensures positive index and wraps around
            return noteNames[index];
        }

        public List<Note> GetCurrentScaleNotes() => currentScaleNotes;
        public List<Scale> GetAvailableScales() => availableScales;

        public static float GetPitchForNote(string note)
        {
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            
            // Extract note name and octave
            string noteName = note.Substring(0, note.Length - 1);
            int octave = int.Parse(note.Substring(note.Length - 1));
            
            // Find note index
            int noteIndex = System.Array.IndexOf(noteNames, noteName);
            
            // Calculate total semitones from A4 (440Hz)
            int semitonesFromA4 = noteIndex - 9 + (octave - 4) * 12;
            
            // Calculate pitch multiplier (2^(semitonesFromA4/12))
            return Mathf.Pow(2f, semitonesFromA4 / 12f);
        }

        public enum ScaleType
        {
            Major,
            Minor,
            Pentatonic
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                SetScale(scaleType);
            }
            else
            {
                // In editor, just update the current scale reference
                currentScale = availableScales.Find(s => s.name == scaleType.ToString());
                if (currentScale == null && availableScales.Count > 0)
                {
                    currentScale = availableScales[0];
                }
            }
        }
    }
} 