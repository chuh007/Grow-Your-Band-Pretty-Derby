#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.ChartEditor.UI
{
    public class EditorNote : MonoBehaviour
    {
        [SerializeField] private Image noteImage;
        [SerializeField] private RectTransform rectTransform;
        
        public NoteData Data { get; private set; }
        public bool IsSelected { get; private set; }

        private void Awake()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (noteImage == null) noteImage = GetComponent<Image>();
        }

        public void Initialize(NoteData data)
        {
            Data = data;
            UpdateVisuals();
        }

        public void UpdatePosition(float pixelsPerSecond)
        {
            if (rectTransform == null) return;
            
            float yPos = (float)Data.Time * pixelsPerSecond;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);
        }

        private void UpdateVisuals()
        {
            if (noteImage != null)
            {
                 noteImage.color = IsSelected ? Color.yellow : Color.white;
            }
        }
        
        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            UpdateVisuals();
        }
    }
}

#endif