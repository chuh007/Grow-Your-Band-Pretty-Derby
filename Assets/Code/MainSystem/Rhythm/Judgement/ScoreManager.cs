using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents; 
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.BaseStats;

using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.Judgement
{
    public class ScoreManager : MonoBehaviour
    {
        public int CurrentScore { get; private set; }
        public int CurrentCombo { get; private set; }
        private int _maxCombo; 

        private int _perfectCount;
        private int _greatCount;
        private int _goodCount;
        private int _missCount;

        [Header("Score Config")]
        [SerializeField] private int basePerfectScore = 100;
        [SerializeField] private int baseGreatScore = 80;
        [SerializeField] private int baseGoodScore = 50;
        [SerializeField] private int comboBonus = 10;
        
        [Header("Part Data")]
        [SerializeField] private List<PartDataSO> partDataList;

        [Inject] private FeverManager _feverManager; 

        // StatManager 의존성 제거, 대신 배율 값을 저장
        private Dictionary<int, float> _memberStatMultipliers = new Dictionary<int, float>();

        private void Start()
        {
            ResetScore();
            Bus<SongEndEvent>.OnEvent += HandleSongEnd;
        }
        
        // Bootstrapper가 호출하여 스탯 배율 설정
        public void SetMemberStatMultiplier(int memberId, float multiplier)
        {
            if (_memberStatMultipliers.ContainsKey(memberId))
            {
                _memberStatMultipliers[memberId] = multiplier;
            }
            else
            {
                _memberStatMultipliers.Add(memberId, multiplier);
            }
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            CurrentCombo = 0;
            _maxCombo = 0;
            _perfectCount = 0;
            _greatCount = 0;
            _goodCount = 0;
            _missCount = 0;

            Bus<ScoreUpdateEvent>.Raise(new ScoreUpdateEvent(0, 0, JudgementType.Perfect, -1));
        }

        private void OnDestroy()
        {
            Bus<SongEndEvent>.OnEvent -= HandleSongEnd;
        }

        public void RegisterResult(JudgementType type, int laneIndex, int memberId)
        {
            float baseScore = 0;

            switch (type)
            {
                case JudgementType.Perfect:
                    _perfectCount++;
                    baseScore = basePerfectScore;
                    CurrentCombo++;
                    break;
                case JudgementType.Great:
                    _greatCount++;
                    baseScore = baseGreatScore;
                    CurrentCombo++;
                    break;
                case JudgementType.Good:
                    _goodCount++;
                    baseScore = baseGoodScore;
                    CurrentCombo++;
                    break;
                case JudgementType.Miss:
                    _missCount++;
                    CurrentCombo = 0;
                    break;
            }

            if (CurrentCombo > _maxCombo) _maxCombo = CurrentCombo;

            float partMult = 1.0f;
            if (partDataList != null && memberId >= 0 && memberId < partDataList.Count)
            {
                if (partDataList[memberId] != null)
                    partMult = partDataList[memberId].ScoreMultiplier;
            }

            // 미리 설정된 스탯 배율 사용
            float statMult = 1.0f;
            if (_memberStatMultipliers.TryGetValue(memberId, out float mult))
            {
                statMult = mult;
            }
            
            float feverMult = (_feverManager != null && _feverManager.IsFeverActive) ? 1.5f : 1.0f;

            float finalScoreAdded = 0;
            
            if (type != JudgementType.Miss)
            {
                float comboBonusVal = (CurrentCombo > 1) ? (CurrentCombo * comboBonus) : 0;
                finalScoreAdded = (baseScore + comboBonusVal) * partMult * statMult * feverMult;
            }

            CurrentScore += (int)finalScoreAdded;

            Bus<ScoreUpdateEvent>.Raise(new ScoreUpdateEvent(CurrentScore, CurrentCombo, type, laneIndex));
        }

        private void HandleSongEnd(SongEndEvent evt)
        {
            string rank = CalculateRank(CurrentScore);

            Bus<RhythmGameResultEvent>.Raise(new RhythmGameResultEvent(
                CurrentScore, 
                _maxCombo, 
                rank, 
                _perfectCount, 
                _greatCount, 
                _goodCount, 
                _missCount
            ));
        }

        private string CalculateRank(int score)
        {
            if (score >= 100000) return "S";
            if (score >= 80000) return "A";
            if (score >= 60000) return "B";
            return "C";
        }
    }
}