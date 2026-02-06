using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Rhythm.Data;
using UnityEngine;

namespace Code.MainSystem.Rhythm.Test
{
    public class TestLchSceneButton : MonoBehaviour
    {
        public void EnterRhythmScene()
        {
            Bus<ConcertStartRequested>.Raise(new ConcertStartRequested("TestSong", ConcertType.Busking,
                RhythmGameConsts.MEMBERS_GROUP));
        }
    }
}