using UnityEngine;
using System;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
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

        [Header("Base Timing Windows (Seconds +/-)")]
        [SerializeField] private double perfectWindow = 0.050; 
        [SerializeField] private double greatWindow = 0.100;   
        [SerializeField] private double goodWindow = 0.150;    

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

            // 수정: 모든 트랙을 대상으로 가장 가까운 노트를 가져옴
            NoteData targetNote = _lineController.GetClosestNoteAcrossAllTracks(compensatedTime);
            
            if (targetNote == null) return;

            double diff = Math.Abs(targetNote.Time - compensatedTime);
            
            // 해당 멤버에 대한 난이도 배수를 가져옴
            float difficultyMult = 1.0f;
            if (_partDataMap.ContainsKey(targetNote.MemberId))
            {
                difficultyMult = _partDataMap[targetNote.MemberId].judgementDifficulty;
            }

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
