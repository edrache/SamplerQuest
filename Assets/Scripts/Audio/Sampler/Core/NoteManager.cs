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

        public void SetScale(string scaleName)
        {
            currentScale = availableScales.Find(s => s.name == scaleName);
            if (currentScale != null)
            {
                UpdateCurrentScale();
            }
        }

        public void SetScaleType(ScaleType type)
        {
            scaleType = type;
            SetScale(type.ToString());
        }

        private void UpdateCurrentScale()
        {
            currentScaleNotes.Clear();
            int rootIndex = GetNoteIndex(rootNote);
            
            foreach (int interval in currentScale.intervals)
            {
                int noteIndex = (rootIndex + interval) % 12;
                int octaveOffset = (rootIndex + interval) / 12;
                string noteName = GetNoteName(noteIndex);
                int octave = baseOctave + octaveOffset;
                
                currentScaleNotes.Add(new Note
                {
                    name = $"{noteName}{octave}",
                    octave = octave,
                    frequency = noteFrequencies[$"{noteName}{octave}"]
                });
            }
        }

        private int GetNoteIndex(string note)
        {
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            return System.Array.IndexOf(noteNames, note);
        }

        private string GetNoteName(int index)
        {
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
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
    }
} 