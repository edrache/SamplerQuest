using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SamplerQuest.Audio.Sampler
{
    public class NotePlayableBehaviour : PlayableBehaviour
    {
        public int noteIndex;
        public float velocity = 1f;
        public SamplerController targetSampler;
        public SampleData sampleData;

        private bool isPlaying = false;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            //Debug.Log($"OnBehaviourPlay called. Sampler: {(targetSampler != null ? "Found" : "Null")}, SampleData: {(sampleData != null ? "Found" : "Null")}, NoteIndex: {noteIndex}");
            
            if (targetSampler != null && !isPlaying && sampleData != null)
            {
                // Ensure sample is loaded
                if (!targetSampler.GetAvailableSamples().Contains(sampleData.sampleName))
                {
                    //Debug.Log($"Loading sample {sampleData.sampleName} into sampler");
                    targetSampler.LoadSample(sampleData);
                }

                string note = NoteManager.GetNoteWithOctave(noteIndex);
                //Debug.Log($"Playing note: {note} on sample: {sampleData.sampleName} with velocity: {velocity}");
                targetSampler.PlayNote(sampleData.sampleName, note, velocity);
                isPlaying = true;
            }
            else
            {
                if (targetSampler == null) Debug.LogError("Target Sampler is null!");
                if (sampleData == null) Debug.LogError("SampleData is null!");
                if (isPlaying) Debug.Log("Already playing!");
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //Debug.Log("OnBehaviourPause called");
            if (targetSampler != null && isPlaying)
            {
                string note = NoteManager.GetNoteWithOctave(noteIndex);
                targetSampler.StopNote(note);
                isPlaying = false;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            //Debug.Log("OnPlayableDestroy called");
            if (targetSampler != null && isPlaying)
            {
                string note = NoteManager.GetNoteWithOctave(noteIndex);
                targetSampler.StopNote(note);
                isPlaying = false;
            }
        }
    }
} 