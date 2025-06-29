using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SamplerQuest.Audio.Sampler;
using UnityEngine.Playables;

public class BrickMusicManager : MonoBehaviour
{
    public static BrickMusicManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private SamplerController m_SamplerController;
    [SerializeField] private NoteManager m_NoteManager;
    [Header("Sample Data")]
    [SerializeField] private SampleData m_SampleData;
    [Header("PlayableDirector Quantization")]
    [SerializeField] private PlayableDirector m_PlayableDirector;
    [SerializeField] private float m_QuantizeStep = 1f;
    [SerializeField] private float m_QuantizeOffset = 0f;
    [SerializeField] private bool m_PlaySimultaneous = true;
    [Header("Note Durations")]
    [SerializeField] private float m_DefaultNoteDuration = 0.5f;
    [SerializeField] private List<NoteDuration> m_NoteDurations = new List<NoteDuration>();

    [System.Serializable]
    public class NoteDuration
    {
        public string note;
        public float duration = 0.5f;
    }

    private Dictionary<string, float> m_NoteDurationDict;

    private class PendingNote
    {
        public string note;
        public float velocity;
        public double requestTime;
    }
    private List<PendingNote> m_PendingNotes = new List<PendingNote>();
    private double m_LastQuantizedTime = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (transform.parent != null)
            transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        m_NoteDurationDict = new Dictionary<string, float>();
        foreach (var nd in m_NoteDurations)
        {
            if (!string.IsNullOrEmpty(nd.note))
                m_NoteDurationDict[nd.note] = nd.duration;
        }
    }

    private void Update()
    {
        if (m_QuantizeStep <= 0f)
        {
            FlushPendingNotes();
            return;
        }
        if (m_PlayableDirector == null || m_PlayableDirector.duration <= 0)
            return;
        double time = m_PlayableDirector.time + m_QuantizeOffset;
        double duration = m_PlayableDirector.duration;
        // Zapętlony timeline
        time = time % duration;
        double quantizedTime = Mathf.FloorToInt((float)(time / m_QuantizeStep)) * m_QuantizeStep;
        if (quantizedTime != m_LastQuantizedTime)
        {
            PlayPendingNotesAtQuantizedTime(quantizedTime);
            m_LastQuantizedTime = quantizedTime;
        }
    }

    public void PlayBrickNote(string note, float velocity)
    {
        if (m_QuantizeStep <= 0f)
        {
            PlayNoteImmediate(note, velocity);
            return;
        }
        m_PendingNotes.Add(new PendingNote { note = note, velocity = velocity, requestTime = m_PlayableDirector != null ? m_PlayableDirector.time : Time.time });
    }

    private void PlayPendingNotesAtQuantizedTime(double quantizedTime)
    {
        if (m_PendingNotes.Count == 0)
            return;
        if (m_PlaySimultaneous)
        {
            foreach (var pending in m_PendingNotes)
            {
                PlayNoteImmediate(pending.note, pending.velocity);
            }
            m_PendingNotes.Clear();
        }
        else
        {
            // Zagraj tylko pierwszy dźwięk z kolejki
            var pending = m_PendingNotes[0];
            PlayNoteImmediate(pending.note, pending.velocity);
            m_PendingNotes.RemoveAt(0);
        }
    }

    public void FlushPendingNotes()
    {
        foreach (var pending in m_PendingNotes)
        {
            PlayNoteImmediate(pending.note, pending.velocity);
        }
        m_PendingNotes.Clear();
    }

    private void PlayNoteImmediate(string note, float velocity)
    {
        if (m_SamplerController == null)
        {
            Debug.LogWarning("BrickMusicManager: No SamplerController assigned!");
            return;
        }
        if (m_SampleData == null)
        {
            Debug.LogWarning("BrickMusicManager: No SampleData assigned!");
            return;
        }
        if (string.IsNullOrEmpty(note))
        {
            Debug.LogWarning("BrickMusicManager: Note is null or empty!");
            return;
        }
        bool played = m_SamplerController.PlayNote(m_SampleData.sampleName, note, velocity);
        Debug.Log($"BrickMusicManager: PlayNote({m_SampleData.sampleName}, {note}, velocity: {velocity}) result: {played}");
        float duration = m_DefaultNoteDuration;
        if (m_NoteDurationDict != null && m_NoteDurationDict.ContainsKey(note))
            duration = m_NoteDurationDict[note];
        StartCoroutine(StopNoteAfterDelay(note, duration));
    }

    private IEnumerator StopNoteAfterDelay(string note, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (m_SamplerController != null)
        {
            m_SamplerController.StopNote(note);
        }
    }
} 