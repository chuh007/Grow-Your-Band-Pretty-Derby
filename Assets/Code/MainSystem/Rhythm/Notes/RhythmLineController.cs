using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.Rhythm.Audio;
using Code.MainSystem.Rhythm.Judgement;

namespace Code.MainSystem.Rhythm.Notes
{
    public class RhythmLineController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform startPoint; 
        [SerializeField] private RectTransform endPoint;   
        [SerializeField] private RhythmPulse pulsePrefab;
        [SerializeField] private Transform pulseContainer;

        [Header("Settings")]
        [SerializeField] private int pulseSteps = 4;

        [Inject] private Conductor _conductor;
        [Inject] private JudgementSystem _judgementSystem;

        private Queue<NoteData> _chartQueue = new Queue<NoteData>();
        private List<RhythmPulse> _activePulses = new List<RhythmPulse>();
        private Queue<RhythmPulse> _pulsePool = new Queue<RhythmPulse>();
        
        private LinkedList<NoteData> _activeHitNotes = new LinkedList<NoteData>();

        private bool _isPlaying = false;
        private double _lastNoteTime = 0.0; // 이전 노트의 시간 (다음 노트의 출발 시간)

        private void Start()
        {
            if (_conductor != null)
            {
                _conductor.OnSongStart += HandleSongStart;
                _conductor.OnSongEnd += HandleSongEnd;

                if (_conductor.IsPlaying)
                {
                    HandleSongStart();
                }
            }
            else
            {
                Debug.LogError("[RhythmLineController] Conductor is missing!");
            }
            
            Debug.Log($"[RhythmLineController] Initialized. StartPoint: {(startPoint != null ? startPoint.name : "NULL")}, EndPoint: {(endPoint != null ? endPoint.name : "NULL")}");
        }

        private void OnDestroy()
        {
            if (_conductor != null)
            {
                _conductor.OnSongStart -= HandleSongStart;
                _conductor.OnSongEnd -= HandleSongEnd;
            }
        }

        public void SetPulsePrefab(RhythmPulse prefab)
        {
            pulsePrefab = prefab;
        }

        public void SetChart(List<NoteData> notes)
        {
            _chartQueue.Clear();
            _activeHitNotes.Clear();
            
            notes.Sort((a, b) => a.Time.CompareTo(b.Time));

            foreach (var note in notes)
            {
                _chartQueue.Enqueue(note);
            }
            
            // 첫 노트 전의 딜레이를 위해 초기화
            _lastNoteTime = 0.0;
            
            Debug.Log($"[RhythmLineController] Chart Set. Total Notes: {notes.Count}");
        }

        private void HandleSongStart()
        {
            _isPlaying = true;
            _lastNoteTime = 0.0; 
            Debug.Log("[RhythmLineController] Song Started.");
        }

        private void HandleSongEnd()
        {
            _isPlaying = false;
            ClearAll();
            Debug.Log("[RhythmLineController] Song Ended.");
        }

        private void ClearAll()
        {
            foreach (var pulse in _activePulses)
            {
                ReturnToPool(pulse);
            }
            _activePulses.Clear();
            _activeHitNotes.Clear();
        }

        private void Update()
        {
            if (!_isPlaying || _conductor == null) return;

            double songTime = _conductor.SongPosition;

            // 1. 펄스 생성 로직: 현재 활성화된 펄스가 없고, 대기 중인 노트가 있다면 생성
            // 단, 노래가 시작된 직후(_lastNoteTime 근처)여야 함
            if (_activePulses.Count == 0 && _chartQueue.Count > 0)
            {
                // 이전 노트 시간(출발 시간)이 현재 시간보다 작거나 같으면 출발 가능
                // (약간의 오차 허용 혹은 바로 출발)
                if (songTime >= _lastNoteTime)
                {
                    NoteData nextNote = _chartQueue.Dequeue();
                    
                    // 출발 시간: 이전 노트의 시간
                    double spawnTime = _lastNoteTime;
                    
                    // 이번 노트는 다음 펄스의 출발 시간이 됨
                    _lastNoteTime = nextNote.Time;

                    SpawnPulse(nextNote, spawnTime);
                    _activeHitNotes.AddLast(nextNote);
                }
            }

            // 2. 미스 처리 (판정선을 너무 지나친 경우)
            ProcessMissedNotes(songTime);
            
            // 3. 사용 완료된 펄스 회수
            for (int i = _activePulses.Count - 1; i >= 0; i--)
            {
                var pulse = _activePulses[i];
                if (!pulse.gameObject.activeSelf)
                {
                    ReturnToPool(pulse);
                    _activePulses.RemoveAt(i);
                }
            }
        }

