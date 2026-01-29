using UnityEngine;
using System;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;

using Code.MainSystem.Rhythm.Notes;
using Code.MainSystem.Rhythm.Audio;
using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.Judgement
{
    public class JudgementSystem : MonoBehaviour
    {
        [Header("Dependencies")]
        [Inject] private RhythmLineController _lineController;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private Conductor _conductor;

        [Header("Timing Windows (Seconds +/-)")]
        [SerializeField] private double perfectWindow = 0.050; 
        [SerializeField] private double greatWindow = 0.100;   
        [SerializeField] private double goodWindow = 0.150;
        
        [SerializeField] private double badWindow = 0.250; 
        
        [SerializeField] private double missWindow = 0.600;    

        private Dictionary<int, PartDataSO> _partDataMap = new Dictionary<int, PartDataSO>();

        public void SetPartData(List<PartDataSO> partDataList)
        {
            _partDataMap.Clear();
            foreach(var data in partDataList)
            {
                //TODO : 나중에 초기화 단계에서 할 게 있으면 채워넣기
            }
        }
        
        public void SetPartDataMap(Dictionary<int, PartDataSO> map)
        {
            _partDataMap = map;
        }

        public void OnInputDetected()
        {
            if (_lineController == null || _conductor == null) return;

            double songTime = _conductor.SongPosition;
            double compensatedTime = songTime + _conductor.InputOffset;

            NoteData targetNote = _lineController.GetClosestNoteAcrossAllTracks(compensatedTime);
            
            if (targetNote == null) return;

            double diff = Math.Abs(targetNote.Time - compensatedTime);
            
            float difficultyMult = 1.0f;
            if (_partDataMap.ContainsKey(targetNote.MemberId))
            {
                difficultyMult = _partDataMap[targetNote.MemberId].judgementDifficulty;
            }

            if (diff > missWindow * difficultyMult) 
            {
                return; 
            }

            if (diff <= perfectWindow * difficultyMult)
            {
                HandleInputResult(targetNote, JudgementType.Perfect);
            }
            else if (diff <= greatWindow * difficultyMult)
            {
                HandleInputResult(targetNote, JudgementType.Great);
            }
            else if (diff <= goodWindow * difficultyMult)
            {
                HandleInputResult(targetNote, JudgementType.Good);
            }
            else if (diff <= badWindow * difficultyMult)
            {
                HandleInputResult(targetNote, JudgementType.Bad);
            }
            else
            {
                HandleInputResult(targetNote, JudgementType.Miss);
            }
        }

        private void HandleInputResult(NoteData note, JudgementType type)
        {
            Debug.Log($"<color=cyan>Input Judgement: {type}</color> Diff: {Math.Abs(note.Time - _conductor.SongPosition):F3}");
            
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