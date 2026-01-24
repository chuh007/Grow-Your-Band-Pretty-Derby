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
            }
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
            _activeSequences.Clear();

            notes.Sort((a, b) => a.Time.CompareTo(b.Time));

            foreach (var note in notes)
            {
                _chartQueue.Enqueue(note);
            }
            Debug.Log($"RhythmLineController: Chart Set. {notes.Count} notes.");
        }

        private void HandleSongStart()
        {
            _isPlaying = true;
        }

        private void HandleSongEnd()
        {
            _isPlaying = false;
            ClearAll();
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
        }

        private void SpawnPulse(NoteData data, double targetTime, double spawnTime, bool isHit)
        {
            RhythmPulse pulse = GetFromPool();
            if (pulse == null) return;

            Vector3 sPos = startPoint != null ? startPoint.position : Vector3.zero;
            Vector3 ePos = endPoint != null ? endPoint.position : Vector3.zero;

            pulse.Initialize(_conductor, sPos, ePos, spawnTime, targetTime, isHit);
            _activePulses.Add(pulse);
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