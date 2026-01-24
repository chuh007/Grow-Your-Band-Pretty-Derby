using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm
{
    public class RhythmLineController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform startPoint; 
        [SerializeField] private RectTransform endPoint;   
        [SerializeField] private RhythmPulse pulsePrefab;
        [SerializeField] private Transform pulseContainer;

        [Header("Settings")]
        [SerializeField] private float travelBeats = 1.0f; 

        [Inject] private Conductor _conductor;
        [Inject] private JudgementSystem _judgementSystem;

        private Queue<NoteData> _chartQueue = new Queue<NoteData>();
        private List<RhythmPulse> _activePulses = new List<RhythmPulse>();
        private Queue<RhythmPulse> _pulsePool = new Queue<RhythmPulse>();
        
        private LinkedList<NoteData> _activeHitNotes = new LinkedList<NoteData>();

        private bool _isPlaying = false;
        
        private class SequenceTracker
        {
            public NoteData Note;
            public int CurrentStep; 
            public double NextSpawnTime;
        }
        private List<SequenceTracker> _activeSequences = new List<SequenceTracker>();

        private void Start()
        {
            if (_conductor != null)
            {
                _conductor.OnSongStart += HandleSongStart;
                _conductor.OnSongEnd += HandleSongEnd;

                // 이미 재생 중이라면(이벤트를 놓쳤을 경우 대비) 바로 핸들러 호출
                if (_conductor.IsPlaying)
                {
                    HandleSongStart();
                }
            }
            else
            {
                Debug.LogError("[RhythmLineController] Conductor is missing!");
            }
            
            // 초기 설정 확인 로그
            Debug.Log($"[RhythmLineController] Initialized. StartPoint: {(startPoint != null ? startPoint.name : "NULL")}, EndPoint: {(endPoint != null ? endPoint.name : "NULL")}, PulseContainer: {(pulseContainer != null ? pulseContainer.name : "NULL")}");
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
            Debug.Log($"[RhythmLineController] Pulse Prefab Set: {(prefab != null ? prefab.name : "NULL")}");
        }

        public void SetChart(List<NoteData> notes)
        {
            _chartQueue.Clear();
            _activeHitNotes.Clear();
            _activeSequences.Clear();

            notes.Sort((a, b) => a.Time.CompareTo(b.Time));

            foreach (var note in notes)
            {
                _chartQueue.Enqueue(note);
            }
            Debug.Log($"[RhythmLineController] Chart Set. Total Notes: {notes.Count}. First Note Time: {(notes.Count > 0 ? notes[0].Time : 0)}");
        }

        private void HandleSongStart()
        {
            _isPlaying = true;
            Debug.Log("[RhythmLineController] Song Started. Playing...");
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
            _activeSequences.Clear();
            _activeHitNotes.Clear();
        }

        private void Update()
        {
            if (!_isPlaying || _conductor == null) return;

            double songTime = _conductor.SongPosition;
            double secPerBeat = _conductor.SecPerBeat;
            double travelDuration = travelBeats * secPerBeat;
            
            // 디버그용: 시간이 잘 흐르는지 가끔 체크
            // if (Time.frameCount % 300 == 0) Debug.Log($"[RhythmLineController] Time: {songTime:F2}, Queue: {_chartQueue.Count}, ActiveSeq: {_activeSequences.Count}");

            while (_chartQueue.Count > 0)
            {
                NoteData nextNote = _chartQueue.Peek();
                int seqLen = nextNote.SequenceLength > 0 ? nextNote.SequenceLength : 7;
                
                double firstPulseArrival = nextNote.Time - (seqLen - 1) * secPerBeat;
                double spawnThreshold = firstPulseArrival - travelDuration;

                if (songTime >= spawnThreshold)
                {
                    _chartQueue.Dequeue();
                    _activeSequences.Add(new SequenceTracker
                    {
                        Note = nextNote,
                        CurrentStep = 0,
                        NextSpawnTime = spawnThreshold 
                    });
                    
                    _activeHitNotes.AddLast(nextNote);
                    Debug.Log($"[RhythmLineController] Started Sequence for Note at {nextNote.Time:F2}. SpawnThreshold: {spawnThreshold:F2}");
                }
                else
                {
                    break;
                }
            }

            for (int i = _activeSequences.Count - 1; i >= 0; i--)
            {
                var seq = _activeSequences[i];
                int seqLen = seq.Note.SequenceLength > 0 ? seq.Note.SequenceLength : 7;

                double arrivalTime = seq.Note.Time - (seqLen - 1 - seq.CurrentStep) * secPerBeat;
                double exactSpawnTime = arrivalTime - travelDuration;

                if (songTime >= exactSpawnTime)
                {
                    bool isHit = (seq.CurrentStep == seqLen - 1);
                    SpawnPulse(seq.Note, arrivalTime, exactSpawnTime, isHit);

                    seq.CurrentStep++;
                    
                    if (seq.CurrentStep >= seqLen)
                    {
                        _activeSequences.RemoveAt(i);
                    }
                }
            }

            ProcessMissedNotes(songTime);
            
            // 비활성화된 펄스(Miss나 Hit로 인해 꺼진 것들)를 풀로 회수
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

        private void SpawnPulse(NoteData data, double targetTime, double spawnTime, bool isHit)
        {
            RhythmPulse pulse = GetFromPool();
            if (pulse == null) 
            {
                Debug.LogWarning("[RhythmLineController] Failed to spawn pulse: Pool empty or Prefab null.");
                return;
            }

            Vector3 sPos = startPoint != null ? startPoint.position : Vector3.zero;
            Vector3 ePos = endPoint != null ? endPoint.position : Vector3.zero;

            pulse.Initialize(_conductor, sPos, ePos, spawnTime, targetTime, isHit);
            _activePulses.Add(pulse);
            
            // Debug.Log($"[RhythmLineController] Spawned Pulse. Hit? {isHit}. Pos: {sPos} -> {ePos}");
        }

        private void ProcessMissedNotes(double songTime)
        {
            var node = _activeHitNotes.First;
            while (node != null)
            {
                var nextNode = node.Next;
                NoteData note = node.Value;
                
                if (songTime > note.Time + 0.2f)
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

        public NoteData GetNearestNote(double songTime)
        {
            if (_activeHitNotes.Count == 0) return null;
            return _activeHitNotes.First.Value; 
        }
        
        public void RemoveNote(NoteData note)
        {
             _activeHitNotes.Remove(note);
             
             foreach(var pulse in _activePulses)
             {
                 if (pulse.IsHitPulse && System.Math.Abs(pulse.TargetTime - note.Time) < 0.001f)
                 {
                     pulse.Deactivate();
                 }
             }
        }
    }
}