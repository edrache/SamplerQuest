using UnityEngine;
using System.Collections.Generic;

namespace SamplerQuest.Audio.Sampler
{
    public class NoteTrigger : MonoBehaviour
    {
        [Header("Sampler Settings")]
        [SerializeField] private SamplerController sampler;
        [SerializeField] private SampleData sampleData;
        [SerializeField] private int priority = 0;
        [SerializeField, Range(0f, 1f)] private float velocity = 1f;

        [Header("Note Settings")]
        [SerializeField] private bool usePositionMapping = true;
        [SerializeField] private int fixedNoteIndex = 0;

        [Header("Position Mapping")]
        [SerializeField] private float minZPosition = -10f;
        [SerializeField] private float maxZPosition = 10f;
        [SerializeField] private int minNoteIndex = 0;
        [SerializeField] private int maxNoteIndex = 24;
        [SerializeField] private bool invertMapping = false;

        // Track which notes are currently playing
        private Dictionary<NoteObject, string> activeNotes = new Dictionary<NoteObject, string>();

        private void Start()
        {
            if (sampler == null)
            {
                Debug.LogError($"Sampler reference is missing on {gameObject.name}!");
                return;
            }

            if (sampleData == null)
            {
                Debug.LogError($"SampleData reference is missing on {gameObject.name}!");
                return;
            }

            // Load the sample when the trigger is created
            sampler.LoadSample(sampleData);
            Debug.Log($"[{gameObject.name}] Loaded sample: {sampleData.sampleName}");
        }

        private void OnDestroy()
        {
            // Stop all notes when the trigger is destroyed
            foreach (var note in activeNotes.Values)
            {
                sampler.StopNote(note);
            }
            activeNotes.Clear();
        }

        private void OnDisable()
        {
            // Stop all notes when the trigger is disabled
            foreach (var note in activeNotes.Values)
            {
                sampler.StopNote(note);
            }
            activeNotes.Clear();
        }

        private int GetNoteIndexFromPosition(float zPosition)
        {
            if (!usePositionMapping)
            {
                return fixedNoteIndex;
            }

            // Check if position is within range
            if (zPosition < minZPosition || zPosition > maxZPosition)
            {
                return fixedNoteIndex;
            }

            // Map position to note index
            float t = Mathf.InverseLerp(minZPosition, maxZPosition, zPosition);
            if (invertMapping)
            {
                t = 1f - t;
            }

            // Quantize to nearest note
            float noteRange = maxNoteIndex - minNoteIndex;
            float quantizedValue = Mathf.Round(t * noteRange);
            return minNoteIndex + Mathf.RoundToInt(quantizedValue);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[{gameObject.name}] Trigger Enter with: {other.gameObject.name}");
            
            NoteObject noteObject = other.GetComponent<NoteObject>();
            if (noteObject != null)
            {
                // Stop any existing note for this object
                if (activeNotes.ContainsKey(noteObject))
                {
                    sampler.StopNote(activeNotes[noteObject]);
                    activeNotes.Remove(noteObject);
                }

                int noteIndex = GetNoteIndexFromPosition(other.transform.position.z);
                string note = NoteManager.GetNoteWithOctave(noteIndex);
                float finalVelocity = noteObject.Velocity * velocity;
                
                Debug.Log($"[{gameObject.name}] Playing note: {note} with velocity: {finalVelocity}");
                sampler.PlayNote(sampleData.sampleName, note, finalVelocity);
                activeNotes[noteObject] = note;
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Collided object {other.gameObject.name} doesn't have NoteObject component!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"[{gameObject.name}] Trigger Exit with: {other.gameObject.name}");
            
            NoteObject noteObject = other.GetComponent<NoteObject>();
            if (noteObject != null && activeNotes.ContainsKey(noteObject))
            {
                string note = activeNotes[noteObject];
                Debug.Log($"[{gameObject.name}] Stopping note: {note}");
                sampler.StopNote(note);
                activeNotes.Remove(noteObject);
            }
        }

        private void OnValidate()
        {
            // Ensure ranges are valid
            if (maxZPosition < minZPosition)
            {
                maxZPosition = minZPosition;
            }
            if (maxNoteIndex < minNoteIndex)
            {
                maxNoteIndex = minNoteIndex;
            }
        }

        public int GetPriority() => priority;
    }
} 