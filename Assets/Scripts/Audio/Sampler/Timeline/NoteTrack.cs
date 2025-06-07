using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace SamplerQuest.Audio.Sampler
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(NotePlayableAsset))]
    [TrackBindingType(typeof(SamplerController))]
    public class NoteTrack : TrackAsset
    {
        [SerializeField] private SampleData sampleData;
        private PlayableDirector director;

        public SampleData GetSampleData() => sampleData;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            Debug.Log("Creating NoteTrack mixer");
            director = go.GetComponent<PlayableDirector>();
            
            // Try to load the sample if we have both sampler and sample data
            if (director != null && sampleData != null)
            {
                var sampler = director.GetGenericBinding(this) as SamplerController;
                if (sampler != null)
                {
                    Debug.Log($"Loading sample {sampleData.sampleName} into sampler");
                    sampler.LoadSample(sampleData);
                }
                else
                {
                    Debug.LogError("SamplerController not found in track binding!");
                }
            }
            
            return ScriptPlayable<NoteMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);
            this.director = director;
            
            if (sampleData != null)
            {
                var sampler = director.GetGenericBinding(this) as SamplerController;
                if (sampler != null)
                {
                    Debug.Log($"Loading sample {sampleData.sampleName} into sampler");
                    sampler.LoadSample(sampleData);
                }
                else
                {
                    Debug.LogError("SamplerController not found in track binding!");
                }
            }
            else
            {
                Debug.LogError("SampleData not assigned to track!");
            }
        }
    }
} 