using UnityEngine;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;

namespace Code.MainSystem.Rhythm.Judgement
{
    public class FeverManager : MonoBehaviour
    {
        [Header("Fever Settings")]
        [SerializeField] private int pointsPerGreat = 5;
        [SerializeField] private int pointsPerPerfect = 10;
        [SerializeField] private float baseMaxFeverGauge = 1000f;
        [SerializeField] private float baseFeverDuration = 10.0f;

        private float _currentGauge = 0;
        private float _runtimeMaxGauge;
        private bool _isFeverActive = false;
        private float _feverTimer = 0f;
        private bool _hasMissedSinceLastFever = false;
        
        private float _statBonusDuration = 0f;

        public bool IsFeverActive => _isFeverActive;

        // Bootstrapper가 호출하여 보너스 시간 설정
        public void SetStatBonusDuration(float duration)
        {
            _statBonusDuration = duration;
        }

        private void Awake()
        {
            _runtimeMaxGauge = baseMaxFeverGauge;
        }

        private void Start()
        {
            Bus<NoteHitEvent>.OnEvent += HandleNoteHit;
        }

        private void OnDestroy()
        {
            Bus<NoteHitEvent>.OnEvent -= HandleNoteHit;
        }

        private void Update()
        {
            if (_isFeverActive)
            {
                _feverTimer -= Time.deltaTime;
                if (_feverTimer <= 0) EndFever();
            }
        }

        private void HandleNoteHit(NoteHitEvent evt)
        {
            if (evt.TrackIndex < 0) return;

            if (evt.Judgement == JudgementType.Miss)
            {
                _hasMissedSinceLastFever = true;
                var holder = TraitManager.Instance.GetHolder((MemberType)evt.TrackIndex);
                _runtimeMaxGauge = holder.GetCalculatedStat(TraitTarget.FeverInput, baseFeverDuration);
                return;
            }

            if (_isFeverActive) return; 

            if (evt.Judgement == JudgementType.Perfect) _currentGauge += pointsPerPerfect;
            else if (evt.Judgement == JudgementType.Great) _currentGauge += pointsPerGreat;
            
            if (_currentGauge >= _runtimeMaxGauge)
            {
                StartFever(evt.TrackIndex);
            }
        }

        private void StartFever(int trackIndex)
        {
            _isFeverActive = true;
            _currentGauge = 0;
            
            var holder = TraitManager.Instance.GetHolder((MemberType)trackIndex);
            float finalTraitDuration = holder.GetCalculatedStat(TraitTarget.FeverTime, baseFeverDuration);
            
            // 미리 주입받은 보너스 사용
            _feverTimer = finalTraitDuration + _statBonusDuration;
            Debug.Log($"<color=orange>FEVER MODE ACTIVATED!</color> Duration: {_feverTimer:F1}s");
        }

        private void EndFever()
        {
            _isFeverActive = false;
            _hasMissedSinceLastFever = false;
            _runtimeMaxGauge = baseMaxFeverGauge;
        }

        public float GetFeverScoreMultiplier(int trackIndex)
        {
            if (!_isFeverActive) return 1.0f;
            var holder = TraitManager.Instance.GetHolder((MemberType)trackIndex);
            return holder.GetCalculatedStat(TraitTarget.FeverScore, 1.0f);
        }
    }
}