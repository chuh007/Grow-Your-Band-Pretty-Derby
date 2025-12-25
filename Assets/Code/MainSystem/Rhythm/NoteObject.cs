using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    public class NoteObject : MonoBehaviour
    {
        public NoteData Data { get; private set; }
        
        public void Initialize(NoteData data)
        {
            this.Data = data;
            SetLanePosition(data.LaneIndex);
            
            gameObject.SetActive(true);
        }

        private void SetLanePosition(int laneIndex)
        {
            float laneWidth = 1.5f; 
            float startX = -2.25f; 
            
            float xPos = startX + (laneIndex * laneWidth);
            transform.localPosition = new Vector3(xPos, transform.localPosition.y, 0);
        }

        public void SetPosition(float yPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, yPos, 0);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}