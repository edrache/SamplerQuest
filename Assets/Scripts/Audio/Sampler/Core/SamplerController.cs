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
        
        [Header("Envelope Settings")]
        [SerializeField] private AudioEnvelope envelopeSettings = new AudioEnvelope();
        
        private Dictionary<string, SamplePlayer> activeSamples = new Dictionary<string, SamplePlayer>();
        private List<SamplePlayer> samplePool;
        private Dictionary<string, SamplePlayer> currentlyPlaying = new Dictionary<string, SamplePlayer>();
        
        private void Awake()
        {
            InitializeSamplePool();
        }
        
        private void InitializeSamplePool()
        {
            samplePool = new List<SamplePlayer>();
            
            for (int i = 0; i < maxSamples; i++)
            {
                GameObject sampleObj = new GameObject($"SamplePlayer_{i}");
                sampleObj.transform.SetParent(samplesContainer);
                SamplePlayer player = sampleObj.AddComponent<SamplePlayer>();
                player.OnPlaybackFinished += HandlePlaybackFinished;
                samplePool.Add(player);
            }
            
            Debug.Log($"Initialized sample pool with {maxSamples} players");
        }

        private void HandlePlaybackFinished(SamplePlayer player)
        {
            if (currentlyPlaying.ContainsValue(player))
            {
                string noteToRemove = null;
                foreach (var kvp in currentlyPlaying)
                {
                    if (kvp.Value == player)
                    {
                        noteToRemove = kvp.Key;
                        break;
                    }
                }
                
                if (noteToRemove != null)
                {
                    currentlyPlaying.Remove(noteToRemove);
                }
                
                player.Reset();
                samplePool.Add(player);
                Debug.Log($"Returned {player.name} to the pool");
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
            
            SamplePlayer player = samplePool[0];
            samplePool.RemoveAt(0);
            player.Initialize(sampleData);
            activeSamples.Add(sampleData.sampleName, player);
            Debug.Log($"Loaded sample: {sampleData.sampleName} with base note {sampleData.baseNote}");
        }
        
        public bool PlayNote(string sampleName, string note, float velocity = 1f)
        {
            Debug.Log($"Attempting to play note {note} on sample {sampleName}");
            
            if (activeSamples.TryGetValue(sampleName, out SamplePlayer player))
            {
                // Find first available SamplePlayer
                SamplePlayer availablePlayer = FindAvailablePlayer();
                if (availablePlayer != null)
                {
                    availablePlayer.Initialize(player.GetSampleData());
                    availablePlayer.Play(note, velocity);
                    currentlyPlaying[note] = availablePlayer;
                    Debug.Log($"Playing note {note} on sample {sampleName} using player {availablePlayer.name}");
                    return true;
                }
                else
                {
                    Debug.LogWarning("No available players in the pool!");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"Sample {sampleName} not found! Available samples: {string.Join(", ", activeSamples.Keys)}");
                return false;
            }
        }

        public void StopNote(string note)
        {
            if (currentlyPlaying.TryGetValue(note, out SamplePlayer player))
            {
                player.Stop();
                Debug.Log($"Stopping note {note}");
            }
        }
        
        private SamplePlayer FindAvailablePlayer()
        {
            foreach (var player in samplePool)
            {
                if (!player.IsPlaying())
                {
                    return player;
                }
            }
            return null;
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
                samplePool.Add(player);
                activeSamples.Remove(sampleName);
                Debug.Log($"Unloaded sample: {sampleName}");
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
        
        public List<string> GetAvailableSamples()
        {
            return new List<string>(activeSamples.Keys);
        }

        #region Envelope Controls
        public void SetAttackTime(float time)
        {
            envelopeSettings.attackTime = Mathf.Clamp(time, 0f, 2f);
        }

        public void SetDecayTime(float time)
        {
            envelopeSettings.decayTime = Mathf.Clamp(time, 0f, 2f);
        }

        public void SetSustainLevel(float level)
        {
            envelopeSettings.sustainLevel = Mathf.Clamp01(level);
        }

        public void SetReleaseTime(float time)
        {
            envelopeSettings.releaseTime = Mathf.Clamp(time, 0f, 2f);
        }

        public float GetAttackTime() => envelopeSettings.attackTime;
        public float GetDecayTime() => envelopeSettings.decayTime;
        public float GetSustainLevel() => envelopeSettings.sustainLevel;
        public float GetReleaseTime() => envelopeSettings.releaseTime;
        #endregion

        private void OnDestroy()
        {
            // Unsubscribe from events
            foreach (var player in samplePool)
            {
                if (player != null)
                {
                    player.OnPlaybackFinished -= HandlePlaybackFinished;
                }
            }
        }
    }
} 