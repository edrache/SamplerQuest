using UnityEngine;

namespace SamplerQuest.Audio.Sampler
{
    public class SamplerTester : MonoBehaviour
    {
        [Header("Sampler Reference")]
        [SerializeField] private SamplerController sampler;
        
        [Header("Test Samples")]
        [SerializeField] private SampleData[] testSamples;
        
        private void Start()
        {
            if (sampler == null)
            {
                Debug.LogError("Sampler reference is missing!");
                return;
            }
            
            // Load all test samples
            foreach (var sample in testSamples)
            {
                if (sample != null)
                {
                    sampler.LoadSample(sample);
                    Debug.Log($"Loaded sample: {sample.sampleName}");
                }
            }
        }
        
        private void Update()
        {
            // Test playing samples with number keys 1-9
            for (int i = 0; i < Mathf.Min(testSamples.Length, 9); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    sampler.PlaySample(testSamples[i].sampleName);
                }
            }
            
            // Test volume control with Q and E
            if (Input.GetKey(KeyCode.Q))
            {
                sampler.SetGlobalVolume(Mathf.Max(0, sampler.GetGlobalVolume() - Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.E))
            {
                sampler.SetGlobalVolume(Mathf.Min(1, sampler.GetGlobalVolume() + Time.deltaTime));
            }
            
            // Test pitch control with Z and C
            if (Input.GetKey(KeyCode.Z))
            {
                sampler.SetGlobalPitch(Mathf.Max(0.5f, sampler.GetGlobalPitch() - Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.C))
            {
                sampler.SetGlobalPitch(Mathf.Min(2f, sampler.GetGlobalPitch() + Time.deltaTime));
            }
        }
    }
} 