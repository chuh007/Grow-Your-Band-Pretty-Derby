using UnityEngine;
using System;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm
{
    public class JudgementSystem : MonoBehaviour
    {
        [Header("Dependencies")]
        [Inject] private NoteManager noteManager;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private Conductor _conductor;

        [Header("Timing Windows (Seconds +/-)")]
        [SerializeField] private double perfectWindow = 0.050; 
        [SerializeField] private double greatWindow = 0.100;   
        [SerializeField] private double goodWindow = 0.150;    

        public void OnInputDetected(int laneIndex)
        {
            if (noteManager == null || _conductor == null) return;

            NoteObject targetNote = noteManager.GetNearestNote(laneIndex);
            
            if (targetNote == null) return;

            double songTime = _conductor.SongPosition;
            double noteTime = targetNote.Data.Time;

            double compensatedTime = songTime + _conductor.InputOffset;

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

            if (_scoreManager != null)
            {
                _scoreManager.RegisterResult(type, laneIndex);
            }

            Bus<NoteHitEvent>.Raise(new NoteHitEvent(type, laneIndex, note.Data.MemberId));
        }

        public void HandleMiss(NoteObject note = null)
        {
            int laneIndex = note != null ? note.Data.LaneIndex : -1;
            int trackIndex = note != null ? note.Data.MemberId : 0;

            if (_scoreManager != null)
            {
                _scoreManager.RegisterResult(JudgementType.Miss, laneIndex);
            }

            Bus<NoteHitEvent>.Raise(new NoteHitEvent(JudgementType.Miss, laneIndex, trackIndex));
        }
    }
}