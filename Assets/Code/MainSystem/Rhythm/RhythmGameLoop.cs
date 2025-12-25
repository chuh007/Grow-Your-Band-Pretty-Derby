using Code.Core.Bus;
using Code.Core.Bus.GameEvents; 
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm
{
    public class RhythmGameLoop : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameHudPanel;
        [SerializeField] private GameObject resultPanel;
        
        [Header("Result UI")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalComboText;
        [SerializeField] private TextMeshProUGUI rankText; 
        
        private void Start()
        {
            if(startPanel != null) startPanel.SetActive(true);
            if(gameHudPanel != null) gameHudPanel.SetActive(false);
            if(resultPanel != null) resultPanel.SetActive(false);
        
            Bus<RhythmGameResultEvent>.OnEvent += OnGameResultReceived;
        }
        
        private void OnDestroy()
        {
            Bus<RhythmGameResultEvent>.OnEvent -= OnGameResultReceived;
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
        
            if (Conductor.Instance != null)
            {
                Conductor.Instance.Play();
            }
        }

        public void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}