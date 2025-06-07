using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace SamplerQuest.Audio.Sampler
{
    public class BPMController : MonoBehaviour
    {
        [Header("BPM Settings")]
        [SerializeField, Range(20f, 300f)] private float currentBPM = 120f;
        [SerializeField] private bool smoothTransition = false;
        [SerializeField, Range(0.1f, 5f)] private float transitionTime = 1f;

        [Header("Timeline Settings")]
        [SerializeField] private List<PlayableDirector> controlledTimelines = new List<PlayableDirector>();

        private float targetBPM;
        private float transitionStartTime;
        private float transitionStartBPM;
        private bool isTransitioning = false;
        private Dictionary<PlayableDirector, float> timelineSpeeds = new Dictionary<PlayableDirector, float>();
        private float lastInspectorBPM;

        private void Start()
        {
            targetBPM = currentBPM;
            lastInspectorBPM = currentBPM;
            Debug.Log($"BPMController started with {controlledTimelines.Count} timelines");
        }

        private void OnValidate()
        {
            if (Application.isPlaying && currentBPM != lastInspectorBPM)
            {
                if (smoothTransition)
                {
                    targetBPM = currentBPM;
                    transitionStartTime = Time.time;
                    transitionStartBPM = lastInspectorBPM;
                    isTransitioning = true;
                    Debug.Log($"Starting smooth transition from {lastInspectorBPM} to {targetBPM}");
                }
                else
                {
                    SetBPM(currentBPM);
                }
                lastInspectorBPM = currentBPM;
            }
        }

        private void Update()
        {
            // Update all timelines
            foreach (var timeline in controlledTimelines)
            {
                if (timeline != null)
                {
                    if (!timelineSpeeds.ContainsKey(timeline))
                    {
                        timelineSpeeds[timeline] = currentBPM / 60f;
                    }

                    if (timeline.playableGraph.IsValid())
                    {
                        var rootPlayable = timeline.playableGraph.GetRootPlayable(0);
                        if (rootPlayable.IsValid())
                        {
                            rootPlayable.SetSpeed(timelineSpeeds[timeline]);
                        }
                    }
                }
            }

            if (isTransitioning)
            {
                float progress = (Time.time - transitionStartTime) / transitionTime;
                if (progress >= 1f)
                {
                    currentBPM = targetBPM;
                    isTransitioning = false;
                    UpdateAllTimelines(currentBPM);
                    Debug.Log($"Transition completed. Final BPM: {currentBPM}");
                }
                else
                {
                    float newBPM = Mathf.Lerp(transitionStartBPM, targetBPM, progress);
                    currentBPM = newBPM;
                    UpdateAllTimelines(currentBPM);
                    Debug.Log($"Transition progress: {progress:P0}, Current BPM: {currentBPM:F1}");
                }
            }
        }

        public void SetBPM(float newBPM)
        {
            Debug.Log($"Setting BPM to {newBPM}");
            if (smoothTransition)
            {
                targetBPM = Mathf.Clamp(newBPM, 20f, 300f);
                transitionStartTime = Time.time;
                transitionStartBPM = currentBPM;
                isTransitioning = true;
                Debug.Log($"Starting smooth transition from {currentBPM} to {targetBPM}");
            }
            else
            {
                currentBPM = Mathf.Clamp(newBPM, 20f, 300f);
                lastInspectorBPM = currentBPM;
                UpdateAllTimelines(currentBPM);
            }
        }

        private void UpdateAllTimelines(float bpm)
        {
            float speed = bpm / 60f;
            
            foreach (var timeline in controlledTimelines)
            {
                if (timeline != null)
                {
                    timelineSpeeds[timeline] = speed;
                    if (timeline.playableGraph.IsValid())
                    {
                        var rootPlayable = timeline.playableGraph.GetRootPlayable(0);
                        if (rootPlayable.IsValid())
                        {
                            rootPlayable.SetSpeed(speed);
                            timeline.playableGraph.Evaluate();
                        }
                    }
                }
            }
        }

        public void AddTimeline(PlayableDirector timeline)
        {
            if (timeline != null && !controlledTimelines.Contains(timeline))
            {
                Debug.Log($"Adding timeline: {timeline.name}");
                controlledTimelines.Add(timeline);
                timelineSpeeds[timeline] = currentBPM / 60f;
            }
        }

        public void RemoveTimeline(PlayableDirector timeline)
        {
            if (timeline != null)
            {
                Debug.Log($"Removing timeline: {timeline.name}");
                controlledTimelines.Remove(timeline);
                timelineSpeeds.Remove(timeline);
            }
        }

        public float GetCurrentBPM() => currentBPM;
        public bool IsTransitioning() => isTransitioning;
    }
} 