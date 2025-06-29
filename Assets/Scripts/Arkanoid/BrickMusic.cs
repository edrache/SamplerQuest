using UnityEngine;
using System.Collections.Generic;

public class BrickMusic : Brick
{
    [Header("Music Settings")]
    [SerializeField] private List<string> m_NoteRange = new List<string> { "C4", "D4", "E4", "F4", "G4", "A4", "B4" };
    [SerializeField, Range(0f, 1f)] private float m_NoteVelocity = 1f;
    private string m_SelectedNote;

    private void Awake()
    {
        base.Awake();
        if (m_NoteRange != null && m_NoteRange.Count > 0)
        {
            int randomIndex = Random.Range(0, m_NoteRange.Count);
            m_SelectedNote = m_NoteRange[randomIndex];
        }
        else
        {
            m_SelectedNote = "C4";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int prevHitPoints = this.CurrentHitPoints;
        base.OnCollisionEnter(collision);
        // Jeśli Ball uderzył i Brick stracił hit point lub został zniszczony
        if (collision.gameObject.GetComponent<BController>() != null)
        {
            if (prevHitPoints > 0 && this.CurrentHitPoints < prevHitPoints)
            {
                PlayNote();
            }
            else if (prevHitPoints == 1 && this.CurrentHitPoints <= 0)
            {
                PlayNote();
            }
        }
    }

    private void PlayNote()
    {
        if (BrickMusicManager.Instance != null && !string.IsNullOrEmpty(m_SelectedNote))
        {
            string noteToPlay = m_SelectedNote;
            // Zmniejsz oktawę o (CurrentHitPoints - 1) jeśli więcej niż 1
            if (this.CurrentHitPoints > 1)
            {
                noteToPlay = DecreaseOctave(m_SelectedNote, this.CurrentHitPoints - 1);
            }
            BrickMusicManager.Instance.PlayBrickNote(noteToPlay, m_NoteVelocity);
        }
    }

    private string DecreaseOctave(string note, int decrease)
    {
        // Zakładamy format np. "C4", "D#5" itd.
        int octaveIndex = note.Length - 1;
        // Jeśli nuta ma #, to oktawa jest na końcu
        if (note.Length > 2 && note[note.Length - 2] == '#')
            octaveIndex = note.Length - 1;
        int octave;
        if (int.TryParse(note.Substring(octaveIndex), out octave))
        {
            int newOctave = Mathf.Max(0, octave - decrease);
            return note.Substring(0, octaveIndex) + newOctave.ToString();
        }
        return note;
    }
} 