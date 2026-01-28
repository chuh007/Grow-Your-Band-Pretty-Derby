#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.MainSystem.Rhythm.ChartEditor.Core;

namespace Code.MainSystem.Rhythm.ChartEditor.UI
{
    public class ChartEditorUIManager : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] private EditorManager editorManager;
        [SerializeField] private EditorAudioController audioController;

        [Header("Global Settings")]
        [SerializeField] private TMP_InputField songIdInput;
        [SerializeField] private TMP_InputField bpmInput;
        [SerializeField] private TMP_Dropdown snapDropdown;
        [SerializeField] private Toggle metronomeToggle;

        [Header("Members")]
        [SerializeField] private Button[] memberButtons;

        [Header("Transport")]
        [SerializeField] private Button playPauseButton;
        [SerializeField] private Button recordButton;
        [SerializeField] private Slider timeSlider; 
        [SerializeField] private TextMeshProUGUI timeText;
        
        [Header("File")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

        private void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (editorManager == null) editorManager = FindObjectOfType<EditorManager>();
            if (audioController == null) audioController = FindObjectOfType<EditorAudioController>();

            if (songIdInput) 
            {
                songIdInput.text = editorManager.SongId;
                songIdInput.onEndEdit.AddListener(val => editorManager.SetSongId(val));
            }

            if (bpmInput) 
            {
                bpmInput.text = editorManager.CurrentBPM.ToString();
                bpmInput.onEndEdit.AddListener(val => 
                {
                    if(double.TryParse(val, out double bpm)) editorManager.SetBPM(bpm);
                });
            }
            
            if (snapDropdown) snapDropdown.onValueChanged.AddListener(val => 
            {
                int divisor = 4;
                switch(val)
                {
                    case 0: divisor = 1; break;
                    case 1: divisor = 2; break;
                    case 2: divisor = 4; break;
                    case 3: divisor = 7; break;
                    case 4: divisor = 0; break;
                }
                editorManager.SetSnapDivisor(divisor);
            });

            if (metronomeToggle) metronomeToggle.onValueChanged.AddListener(val => audioController.ToggleMetronome(val));

            if (memberButtons != null)
            {
                for(int i=0; i<memberButtons.Length; i++)
                {
                    int id = i;
                    if(memberButtons[i] != null)
                        memberButtons[i].onClick.AddListener(() => editorManager.SetTargetMember(id));
                }
            }

            if (playPauseButton) playPauseButton.onClick.AddListener(() => editorManager.TogglePlayPause());
            
            if (recordButton) recordButton.onClick.AddListener(() => 
            {
                if(editorManager.CurrentState == EditorManager.EditorState.Recording)
                    editorManager.SetState(EditorManager.EditorState.Idle);
                else
                    editorManager.SetState(EditorManager.EditorState.Recording);
            });
            
            if (saveButton) saveButton.onClick.AddListener(OnSaveClicked);
            if (loadButton) loadButton.onClick.AddListener(OnLoadClicked);
        }

        private void Update()
        {
            if (audioController != null)
            {
                if (timeText) timeText.text = $"{audioController.CurrentTime:F2} / {audioController.Duration:F2}";
                if (timeSlider) 
                {
                    timeSlider.maxValue = audioController.Duration;
                    timeSlider.value = (float)audioController.CurrentTime;
                }
            }
        }

        private void OnSaveClicked()
        {
            EditorIO.SaveChart(editorManager.SongId, editorManager.TargetMemberId, editorManager.GetCurrentChart(), editorManager.CurrentBPM);
        }

        private void OnLoadClicked()
        {
             var notes = EditorIO.LoadChart(editorManager.SongId, editorManager.TargetMemberId);
             editorManager.SetChart(notes);
        }
    }
}

#endif