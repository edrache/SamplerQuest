using UnityEngine;
using System.Collections.Generic;

namespace SamplerQuest.Audio.Sampler
{
    public class SamplerController : MonoBehaviour
    {
        [Header("Sample Settings")]
        [SerializeField] private int maxSamples = 16;
        [SerializeField] private Transform samplesContainer;
        
        [Header("Playback Settings")]
        [SerializeField] private float globalVolume = 1f;
        [SerializeField] private float globalPitch = 1f;
        
        private Dictionary<string, SamplePlayer> activeSamples = new Dictionary<string, SamplePlayer>();
        private Queue<SamplePlayer> samplePool;
        
        private void Awake()
        {
            InitializeSamplePool();
        }
        
        private void InitializeSamplePool()
        {
            samplePool = new Queue<SamplePlayer>();
            
            for (int i = 0; i < maxSamples; i++)
            {
                GameObject sampleObj = new GameObject($"SamplePlayer_{i}");
                sampleObj.transform.SetParent(samplesContainer);
                SamplePlayer player = sampleObj.AddComponent<SamplePlayer>();
                samplePool.Enqueue(player);
            }
        }
        
        public void LoadSample(SampleData sampleData)
        {
            if (activeSamples.ContainsKey(sampleData.sampleName))
            {
                Debug.LogWarning($"Sample {sampleData.sampleName} is already loaded!");
                return;
            }
            
            if (samplePool.Count == 0)
            {
                Debug.LogWarning("No available sample slots!");
                return;
            }
            
            SamplePlayer player = samplePool.Dequeue();
            player.Initialize(sampleData);
            activeSamples.Add(sampleData.sampleName, player);
        }
        
        public void PlaySample(string sampleName, float velocity = 1f)
        {
            if (activeSamples.TryGetValue(sampleName, out SamplePlayer player))
            {
                player.Play(velocity);
            }
        }
        
        public void StopSample(string sampleName)
        {
            if (activeSamples.TryGetValue(sampleName, out SamplePlayer player))
            {
                player.Stop();
            }
        }
        
        public void UnloadSample(string sampleName)
        {
            if (activeSamples.TryGetValue(sampleName, out SamplePlayer player))
            {
                player.Stop();
                player.Reset();
                samplePool.Enqueue(player);
                activeSamples.Remove(sampleName);
            }
        }
        
        public void SetGlobalVolume(float volume)
        {
            globalVolume = Mathf.Clamp01(volume);
            foreach (var player in activeSamples.Values)
            {
                player.SetVolume(globalVolume);
            }
        }
        
        public void SetGlobalPitch(float pitch)
        {
            globalPitch = pitch;
            foreach (var player in activeSamples.Values)
            {
                player.SetPitch(globalPitch);
            }
        }

        public float GetGlobalVolume() => globalVolume;
        public float GetGlobalPitch() => globalPitch;
    }
} 