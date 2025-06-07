using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SamplerQuest.Audio.Sampler
{
    [System.Serializable]
    public class NotePlayableAsset : PlayableAsset
    {
        public int noteIndex;
        public float velocity = 1f;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Debug.Log("Creating NotePlayable");
            var playable = ScriptPlayable<NotePlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            
            behaviour.noteIndex = noteIndex;
            behaviour.velocity = velocity;
            
            var director = owner.GetComponent<PlayableDirector>();
            if (director != null)
            {
                Debug.Log("Found PlayableDirector");
                
                // Get all tracks from the timeline
                var timeline = director.playableAsset as TimelineAsset;
                if (timeline != null)
                {
                    foreach (var track in timeline.GetOutputTracks())
                    {
                        if (track is NoteTrack noteTrack)
                        {
                            Debug.Log("Found NoteTrack");
                            behaviour.targetSampler = director.GetGenericBinding(noteTrack) as SamplerController;
                            behaviour.sampleData = noteTrack.GetSampleData();
                            
                            Debug.Log($"Sampler: {(behaviour.targetSampler != null ? "Found" : "Null")}, SampleData: {(behaviour.sampleData != null ? "Found" : "Null")}");
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Timeline asset is not a TimelineAsset!");
                }
            }
            else
            {
                Debug.LogError("Could not find PlayableDirector!");
            }
            
            return playable;
        }
    }
} 