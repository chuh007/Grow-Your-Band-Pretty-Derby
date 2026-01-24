using System.Collections.Generic;
using Code.MainSystem.Rhythm.SceneTransition;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.Test
{
    public class RhythmGameEnterButton : MonoBehaviour
    {
        [Header("Data Senders")]
        [SerializeField] private RhythmGameDataSenderSO _dataSender;
        [SerializeField] private SceneTransitionSenderSO _transitionSender;

        [Header("Test Settings")]
        [SerializeField] private string _songId = "TestSong";
        [SerializeField] private ConcertType _concertType = ConcertType.Busking;
        [SerializeField] private int _difficulty = 1;
        [SerializeField] private string _rhythmSceneName = "Rhythm";
        [SerializeField] private string _transitionSceneName = "TransitionScene";

        public void EnterRhythmGame()
        {
            if (_dataSender == null || _transitionSender == null)
            {
                Debug.LogError("[Test] DataSender or TransitionSender is not assigned!");
                return;
            }

            _dataSender.SongId = _songId;
            _dataSender.ConcertType = _concertType;
            _dataSender.Difficulty = _difficulty;
            _dataSender.IsResultDataAvailable = false;
            
            _dataSender.MemberIds = new List<int> { 1, 2, 3, 4, 5 };
            _dataSender.members = new List<MemberGroup>
            {
                new MemberGroup { Members = new List<MemberType> { MemberType.Vocal } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Guitar } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Bass } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Drums } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Piano } }
            };

            _transitionSender.SetTransition(_rhythmSceneName, TransitionMode.ToLandscape);

            Debug.Log($"[Test] Entering Rhythm Game: {_songId} ({_concertType})");
            SceneManager.LoadScene(_transitionSceneName);
        }
    }
}