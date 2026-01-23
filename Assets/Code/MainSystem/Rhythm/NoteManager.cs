using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;

namespace Code.MainSystem.Rhythm
{
    public class NoteManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float noteSpeed = 800.0f;
        [SerializeField] private float spawnDistance = 1200.0f;
        [SerializeField] private float hitLineYOffset = -600.0f; 
        
        [Tooltip("Assign the Lane UI Objects (Lane_0, Lane_1, etc.) here.")]
        [SerializeField] private List<RectTransform> laneContainers; 
        [SerializeField] private int laneCount = 4;

        [Inject] private Conductor _conductor;
        [Inject] private JudgementSystem _judgementSystem;

        private GameObject _notePrefab;
        private Queue<NoteData> _noteQueue = new Queue<NoteData>();
        
        private List<NoteObject>[] _laneNotes;

        private Queue<NoteObject> _notePool = new Queue<NoteObject>();
        private bool _externalChartLoaded = false;

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
            if (_conductor != null)
            {
                _conductor.OnSongStart += HandleSongStart;
                _conductor.OnSongEnd += HandleSongEnd;
            }
        }

        public void SetNotePrefab(GameObject prefab)
        {
            _notePrefab = prefab;
        }

        public void SetChart(List<NoteData> notes)
        {
            _noteQueue.Clear();
            foreach (var note in notes)
            {
                _noteQueue.Enqueue(note);
            }
            _externalChartLoaded = true;
            Debug.Log($"NoteManager: Chart Set via Bootstrapper. Total Notes: {notes.Count}");
        }

        private void OnDestroy()
        {
            if (_conductor != null)
            {
                _conductor.OnSongStart -= HandleSongStart;
                _conductor.OnSongEnd -= HandleSongEnd;
            }
        }

        private void HandleSongStart()
        {
            if (!_externalChartLoaded)
            {
                Debug.LogWarning("NoteManager: Song Started but no chart loaded!");
                return;
            }

            Debug.Log("NoteManager: Song Started.");
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



        private void Update()
        {
            if (_conductor == null) return;

            SpawnNotes();
            MoveNotes();
        }

        private void SpawnNotes()
        {
            if (_noteQueue.Count == 0) return;

            double currentSongTime = _conductor.SongPosition;
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
            double currentSongTime = _conductor.SongPosition;

            for (int lane = 0; lane < laneCount; lane++)
            {
                var list = _laneNotes[lane];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    NoteObject noteObj = list[i];
                    
                    float visualY = (float)((noteObj.Data.Time - currentSongTime) * noteSpeed) + hitLineYOffset;
                    noteObj.SetPosition(visualY);

                    if (visualY < hitLineYOffset - 200.0f) 
                    {
                        if (_judgementSystem != null)
                        {
                            _judgementSystem.HandleMiss(noteObj);
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
            
            if (laneContainers != null && data.LaneIndex < laneContainers.Count)
            {
                noteObj.transform.SetParent(laneContainers[data.LaneIndex], false);
            }

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
                if (_notePrefab == null)
                {
                    Debug.LogError("NoteManager: Note Prefab is not set!");
                    return null;
                }

                GameObject go = Instantiate(_notePrefab, transform);
                NoteObject noteObj = go.GetComponent<NoteObject>();
                if (noteObj == null) noteObj = go.AddComponent<NoteObject>();
                return noteObj;
            }
        }

        private void ReturnToPool(NoteObject obj)
        {
            obj.Deactivate();
            obj.transform.SetParent(transform, false);
            _notePool.Enqueue(obj);
        }
    }
}