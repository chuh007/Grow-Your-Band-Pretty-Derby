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
        [Inject] private RhythmLineController _lineController;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private Conductor _conductor;

        [Header("Base Timing Windows (Seconds +/-)")]
        [SerializeField] private double perfectWindow = 0.050; 
        [SerializeField] private double greatWindow = 0.100;   
        [SerializeField] private double goodWindow = 0.150;    

        private PartDataSO _currentPartData;

        public void SetPartData(PartDataSO partData)
        {
            _currentPartData = partData;
        }

        public void OnInputDetected(int laneIndex)
        {
            if (_lineController == null || _conductor == null) return;

            double songTime = _conductor.SongPosition;
            double compensatedTime = songTime + _conductor.InputOffset;

            NoteData targetNote = _lineController.GetNearestNote(compensatedTime);
            
            if (targetNote == null) return;

            double diff = Math.Abs(targetNote.Time - compensatedTime);
            
            float difficultyMult = _currentPartData != null ? _currentPartData.JudgementDifficulty : 1.0f;

            if (diff <= perfectWindow * difficultyMult)
            {
                HandleHit(targetNote, JudgementType.Perfect);
            }
            else if (diff <= greatWindow * difficultyMult)
            {
                HandleHit(targetNote, JudgementType.Great);
            }
            else if (diff <= goodWindow * difficultyMult)
            {
                HandleHit(targetNote, JudgementType.Good);
            }
        }

        private void HandleHit(NoteData note, JudgementType type)
        {
            Debug.Log($"<color=cyan>{type}</color> Diff: {Math.Abs(note.Time - _conductor.SongPosition):F3}");
            
            _lineController.RemoveNote(note);

            if (_scoreManager != null)
            {
                _scoreManager.RegisterResult(type, note.LaneIndex, note.MemberId);
            }

            Bus<NoteHitEvent>.Raise(new NoteHitEvent(type, note.LaneIndex, note.MemberId));
        }

        public void HandleMiss(NoteData note)
        {
            if (_scoreManager != null)
            {
                _scoreManager.RegisterResult(JudgementType.Miss, note.LaneIndex, note.MemberId);
            }

            Bus<NoteHitEvent>.Raise(new NoteHitEvent(JudgementType.Miss, note.LaneIndex, note.MemberId));
        }
    }
}