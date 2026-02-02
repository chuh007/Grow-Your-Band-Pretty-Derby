using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Rhythm.Core;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.Rhythm.Test
{
    public class RhythmGameEnterButton : MonoBehaviour
    {
        public string testSongId = "TestSong";
        public ConcertType testConcertType = ConcertType.Busking;

        public void RequestConcertStart()
        {
            var memberGroup = new MemberGroup();
            memberGroup.Members = new List<MemberType>
            {
                MemberType.Vocal,
                MemberType.Guitar,
                MemberType.Bass,
                MemberType.Drums,
                MemberType.Piano
            };

            var members = new List<MemberGroup> { memberGroup };

            Debug.Log($"[RhythmGameEnterButton] Requesting concert: {testSongId}, {testConcertType}");
            Bus<ConcertStartRequested>.Raise(new ConcertStartRequested(testSongId, testConcertType, members));
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 200, 50), "Start Rhythm Game"))
            {
                RequestConcertStart();
            }
        }
    }
}
