using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    public class NoteObject : MonoBehaviour
    {
        public NoteData Data { get; private set; }
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(NoteData data)
        {
            this.Data = data;
            
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            
            _rectTransform.anchoredPosition = Vector2.zero; 
            _rectTransform.localRotation = Quaternion.identity;
            _rectTransform.localScale = Vector3.one;
            
            gameObject.SetActive(true);
        }

        public void SetPosition(float yPos)
        {
            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition = new Vector2(0, yPos);
            }
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}