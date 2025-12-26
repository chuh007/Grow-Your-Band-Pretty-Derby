using UnityEngine;
using System;

namespace Code.MainSystem.Rhythm
{
    public class JudgementSystem : MonoBehaviour
    {
        public static JudgementSystem Instance { get; private set; }

        [Header("Dependencies")]
        [SerializeField] private NoteManager noteManager;

        [Header("Timing Windows (Seconds +/-)")]
        [SerializeField] private double perfectWindow = 0.050; 
        [SerializeField] private double greatWindow = 0.100;   
        [SerializeField] private double goodWindow = 0.150;    

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void OnInputDetected(int laneIndex)
        {
            if (noteManager == null || Conductor.Instance == null) return;

            NoteObject targetNote = noteManager.GetNearestNote(laneIndex);
            
            if (targetNote == null) return;

            double songTime = Conductor.Instance.SongPosition;
            double noteTime = targetNote.Data.Time;

            double compensatedTime = songTime + Conductor.Instance.InputOffset;

            double diff = Math.Abs(noteTime - compensatedTime);

            if (diff <= perfectWindow)
            {
                HandleHit(targetNote, JudgementType.Perfect);
            }
            else if (diff <= greatWindow)
            {
                HandleHit(targetNote, JudgementType.Great);
            }
            else if (diff <= goodWindow)
            {
                HandleHit(targetNote, JudgementType.Good);
            }
        }

        private void HandleHit(NoteObject note, JudgementType type)
        {
            Debug.Log($"<color=cyan>{type}</color> on Lane {note.Data.LaneIndex}");
            
            int laneIndex = note.Data.LaneIndex;
            noteManager.DespawnNote(note);

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.RegisterResult(type, laneIndex);
            }
        }

        public void HandleMiss()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.RegisterResult(JudgementType.Miss, -1);
            }
        }
    }
}