using UnityEngine;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents; 

namespace Code.MainSystem.Rhythm
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
        [SerializeField] private int perfectScore = 100;
        [SerializeField] private int greatScore = 80;
        [SerializeField] private int goodScore = 50;
        [SerializeField] private int comboBonus = 10;

        private void Start()
        {
            Bus<SongEndEvent>.OnEvent += HandleSongEnd;
        }

        private void OnDestroy()
        {
            Bus<SongEndEvent>.OnEvent -= HandleSongEnd;
        }

        public void RegisterResult(JudgementType type, int laneIndex = -1)
        {
            int scoreToAdd = 0;

            switch (type)
            {
                case JudgementType.Perfect:
                    _perfectCount++;
                    scoreToAdd = perfectScore;
                    CurrentCombo++;
                    break;
                case JudgementType.Great:
                    _greatCount++;
                    scoreToAdd = greatScore;
                    CurrentCombo++;
                    break;
                case JudgementType.Good:
                    _goodCount++;
                    scoreToAdd = goodScore;
                    CurrentCombo++;
                    break;
                case JudgementType.Miss:
                    _missCount++;
                    CurrentCombo = 0;
                    break;
            }

            if (CurrentCombo > _maxCombo) _maxCombo = CurrentCombo;

            if (type != JudgementType.Miss && CurrentCombo > 1)
            {
                scoreToAdd += (CurrentCombo * comboBonus);
            }

            CurrentScore += scoreToAdd;

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
            
            Debug.Log($"ScoreManager: Report sent. Score: {CurrentScore}, Rank: {rank}");
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