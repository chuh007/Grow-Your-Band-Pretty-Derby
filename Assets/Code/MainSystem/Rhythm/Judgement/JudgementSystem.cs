using UnityEngine;
using System;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Rhythm.Notes;
using Code.MainSystem.Rhythm.Audio;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;

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

        private JudgementType GetJudgement(double diff, float difficultyMult, int memberId)
        {
            JudgementType initialType;

            if (diff <= perfectWindow * difficultyMult) 
                initialType = JudgementType.Perfect;
            else if (diff <= greatWindow * difficultyMult)
                initialType = JudgementType.Great;
            else if (diff <= goodWindow * difficultyMult)
                initialType = JudgementType.Good;
            else if (diff <= badWindow * difficultyMult) 
                initialType = JudgementType.Bad;
            else 
                initialType = JudgementType.Miss;

            return ApplyCorrection(initialType, memberId);
        }

        private JudgementType ApplyCorrection(JudgementType type, int memberId)
        {
            var holder = TraitManager.Instance.GetHolder((MemberType)memberId);
            if (holder == null) return type;

            return holder.CheckTriggerCondition(TraitTrigger.CheckSuccessGuaranteed, type) ? JudgementType.Good : type;
        }

        public void OnInputDetected()
        {
            if (_lineController == null || _conductor == null) return;

            double compensatedTime = _conductor.SongPosition + _conductor.InputOffset;
            NoteData targetNote = _lineController.GetClosestNoteAcrossAllTracks(compensatedTime);
    
            if (targetNote == null)
            {
                HandleEmptyTap();
                return;
            }

            double diff = Math.Abs(targetNote.Time - compensatedTime);
    
            float difficultyMult = 1.0f;
            if (_partDataMap.ContainsKey(targetNote.MemberId))
            {
                difficultyMult = _partDataMap[targetNote.MemberId].judgementDifficulty;
            }
            
            // 판정 범위 내에 들어온 경우
            if (diff <= missWindow * difficultyMult)
            {
                JudgementType finalType = GetJudgement(diff, difficultyMult, targetNote.MemberId);
                HandleInputResult(targetNote, finalType);
            }
            // 판정 범위보다 일찍 누른 경우 (Early Penalty)
            else if (targetNote.Time > compensatedTime && diff <= missWindow * 1.5 * difficultyMult)
            {
                // 너무 일찍 누른 경우 'Bad' 처리하고 노트를 제거함
                HandleInputResult(targetNote, JudgementType.Bad);
            }
            else
            {
                // 허공에 눌렀거나 너무 동떨어진 타이밍인 경우
                HandleEmptyTap();
            }
        }

        private void HandleEmptyTap()
        {
            // 노트를 제거하지 않고 콤보만 끊음
            if (_scoreManager != null)
            {
                _scoreManager.RegisterResult(JudgementType.Miss, -1, -1);
            }

            Bus<NoteHitEvent>.Raise(new NoteHitEvent(JudgementType.Miss, -1, -1));
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