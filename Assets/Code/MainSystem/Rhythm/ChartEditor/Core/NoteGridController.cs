#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.Rhythm.ChartEditor.UI;

namespace Code.MainSystem.Rhythm.ChartEditor.Core
{
    public class NoteGridController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private EditorNote notePrefab;
        [SerializeField] private RawImage waveformImage;
        [SerializeField] private EditorManager editorManager;
        [SerializeField] private EditorAudioController audioController;

        [Header("Grid Settings")]
        [SerializeField] private float zoomFactor = 100f;
        [SerializeField] private GameObject beatLinePrefab;
        
        private List<EditorNote> _spawnedNotes = new List<EditorNote>();
        private List<GameObject> _spawnedGridLines = new List<GameObject>();
        
        private double _currentBPM = 120.0;
        
        private void Start()
        {
            if (editorManager == null) editorManager = GetComponent<EditorManager>();
            if (audioController == null) audioController = GetComponent<EditorAudioController>();
        }

        public void UpdateGrid(double bpm)
        {
            _currentBPM = bpm;
            RefreshGridLines();
            RefreshNotesPosition();
        }

        public void RefreshNotesView(int memberId)
        {
             foreach(var note in _spawnedNotes)
             {
                 bool isTarget = note.Data.MemberId == memberId;
                 var img = note.GetComponent<Image>();
                 if(img != null)
                 {
                     var c = img.color;
                     c.a = isTarget ? 1.0f : 0.3f;
                     img.color = c;
                 }
             }
        }

        public void AddVisualNote(NoteData data)
        {
            if (notePrefab == null || contentContainer == null) return;
            
            EditorNote newNote = Instantiate(notePrefab, contentContainer);
            newNote.Initialize(data);
            newNote.UpdatePosition(zoomFactor);
            _spawnedNotes.Add(newNote);
        }

        public void RemoveVisualNote(NoteData data)
        {
            EditorNote target = _spawnedNotes.Find(n => n.Data == data);
            if (target != null)
            {
                _spawnedNotes.Remove(target);
                Destroy(target.gameObject);
            }
        }

        public void RefreshGridLines()
        {
            foreach(var line in _spawnedGridLines) Destroy(line);
            _spawnedGridLines.Clear();
            
            if (beatLinePrefab == null || audioController == null || contentContainer == null) return;
            
            float duration = audioController.Duration;
            if (duration <= 0) duration = 300f;

            float height = duration * zoomFactor;
            contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, height);

            double secPerBeat = 60.0 / _currentBPM;
            
            for (double t = 0; t < duration; t += secPerBeat)
            {
                GameObject line = Instantiate(beatLinePrefab, contentContainer);
                RectTransform rt = line.GetComponent<RectTransform>();
                if (rt != null)
                {
                    float yPos = (float)t * zoomFactor;
                    rt.anchoredPosition = new Vector2(0, yPos);
                }
                _spawnedGridLines.Add(line);
            }
        }
        
        private void RefreshNotesPosition()
        {
            foreach(var note in _spawnedNotes)
            {
                note.UpdatePosition(zoomFactor);
            }
        }
        
        public void SetZoom(float newZoom)
        {
            zoomFactor = Mathf.Clamp(newZoom, 10f, 1000f);
            RefreshGridLines();
            RefreshNotesPosition();
        }

        public void GenerateWaveform(AudioClip clip)
        {
            Debug.Log("[NoteGridController] Generating waveform...");
        }

        public float GetZoom() => zoomFactor;

        public double GetTimeFromLocalPoint(Vector2 localPoint)
        {
            return (double)(localPoint.y / zoomFactor);
        }
        
        public RectTransform GetContentRect() => contentContainer;

        public void SyncScroll(double time)
        {
            if(contentContainer == null) return;
             
            RectTransform viewport = contentContainer.parent as RectTransform;
            if(viewport == null) return;
             
            float currentY = (float)time * zoomFactor;
            float halfHeight = viewport.rect.height * 0.5f;
            
            contentContainer.anchoredPosition = new Vector2(contentContainer.anchoredPosition.x, -currentY + halfHeight);
        }
    }
}

#endif