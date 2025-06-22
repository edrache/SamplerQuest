using UnityEngine;

namespace SamplerQuest.Audio.Sampler
{
    [System.Serializable]
    public class AudioEnvelope
    {
        [Header("ADSR Settings")]
        public float attackTime = 0.1f;    // Time to reach peak volume
        public float decayTime = 0.1f;     // Time to reach sustain level
        public float sustainLevel = 0.7f;  // Volume level during sustain
        public float releaseTime = 0.2f;   // Time to fade out after release
        public float sustainDuration = 0f; // Duration of sustain phase (0 = infinite)
        
        private float currentTime;
        private float currentVolume;
        private bool isReleased;
        private EnvelopeStage currentStage;
        private float currentVelocity = 1f;
        private float totalDuration = 0f;
        
        private enum EnvelopeStage
        {
            Attack,
            Decay,
            Sustain,
            Release,
            Finished
        }
        
        public void Start(float velocity = 1f)
        {
            currentTime = 0f;
            currentVolume = 0f;
            isReleased = false;
            currentStage = EnvelopeStage.Attack;
            currentVelocity = Mathf.Clamp01(velocity);
            totalDuration = 0f;
            Debug.Log($"AudioEnvelope: Started with velocity {currentVelocity}");
        }
        
        public void StartWithDuration(float velocity, float duration)
        {
            currentTime = 0f;
            currentVolume = 0f;
            isReleased = false;
            currentStage = EnvelopeStage.Attack;
            currentVelocity = Mathf.Clamp01(velocity);
            totalDuration = duration;
            Debug.Log($"AudioEnvelope: Started with velocity {currentVelocity} and duration {duration}");
        }
        
        public void Release()
        {
            if (currentStage != EnvelopeStage.Finished)
            {
                isReleased = true;
                currentTime = 0f;
                currentStage = EnvelopeStage.Release;
                Debug.Log($"AudioEnvelope: Released, moving to Release stage");
            }
        }
        
        public float Update(float deltaTime)
        {
            if (currentStage == EnvelopeStage.Finished)
                return 0f;
                
            currentTime += deltaTime;
            
            switch (currentStage)
            {
                case EnvelopeStage.Attack:
                    if (currentTime >= attackTime)
                    {
                        currentStage = EnvelopeStage.Decay;
                        currentTime = 0f;
                        Debug.Log($"AudioEnvelope: Attack finished, moving to Decay");
                    }
                    else
                    {
                        currentVolume = Mathf.Lerp(0f, currentVelocity, currentTime / attackTime);
                    }
                    break;
                    
                case EnvelopeStage.Decay:
                    if (currentTime >= decayTime)
                    {
                        currentStage = EnvelopeStage.Sustain;
                        currentVolume = sustainLevel * currentVelocity;
                        currentTime = 0f; // Reset time for sustain phase
                        Debug.Log($"AudioEnvelope: Decay finished, moving to Sustain");
                    }
                    else
                    {
                        currentVolume = Mathf.Lerp(currentVelocity, sustainLevel * currentVelocity, currentTime / decayTime);
                    }
                    break;
                    
                case EnvelopeStage.Sustain:
                    // Check if sustain duration has elapsed
                    if (sustainDuration > 0f && currentTime >= sustainDuration)
                    {
                        currentStage = EnvelopeStage.Release;
                        currentTime = 0f;
                        Debug.Log($"AudioEnvelope: Sustain duration elapsed, moving to Release");
                    }
                    else if (isReleased)
                    {
                        currentStage = EnvelopeStage.Release;
                        currentTime = 0f;
                        Debug.Log($"AudioEnvelope: Sustain interrupted, moving to Release");
                    }
                    break;
                    
                case EnvelopeStage.Release:
                    if (currentTime >= releaseTime)
                    {
                        currentStage = EnvelopeStage.Finished;
                        currentVolume = 0f;
                        Debug.Log($"AudioEnvelope: Release finished, envelope complete");
                    }
                    else
                    {
                        currentVolume = Mathf.Lerp(sustainLevel * currentVelocity, 0f, currentTime / releaseTime);
                    }
                    break;
            }
            
            return currentVolume;
        }
        
        public bool IsFinished()
        {
            return currentStage == EnvelopeStage.Finished;
        }
    }
} 