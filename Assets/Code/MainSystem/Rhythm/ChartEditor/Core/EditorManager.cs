#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;
using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.ChartEditor.Core
{
    public class EditorManager : MonoBehaviour
    {
        public enum EditorState
        {
            Idle,
            Playing,
            Recording
        }

        [Header("State")]
        [SerializeField] private EditorState currentState = EditorState.Idle;
        [SerializeField] private int targetMemberId = 0;
        [SerializeField] private double currentBPM = 120.0;
        [SerializeField] private string songId = "NewSong";
        [SerializeField] private int snapDivisor = 4;

        [Header("References")]
        [SerializeField] private EditorAudioController audioController;
        [SerializeField] private NoteGridController gridController;

        private List<NoteData> _currentChart = new List<NoteData>();
        
        public EditorState CurrentState => currentState;
        public int TargetMemberId => targetMemberId;
        public double CurrentBPM => currentBPM;
        public string SongId => songId;
        public int SnapDivisor => snapDivisor;

        public void SetSongId(string id)
        {
            songId = id;
        }

        public void SetChart(List<NoteData> newNotes)
        {
            foreach(var note in new List<NoteData>(_currentChart))
            {
                RemoveNote(note);
            }
            _currentChart.Clear();
            
            foreach(var note in newNotes)
            {
                _currentChart.Add(note);
                gridController.AddVisualNote(note);
            }
        }

        private void Start()
        {
            if (audioController == null) audioController = GetComponent<EditorAudioController>();
            if (gridController == null) gridController = GetComponent<NoteGridController>();
            
            InitializeEditor();
        }

        private void InitializeEditor()
        {
            SetBPM(currentBPM);
        }

        public void SetBPM(double bpm)
        {
            currentBPM = bpm;
            if (audioController != null) audioController.SetBPM(bpm);
            if (gridController != null) gridController.UpdateGrid(bpm);
        }

        public void SetState(EditorState newState)
        {
            currentState = newState;
        }
        
        public void TogglePlayPause()
        {
            if (currentState == EditorState.Playing || currentState == EditorState.Recording)
            {
                SetState(EditorState.Idle);
                audioController.Pause();
            }
            else
            {
                SetState(EditorState.Playing);
                audioController.Play();
            }
        }

        public void SetTargetMember(int memberId)
        {
            targetMemberId = memberId;
            gridController.RefreshNotesView(memberId);
        }

        public void SetSnapDivisor(int divisor)
        {
            snapDivisor = divisor;
        }

        public double GetSnappedTime(double rawTime)
        {
            if (snapDivisor <= 0) return rawTime;

            double beatDuration = 60.0 / currentBPM;
            double snapInterval = beatDuration / snapDivisor;
            
            return System.Math.Round(rawTime / snapInterval) * snapInterval;
        }

        public void AddNote(double time)
        {
            double finalTime = GetSnappedTime(time);
            
            if (_currentChart.Exists(n => System.Math.Abs(n.Time - finalTime) < 0.001 && n.MemberId == targetMemberId))
            {
                return;
            }

            NoteData newNote = new NoteData(finalTime, 0, 0, targetMemberId);
            _currentChart.Add(newNote);
            
            gridController.AddVisualNote(newNote);
        }

        public void RemoveNote(NoteData note)
        {
            if (_currentChart.Contains(note))
            {
                _currentChart.Remove(note);
                gridController.RemoveVisualNote(note);
            }
        }
        
        public List<NoteData> GetCurrentChart()
        {
            return _currentChart;
        }

        private void Update()
        {
            if(audioController != null && gridController != null)
            {
                gridController.SyncScroll(audioController.CurrentTime);
            }
        }
    }
}

#endif