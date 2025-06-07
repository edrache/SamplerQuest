using UnityEngine;
using UnityEngine.Playables;

namespace SamplerQuest.Audio.Sampler
{
    public class NoteMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // We don't need to do anything here as the individual NotePlayableBehaviour
            // instances handle their own playback
        }
    }
} 