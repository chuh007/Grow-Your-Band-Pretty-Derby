using UnityEngine;
using System.Collections.Generic;

namespace Code.MainSystem.Rhythm
{
    public class NoteManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private float noteSpeed = 5.0f;
        [SerializeField] private float spawnDistance = 10.0f; 
        [SerializeField] private Transform noteContainer; 
        [SerializeField] private int laneCount = 4;

        private Queue<NoteData> _noteQueue = new Queue<NoteData>();
        
        private List<NoteObject>[] _laneNotes;

        private Queue<NoteObject> _notePool = new Queue<NoteObject>();

        private void Awake()
        {
            _laneNotes = new List<NoteObject>[laneCount];
            for(int i=0; i<laneCount; i++)
            {
                _laneNotes[i] = new List<NoteObject>();
            }
        }

        private void Start()
        {
            if (Conductor.Instance != null)
            {
                Conductor.Instance.OnSongStart += HandleSongStart;
                Conductor.Instance.OnSongEnd += HandleSongEnd;
            }
        }

        private void OnDestroy()
        {
            if (Conductor.Instance != null)
            {
                Conductor.Instance.OnSongStart -= HandleSongStart;
                Conductor.Instance.OnSongEnd -= HandleSongEnd;
            }
        }

        private void HandleSongStart()
        {
            Debug.Log("NoteManager: Song Started. Loading Chart...");
            LoadChartData();
        }

        private void HandleSongEnd()
        {
            Debug.Log("NoteManager: Song Ended. Clearing notes...");
            ClearAllNotes();
        }

        private void ClearAllNotes()
        {
            for (int i = 0; i < laneCount; i++)
            {
                var list = _laneNotes[i];
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    ReturnToPool(list[j]);
                }
                list.Clear();
            }
            _noteQueue.Clear();
        }

        private void LoadChartData()
        {
            _noteQueue.Clear();
            
            List<NoteData> notes = ChartLoader.LoadTestChart();
            
            foreach (var note in notes)
            {
                _noteQueue.Enqueue(note);
            }
            
            Debug.Log($"NoteManager: Loaded {notes.Count} notes from ChartLoader.");
        }

        private void Update()
        {
            if (Conductor.Instance == null) return;

            SpawnNotes();
            MoveNotes();
        }

        private void SpawnNotes()
        {
            if (_noteQueue.Count == 0) return;

            double currentSongTime = Conductor.Instance.SongPosition;
            double timeToReach = spawnDistance / noteSpeed;
            double spawnThresholdTime = currentSongTime + timeToReach;

            while (_noteQueue.Count > 0 && _noteQueue.Peek().Time <= spawnThresholdTime)
            {
                NoteData noteData = _noteQueue.Dequeue();
                ActivateNote(noteData);
            }
        }

        private void MoveNotes()
        {
            double currentSongTime = Conductor.Instance.SongPosition;

            for (int lane = 0; lane < laneCount; lane++)
            {
                var list = _laneNotes[lane];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    NoteObject noteObj = list[i];
                    
                    float visualY = (float)((noteObj.Data.Time - currentSongTime) * noteSpeed);
                    noteObj.SetPosition(visualY);

                    if (visualY < -5.0f) 
                    {
                        if (JudgementSystem.Instance != null)
                        {
                            JudgementSystem.Instance.HandleMiss();
                        }
                        
                        ReturnToPool(noteObj);
                        list.RemoveAt(i);
                    }
                }
            }
        }

        private void ActivateNote(NoteData data)
        {
            if (data.LaneIndex < 0 || data.LaneIndex >= laneCount) return;

            NoteObject noteObj = GetFromPool();
            noteObj.Initialize(data);
            
            _laneNotes[data.LaneIndex].Add(noteObj);
        }

        public NoteObject GetNearestNote(int laneIndex)
        {
            if (laneIndex < 0 || laneIndex >= laneCount) return null;

            var list = _laneNotes[laneIndex];
            if (list.Count == 0) return null;

            return list[0];
        }

        public void DespawnNote(NoteObject note)
        {
            int lane = note.Data.LaneIndex;
            if (lane < 0 || lane >= laneCount) return;

            _laneNotes[lane].Remove(note);
            
            ReturnToPool(note);
        }

        private NoteObject GetFromPool()
        {
            if (_notePool.Count > 0)
            {
                return _notePool.Dequeue();
            }
            else
            {
                GameObject go = Instantiate(notePrefab, noteContainer != null ? noteContainer : transform);
                NoteObject noteObj = go.GetComponent<NoteObject>();
                if (noteObj == null) noteObj = go.AddComponent<NoteObject>();
                return noteObj;
            }
        }

        private void ReturnToPool(NoteObject obj)
        {
            obj.Deactivate();
            _notePool.Enqueue(obj);
        }
    }
}