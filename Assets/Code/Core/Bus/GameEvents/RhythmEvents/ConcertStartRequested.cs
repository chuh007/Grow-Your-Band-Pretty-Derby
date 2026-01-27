using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.Rhythm.Core;

namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct ConcertStartRequested : IEvent
    {
        public string SongId;
        public List<MemberGroup> Members;

        public ConcertStartRequested(string songId, List<MemberGroup> members)
        {
            SongId = songId;
            Members = members;
        }
    }
}
