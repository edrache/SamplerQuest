using UnityEngine;

namespace SamplerQuest.Audio.Sampler
{
    [System.Serializable]
    public class AudioEnvelope
    {
        [Header("ADSR Settings")]
        [Range(0f, 2f)]
        public float attackTime = 0.1f;    // Time to reach peak volume
        [Range(0f, 2f)]
        public float decayTime = 0.1f;     // Time to reach sustain level
        [Range(0f, 1f)]
        public float sustainLevel = 0.7f;  // Volume level during sustain
        [Range(0f, 2f)]
        public float releaseTime = 0.2f;   // Time to fade out after release
        
        private float currentTime;
        private float currentVolume;
        private bool isReleased;
        private EnvelopeStage currentStage;
        
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
        }
        
        public void Release()
        {
            if (currentStage != EnvelopeStage.Finished)
            {
                isReleased = true;
                currentTime = 0f;
                currentStage = EnvelopeStage.Release;
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
                    }
                    else
                    {
                        currentVolume = Mathf.Lerp(0f, 1f, currentTime / attackTime);
                    }
                    break;
                    
                case EnvelopeStage.Decay:
                    if (currentTime >= decayTime)
                    {
                        currentStage = EnvelopeStage.Sustain;
                        currentVolume = sustainLevel;
                    }
                    else
                    {
                        currentVolume = Mathf.Lerp(1f, sustainLevel, currentTime / decayTime);
                    }
                    break;
                    
                case EnvelopeStage.Sustain:
                    if (isReleased)
                    {
                        currentStage = EnvelopeStage.Release;
                        currentTime = 0f;
                    }
                    break;
                    
                case EnvelopeStage.Release:
                    if (currentTime >= releaseTime)
                    {
                        currentStage = EnvelopeStage.Finished;
                        currentVolume = 0f;
                    }
                    else
                    {
                        currentVolume = Mathf.Lerp(sustainLevel, 0f, currentTime / releaseTime);
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