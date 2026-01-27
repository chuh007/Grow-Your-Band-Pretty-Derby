using UnityEngine;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;

using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.Judgement
{
    public class FeverManager : MonoBehaviour
    {
        [Header("Fever Settings")]
        [SerializeField] private int pointsPerGreat = 5;
        [SerializeField] private int pointsPerPerfect = 10;
        [SerializeField] private int maxFeverGauge = 1000;
        [SerializeField] private float baseFeverDuration = 10.0f;

        // StatManager 의존성 제거

        private int _currentGauge = 0;
        private bool _isFeverActive = false;
        private float _feverTimer = 0f;
        
        private float _statBonusDuration = 0f;

        public bool IsFeverActive => _isFeverActive;
        
        // Bootstrapper가 호출하여 보너스 시간 설정
        public void SetStatBonusDuration(float duration)
        {
            _statBonusDuration = duration;
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
                if (_feverTimer <= 0)
                {
                    EndFever();
                }
            }
        }

        private void HandleNoteHit(NoteHitEvent evt)
        {
            if (_isFeverActive) return; 

            if (evt.Judgement == JudgementType.Perfect)
            {
                _currentGauge += pointsPerPerfect;
            }
            else if (evt.Judgement == JudgementType.Great)
            {
                _currentGauge += pointsPerGreat;
            }
            
            if (_currentGauge >= maxFeverGauge)
            {
                StartFever();
            }
        }

        private void StartFever()
        {
            _isFeverActive = true;
            _currentGauge = 0;
            
            // 미리 주입받은 보너스 사용
            _feverTimer = baseFeverDuration + _statBonusDuration;
            
            Debug.Log($"<color=orange>FEVER MODE ACTIVATED!</color> Duration: {_feverTimer:F1}s");
        }

        private void EndFever()
        {
            _isFeverActive = false;
            Debug.Log("<color=orange>FEVER MODE ENDED</color>");
        }
    }
}