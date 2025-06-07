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
        private bool isInitialized = false;

        public SampleData GetSampleData() => sampleData;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            Debug.Log("Creating NoteTrack mixer");
            director = go.GetComponent<PlayableDirector>();
            
            if (director == null)
            {
                Debug.LogError("PlayableDirector not found on GameObject!");
                return ScriptPlayable<NoteMixerBehaviour>.Create(graph, inputCount);
            }

            if (sampleData == null)
            {
                Debug.LogError("SampleData not assigned to track!");
                return ScriptPlayable<NoteMixerBehaviour>.Create(graph, inputCount);
            }

            var sampler = director.GetGenericBinding(this) as SamplerController;
            if (sampler == null)
            {
                Debug.LogError("SamplerController not found in track binding!");
                return ScriptPlayable<NoteMixerBehaviour>.Create(graph, inputCount);
            }

            // Try to load the sample
            try
            {
                Debug.Log($"Loading sample {sampleData.sampleName} into sampler");
                sampler.LoadSample(sampleData);
                isInitialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load sample: {e.Message}");
            }
            
            return ScriptPlayable<NoteMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            if (director == null)
            {
                Debug.LogError("PlayableDirector is null in GatherProperties!");
                return;
            }

            base.GatherProperties(director, driver);
            this.director = director;
            
            if (sampleData == null)
            {
                Debug.LogError("SampleData not assigned to track!");
                return;
            }

            var sampler = director.GetGenericBinding(this) as SamplerController;
            if (sampler == null)
            {
                Debug.LogError("SamplerController not found in track binding!");
                return;
            }

            // Only try to load the sample if we haven't already initialized
            if (!isInitialized)
            {
                try
                {
                    Debug.Log($"Loading sample {sampleData.sampleName} into sampler");
                    sampler.LoadSample(sampleData);
                    isInitialized = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load sample: {e.Message}");
                }
            }
        }

        private void OnDestroy()
        {
            isInitialized = false;
        }
    }
} 