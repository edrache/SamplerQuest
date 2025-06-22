using UnityEngine;
using System;
using System.Collections;

namespace SamplerQuest.Audio.Sampler
{
    public class SamplePlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        private AudioEnvelope envelope;
        private bool isPlaying;
        private float currentVelocity = 1f;
        private SamplerController samplerController;
        private SampleData sampleData;
        private string currentNote;
        private Coroutine stopNoteCoroutine;
        
        public event Action<SamplePlayer> OnPlaybackFinished;
        
        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            envelope = new AudioEnvelope();
            
            // Get SamplerController from parent
            samplerController = GetComponentInParent<SamplerController>();
            if (samplerController == null)
            {
                Debug.LogError("No SamplerController found in parent hierarchy!");
            }
        }
        
        public void Initialize(SampleData data)
        {
            sampleData = data;
            audioSource.clip = data.audioClip;
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
            Debug.Log($"SamplePlayer {name}: Initialized with sample {data.sampleName}");
        }
        
        public void Play(string note, float velocity = 1f)
        {
            if (audioSource.clip == null)
            {
                Debug.LogError($"SamplePlayer {name}: No audio clip assigned!");
                return;
            }
            
            currentVelocity = Mathf.Clamp01(velocity);
            currentNote = note;
            float pitch = NoteManager.GetPitchForNote(note);
            audioSource.pitch = pitch;
            
            // Copy envelope settings from SamplerController
            if (samplerController != null)
            {
                envelope.attackTime = samplerController.GetAttackTime();
                envelope.decayTime = samplerController.GetDecayTime();
                envelope.sustainLevel = samplerController.GetSustainLevel();
                envelope.releaseTime = samplerController.GetReleaseTime();
            }
            
            // Start playback
            audioSource.Play();
            isPlaying = true;
            envelope.Start(currentVelocity);
            Debug.Log($"SamplePlayer {name}: Started playing note {note} with pitch {pitch}");
        }
        
        public void PlayWithDuration(string note, float duration, float velocity = 1f)
        {
            if (audioSource.clip == null)
            {
                Debug.LogError($"SamplePlayer {name}: No audio clip assigned!");
                return;
            }
            
            currentVelocity = Mathf.Clamp01(velocity);
            currentNote = note;
            float pitch = NoteManager.GetPitchForNote(note);
            audioSource.pitch = pitch;
            
            // Set envelope duration based on the note duration
            if (samplerController != null)
            {
                float attackTime = Mathf.Min(samplerController.GetAttackTime(), duration * 0.1f);
                float decayTime = Mathf.Min(samplerController.GetDecayTime(), duration * 0.1f);
                float sustainTime = duration - attackTime - decayTime - samplerController.GetReleaseTime();
                
                envelope.attackTime = attackTime;
                envelope.decayTime = decayTime;
                envelope.sustainLevel = samplerController.GetSustainLevel();
                envelope.releaseTime = samplerController.GetReleaseTime();
                envelope.sustainDuration = Mathf.Max(0f, sustainTime);
            }
            
            // Start playback
            audioSource.Play();
            isPlaying = true;
            envelope.StartWithDuration(currentVelocity, duration);
            
            // Start coroutine to stop the note after duration
            if (stopNoteCoroutine != null)
            {
                StopCoroutine(stopNoteCoroutine);
            }
            stopNoteCoroutine = StartCoroutine(StopNoteAfterDuration(duration));
            
            Debug.Log($"SamplePlayer {name}: Started playing note {note} with pitch {pitch} and duration {duration}");
        }
        
        private IEnumerator StopNoteAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            
            if (isPlaying)
            {
                Debug.Log($"SamplePlayer {name}: Duration elapsed, stopping note {currentNote}");
                Stop();
            }
        }
        
        public void Stop()
        {
            if (isPlaying)
            {
                Debug.Log($"SamplePlayer {name}: Stopping playback");
                envelope.Release();
                
                // Stop the coroutine if it's running
                if (stopNoteCoroutine != null)
                {
                    StopCoroutine(stopNoteCoroutine);
                    stopNoteCoroutine = null;
                }
            }
        }
        
        private void Update()
        {
            if (isPlaying)
            {
                float envelopeVolume = envelope.Update(Time.deltaTime);
                audioSource.volume = envelopeVolume;
                
                if (envelope.IsFinished())
                {
                    Debug.Log($"SamplePlayer {name}: Playback finished, triggering OnPlaybackFinished event");
                    audioSource.Stop();
                    isPlaying = false;
                    
                    // Stop the coroutine if it's still running
                    if (stopNoteCoroutine != null)
                    {
                        StopCoroutine(stopNoteCoroutine);
                        stopNoteCoroutine = null;
                    }
                    
                    OnPlaybackFinished?.Invoke(this);
                }
            }
        }
        
        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }
        
        public void SetPitch(float pitch)
        {
            audioSource.pitch = pitch;
        }
        
        public void Reset()
        {
            Debug.Log($"SamplePlayer {name}: Resetting");
            audioSource.clip = null;
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
            isPlaying = false;
            sampleData = null;
            currentNote = null;
            
            // Stop the coroutine if it's running
            if (stopNoteCoroutine != null)
            {
                StopCoroutine(stopNoteCoroutine);
                stopNoteCoroutine = null;
            }
        }

        public SampleData GetSampleData() => sampleData;
        public bool IsPlaying() => isPlaying;
    }
} 