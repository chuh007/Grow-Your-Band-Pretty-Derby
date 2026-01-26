using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Code.MainSystem.Rhythm.Core;
using Code.MainSystem.Rhythm.SceneTransition;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.Rhythm.Test
{
    public class RhythmGameTransitionTester : MonoBehaviour
    {
        [Header("System References (Required)")]
        [SerializeField] private RhythmGameDataSenderSO dataSender;
        [SerializeField] private SceneTransitionSenderSO transitionSender;

        [Header("UI References")]
        [SerializeField] private GameObject entrancePanel;

        [Header("Default Settings (Fallback)")]
        [SerializeField] private string defaultSongId = "TestSong";
        [SerializeField] private int defaultDifficulty = 1;
        [SerializeField] private List<int> memberIds = new List<int> { 1, 2, 3, 4, 5 };

        private void Start()
        {
            if (entrancePanel != null) entrancePanel.SetActive(false);
        }

        public void OpenEntrancePanel()
        {
            if (entrancePanel != null) entrancePanel.SetActive(true);
        }

        public void CloseEntrancePanel()
        {
            if (entrancePanel != null) entrancePanel.SetActive(false);
        }

        public void EnterRhythmGame()
        {
            string sId = defaultSongId;
            int diff = defaultDifficulty;

            Debug.Log($"[Test] Starting Rhythm Game directly: {sId} (Diff: {diff})");

            // 1. DataSender 설정
            if (dataSender != null)
            {
                dataSender.songId = defaultSongId;
                dataSender.memberIds = new List<int>(memberIds);
                dataSender.difficulty = defaultDifficulty;
                dataSender.isResultDataAvailable = false;
                
                // 임시 멤버 구성 (5인조 기본값)
                dataSender.members = new List<MemberGroup>
                {
                    new MemberGroup { Members = new List<MemberType> { MemberType.Vocal } },
                    new MemberGroup { Members = new List<MemberType> { MemberType.Guitar } },
                    new MemberGroup { Members = new List<MemberType> { MemberType.Bass } },
                    new MemberGroup { Members = new List<MemberType> { MemberType.Drums } },
                    new MemberGroup { Members = new List<MemberType> { MemberType.Piano } }
                };
            }
            else
            {
                Debug.LogError("[Test] RhythmGameDataSenderSO is not assigned! Please assign it in Inspector.");
                return;
            }

            // 2. TransitionSender 설정 및 씬 로드
            if (transitionSender != null)
            {
                transitionSender.SetTransition("Rhythm", TransitionMode.ToLandscape);
                SceneManager.LoadScene("TransitionScene");
            }
            else
            {
                Debug.LogError("[Test] SceneTransitionSenderSO is not assigned! Please assign it in Inspector.");
                // 비상시 직접 로드 시도
                // SceneManager.LoadScene("Rhythm");
            }
            
            if (entrancePanel != null) entrancePanel.SetActive(false);
        }
    }
}