using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents; 
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Reflex.Attributes;
using Code.MainSystem.Rhythm.Audio;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.Rhythm.Core
{
    public class RhythmGameLoop : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameHudPanel;
        [SerializeField] private GameObject resultPanel;

        [Header("In-Game HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        
        [Header("Result UI")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalComboText;
        [SerializeField] private TextMeshProUGUI rankText; 
        
        [Inject] private RhythmGameResultSender _resultSender;
        
        [Inject] private Conductor _conductor;

        private bool _isGameEnded = false;

        private void OnEnable()
        {
            Bus<RhythmGameResultEvent>.OnEvent += OnGameResultReceived;
            Bus<ScoreUpdateEvent>.OnEvent += OnScoreUpdated;
            Bus<SongEndEvent>.OnEvent += OnSongEnd;
        }

        private void OnDisable()
        {
            Bus<RhythmGameResultEvent>.OnEvent -= OnGameResultReceived;
            Bus<ScoreUpdateEvent>.OnEvent -= OnScoreUpdated;
            Bus<SongEndEvent>.OnEvent -= OnSongEnd;
        }

        private void Start()
        {
            if(startPanel != null) startPanel.SetActive(true);
            if(gameHudPanel != null) gameHudPanel.SetActive(false);
            if(resultPanel != null) resultPanel.SetActive(false);
        }

        private void OnSongEnd(SongEndEvent evt)
        {
            if (_isGameEnded) return;
            _isGameEnded = true;
            HandleGameEndSequence().Forget();
        }

        private async UniTaskVoid HandleGameEndSequence()
        {
            await UniTask.Delay(1000);
            if (_resultSender != null)
            {
                _resultSender.SendResult();
            }
        }

        private void OnScoreUpdated(ScoreUpdateEvent evt)
        {
            if (scoreText != null) scoreText.text = $"SCORE: {evt.CurrentScore}";
            if (comboText != null) 
            {
                comboText.text = evt.CurrentCombo > 0 ? $"{evt.CurrentCombo}" : "";
                // 콤보 애니메이션 등을 여기에 추가
            }
        }
        
        private void OnGameResultReceived(RhythmGameResultEvent evt)
        {
            ShowResult(evt);
        }
        
        private void ShowResult(RhythmGameResultEvent data)
        {
            if(gameHudPanel != null) gameHudPanel.SetActive(false);
            if(resultPanel != null) resultPanel.SetActive(true);
        
            if(finalScoreText != null) finalScoreText.text = $"SCORE: {data.FinalScore}";
            if(finalComboText != null) finalComboText.text = $"MAX COMBO: {data.MaxCombo}";
            
            if (rankText != null) 
                rankText.text = $"RANK: {data.Rank}";
        }
        
        public void OnStartButtonClicked()
        {
            if(startPanel != null) startPanel.SetActive(false);
            if(gameHudPanel != null) gameHudPanel.SetActive(true);
        
            if (_conductor != null)
            {
                _conductor.Play();
            }
        }

        public void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnExitToMainButtonClicked()
        {
            if (_resultSender != null)
            {
                _resultSender.SubmitResultAndExit();
            }
            else
            {
                Debug.LogWarning("RhythmGameLoop: ResultSender is missing!");
                SceneManager.LoadScene("MainScene");
            }
        }
    }
}