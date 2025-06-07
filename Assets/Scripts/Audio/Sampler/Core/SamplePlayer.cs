using UnityEngine;
using System;

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
        
        public event Action<SamplePlayer> OnPlaybackFinished;
        
        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            envelope = new AudioEnvelope();
            samplerController = FindAnyObjectByType<SamplerController>();
        }
        
        public void Initialize(SampleData data)
        {
            sampleData = data;
            audioSource.clip = data.audioClip;
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
        }
        
        public void Play(string note, float velocity = 1f)
        {
            if (audioSource.clip == null)
            {
                //Debug.LogError("No audio clip assigned!");
                return;
            }
            
            currentVelocity = Mathf.Clamp01(velocity);
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
        }
        
        public void Stop()
        {
            if (isPlaying)
            {
                envelope.Release();
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
                    audioSource.Stop();
                    isPlaying = false;
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
            audioSource.clip = null;
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
            isPlaying = false;
            sampleData = null;
        }

        public SampleData GetSampleData() => sampleData;
        public bool IsPlaying() => isPlaying;
    }
} 