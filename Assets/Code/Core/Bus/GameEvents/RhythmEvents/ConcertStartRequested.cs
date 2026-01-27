using System.Collections.Generic;
using Code.Core.Bus;

namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct ConcertStartRequested : IEvent
    {
        public string SongId;

        public ConcertStartRequested(string songId)
        {
            SongId = songId;
        }
    }
}
