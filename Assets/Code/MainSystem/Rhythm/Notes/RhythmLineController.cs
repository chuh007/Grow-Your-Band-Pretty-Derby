using UnityEngine;
using System;
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
        private double _lastNoteTime = 0.0; 

        private void Start()
        {
            if (_conductor != null)
            {
                _conductor.OnSongStart += HandleSongStart;
                _conductor.OnSongEnd += HandleSongEnd;

                if (_conductor.IsPlaying) HandleSongStart();
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

        public void SetPulsePrefab(RhythmPulse prefab) => pulsePrefab = prefab;

        public void SetChart(List<NoteData> notes)
        {
            _chartQueue.Clear();
            _activeHitNotes.Clear();
            notes.Sort((a, b) => a.Time.CompareTo(b.Time));
            foreach (var note in notes) _chartQueue.Enqueue(note);
            _lastNoteTime = 0.0;
        }

        private void HandleSongStart()
        {
            _isPlaying = true;
            _lastNoteTime = 0.0;
        }

        private void HandleSongEnd()
        {
            _isPlaying = false;
            ClearAll();
        }

        private void ClearAll()
        {
            foreach (var pulse in _activePulses) ReturnToPool(pulse);
            _activePulses.Clear();
            _activeHitNotes.Clear();
        }

        private void Update()
        {
            if (!_isPlaying || _conductor == null) return;

            double songTime = _conductor.SongPosition;

            // 순차적 생성 로직 복구
            if (_activePulses.Count == 0 && _chartQueue.Count > 0)
            {
                if (songTime >= _lastNoteTime)
                {
                    NoteData nextNote = _chartQueue.Dequeue();
                    double spawnTime = _lastNoteTime;
                    _lastNoteTime = nextNote.Time;

                    SpawnPulse(nextNote, spawnTime);
                    _activeHitNotes.AddLast(nextNote);
                }
            }

            ProcessMissedNotes(songTime);
            
            for (int i = _activePulses.Count - 1; i >= 0; i--)
            {
                var pulse = _activePulses[i];
                if (!pulse.gameObject.activeSelf)
                {
                    _pulsePool.Enqueue(pulse);
                    _activePulses.RemoveAt(i);
                }
            }
        }

        private void SpawnPulse(NoteData data, double spawnTime)
        {
            RhythmPulse pulse = GetFromPool();
            if (pulse == null) return;

            Vector3 sPos = startPoint != null ? startPoint.localPosition : Vector3.zero;
            Vector3 ePos = endPoint != null ? endPoint.localPosition : Vector3.zero;

            pulse.Initialize(_conductor, sPos, ePos, spawnTime, data.Time, true, pulseSteps);
            _activePulses.Add(pulse);
        }

        private void ProcessMissedNotes(double songTime)
        {
            var node = _activeHitNotes.First;
            while (node != null)
            {
                var nextNode = node.Next;
                NoteData note = node.Value;
                if (songTime > note.Time + RhythmGameBalanceConsts.MISS_THRESHOLD_SECONDS)
                {
                    if (_judgementSystem != null) _judgementSystem.HandleMiss(note);
                    _activeHitNotes.Remove(node);
                }
                node = nextNode;
            }
        }
        
        private RhythmPulse GetFromPool()
        {
            if (_pulsePool.Count > 0) return _pulsePool.Dequeue();
            if (pulsePrefab != null) return Instantiate(pulsePrefab, pulseContainer != null ? pulseContainer : transform);
            return null;
        }

        private void ReturnToPool(RhythmPulse pulse) => pulse.Deactivate();

        public void RemoveNote(NoteData note)
        {
             _activeHitNotes.Remove(note);
             foreach(var pulse in _activePulses)
             {
                 if (Math.Abs(pulse.TargetTime - note.Time) < 0.001f) pulse.Deactivate();
             }
        }

        public NoteData GetClosestNoteAcrossAllTracks(double inputTime)
        {
            if (_activeHitNotes.Count == 0) return null;
            NoteData closestNote = null;
            double minDiff = double.MaxValue;
            foreach (var note in _activeHitNotes)
            {
                double diff = Math.Abs(note.Time - inputTime);
                if (diff < minDiff && diff <= RhythmGameBalanceConsts.MISS_THRESHOLD_SECONDS)
                {
                    minDiff = diff;
                    closestNote = note;
                }
            }
            return closestNote;
        }
    }
}