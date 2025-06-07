using UnityEngine;

namespace SamplerQuest.Audio.Sampler
{
    public class SamplePlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        private SampleData sampleData;
        private float currentVolume;
        private float currentPitch;
        
        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        public void Initialize(SampleData data)
        {
            sampleData = data;
            audioSource.clip = data.audioClip;
            audioSource.loop = data.isLooping;
            currentVolume = data.defaultVolume;
            currentPitch = data.defaultPitch;
            UpdateAudioSource();
        }
        
        public void Play(float velocity = 1f)
        {
            if (sampleData == null) return;
            
            float velocityMultiplier = Mathf.Lerp(
                sampleData.minVelocity,
                sampleData.maxVelocity,
                velocity
            );
            
            audioSource.volume = currentVolume * velocityMultiplier;
            audioSource.Play();
        }
        
        public void Stop()
        {
            audioSource.Stop();
        }
        
        public void SetVolume(float volume)
        {
            currentVolume = volume;
            UpdateAudioSource();
        }
        
        public void SetPitch(float pitch)
        {
            currentPitch = pitch;
            UpdateAudioSource();
        }
        
        public void Reset()
        {
            sampleData = null;
            audioSource.clip = null;
            currentVolume = 1f;
            currentPitch = 1f;
            UpdateAudioSource();
        }
        
        private void UpdateAudioSource()
        {
            if (audioSource != null)
            {
                audioSource.volume = currentVolume;
                audioSource.pitch = currentPitch;
            }
        }
    }
} 