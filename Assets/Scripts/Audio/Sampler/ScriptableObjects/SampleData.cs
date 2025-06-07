using UnityEngine;

namespace SamplerQuest.Audio.Sampler
{
    [CreateAssetMenu(fileName = "New Sample", menuName = "SamplerQuest/Audio/Sample")]
    public class SampleData : ScriptableObject
    {
        [Header("Sample Settings")]
        public AudioClip audioClip;
        public string sampleName;
        public string baseNote = "C4"; // Base note for pitch mapping
        public bool isLooping = false;
        
        [Header("Playback Settings")]
        public float defaultVolume = 1f;
        public float defaultPitch = 1f;
        
        [Header("Velocity Settings")]
        public float minVelocity = 0f;
        public float maxVelocity = 1f;
        
        private void OnValidate()
        {
            if (audioClip != null && string.IsNullOrEmpty(sampleName))
            {
                sampleName = audioClip.name;
            }
        }
    }
} 