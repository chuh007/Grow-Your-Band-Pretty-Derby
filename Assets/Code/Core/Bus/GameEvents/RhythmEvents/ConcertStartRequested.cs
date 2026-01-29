using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.Rhythm.Core;
using Code.MainSystem.Rhythm.Data;

namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct ConcertStartRequested : IEvent
    {
        public string SongId;
        public ConcertType ConcertType;
        public List<MemberGroup> Members;

        public ConcertStartRequested(string songId, ConcertType concertType, List<MemberGroup> members)
        {
            SongId = songId;
            ConcertType = concertType;
            Members = members;
        }
    }
}
