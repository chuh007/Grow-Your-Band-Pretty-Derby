using System.Collections.Generic;
using Code.Core.Bus;

namespace Code.Core.Bus.GameEvents
{
    public struct ConcertStartRequested : IEvent
    {
        public string SongId;
        public List<int> MemberIds;
        public int Difficulty;

        public ConcertStartRequested(string songId, List<int> memberIds, int difficulty)
        {
            SongId = songId;
            MemberIds = memberIds;
            Difficulty = difficulty;
        }
    }
}