        private void SpawnPulse(NoteData data, double spawnTime)
        {
            RhythmPulse pulse = GetFromPool();
            if (pulse == null) 
            {
                return;
            }

            Vector3 sPos = startPoint != null ? startPoint.position : Vector3.zero;
            Vector3 ePos = endPoint != null ? endPoint.position : Vector3.zero;

            // TargetTime = 노트의 판정 시간
            // SpawnTime = 이전 노트의 시간 (즉, 출발 시간)
            // isHitPulse = true (이제 모든 펄스는 실제 타격 노트임)
            // pulseSteps를 7번째 인자로 명확히 전달
            pulse.Initialize(_conductor, sPos, ePos, spawnTime, data.Time, true, pulseSteps);
            _activePulses.Add(pulse);
            
            Debug.Log($"[RhythmLineController] Pulse Spawned. From {spawnTime:F2} to {data.Time:F2} (Duration: {data.Time - spawnTime:F2}s), Steps: {pulseSteps}");
        }

        private void ProcessMissedNotes(double songTime)
        {
            var node = _activeHitNotes.First;
            while (node != null)
            {
                var nextNode = node.Next;
                NoteData note = node.Value;
                
                // 판정 시간 + 0.2초가 지나도록 처리가 안 되면 Miss
                if (songTime > note.Time + RhythmGameBalanceConsts.MISS_THRESHOLD_SECONDS)
                {
                    if (_judgementSystem != null)
                    {
                        _judgementSystem.HandleMiss(note);
                    }
                    _activeHitNotes.Remove(node);
                    Debug.Log($"[RhythmLineController] Missed Note at {note.Time:F2}");
                }
                
                node = nextNode;
            }
        }
        
        private RhythmPulse GetFromPool()
        {
            if (_pulsePool.Count > 0)
            {
                return _pulsePool.Dequeue();
            }
            if (pulsePrefab != null)
            {
                return Instantiate(pulsePrefab, pulseContainer != null ? pulseContainer : transform);
            }
            return null;
        }

        private void ReturnToPool(RhythmPulse pulse)
        {
            pulse.Deactivate();
            _pulsePool.Enqueue(pulse);
        }

        public NoteData GetClosestNoteAcrossAllTracks(double inputTime)
        {
            if (_activeHitNotes.Count == 0) return null;

            NoteData closestNote = null;
            double minDiff = double.MaxValue;
            double window = RhythmGameBalanceConsts.MISS_THRESHOLD_SECONDS; // Valid window

            foreach (var note in _activeHitNotes)
            {
                double diff = System.Math.Abs(note.Time - inputTime);
                if (diff < minDiff && diff <= window)
                {
                    minDiff = diff;
                    closestNote = note;
                }
            }
            return closestNote;
        }
        
        public void RemoveNote(NoteData note)
        {
             _activeHitNotes.Remove(note);
             
             // 해당 노트와 연결된 펄스를 찾아 비활성화
             foreach(var pulse in _activePulses)
             {
                 // 펄스의 목표 시간이 노트 시간과 거의 일치하면 제거
                 if (System.Math.Abs(pulse.TargetTime - note.Time) < 0.001f)
                 {
                     pulse.Deactivate();
                 }
             }
        }
    }
}