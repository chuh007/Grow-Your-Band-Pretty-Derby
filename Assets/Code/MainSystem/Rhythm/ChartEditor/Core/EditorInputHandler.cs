#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Code.MainSystem.Rhythm.ChartEditor.Core
{
    public class EditorInputHandler : MonoBehaviour
    {
        [SerializeField] private EditorManager manager;
        [SerializeField] private EditorAudioController audioController;
        [SerializeField] private NoteGridController gridController;
        
        private void Start()
        {
            if (manager == null) manager = GetComponent<EditorManager>();
            if (audioController == null) audioController = GetComponent<EditorAudioController>();
            if (gridController == null) gridController = GetComponent<NoteGridController>();
        }

        private void Update()
        {
            if (Keyboard.current == null || Mouse.current == null) return;

            HandleGlobalShortcuts();
            
            if (manager.CurrentState == EditorManager.EditorState.Recording)
            {
                HandleRecordingInput();
            }
            else
            {
                HandleEditInput();
            }
        }

        private void HandleGlobalShortcuts()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                manager.TogglePlayPause();
            }
            
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) audioController.SeekStep(-1);
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) audioController.SeekStep(1);
        }

        private void HandleEditInput()
        {
            bool isCtrlPressed = Keyboard.current.ctrlKey.isPressed;
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();
            float scrollY = scrollDelta.y;

            if (isCtrlPressed)
            {
                if (Mathf.Abs(scrollY) > 0.01f)
                {
                     float zoomChange = Mathf.Sign(scrollY) * 10f; 
                     float currentZoom = gridController.GetZoom();
                     gridController.SetZoom(currentZoom + zoomChange);
                }
            }
            else
            {
                if (Mathf.Abs(scrollY) > 0.01f)
                {
                    float normalizedScroll = scrollY / 120f; 
                    double seekAmount = normalizedScroll * (60.0 / manager.CurrentBPM); 
                    audioController.Seek(audioController.CurrentTime + seekAmount);
                }
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

                RectTransform gridRect = gridController.GetContentRect();
                if (gridRect == null) return;
                
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, mousePos, null, out localPoint))
                {
                    double time = gridController.GetTimeFromLocalPoint(localPoint);
                    if (time >= 0)
                    {
                        manager.AddNote(time);
                    }
                }
            }
            
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                 RectTransform gridRect = gridController.GetContentRect();
                 if (gridRect == null) return;

                 Vector2 mousePos = Mouse.current.position.ReadValue();
                 Vector2 localPoint;
                 if (RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, mousePos, null, out localPoint))
                 {
                     double time = gridController.GetTimeFromLocalPoint(localPoint);
                     
                     double tolerance = 20f / gridController.GetZoom(); 
                     
                     var chart = manager.GetCurrentChart();
                     var target = chart.Find(n => System.Math.Abs(n.Time - time) < tolerance && n.MemberId == manager.TargetMemberId);
                     
                     if (target != null)
                     {
                         manager.RemoveNote(target);
                     }
                 }
            }
        }

        private void HandleRecordingInput()
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame) 
            {
                 double time = audioController.CurrentTime;
                 manager.AddNote(time);
            }
        }
    }
}
#endif