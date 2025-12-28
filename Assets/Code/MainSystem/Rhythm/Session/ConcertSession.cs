using System.Collections.Generic;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm.Session
{
    public class ConcertSession
    {
        public string SongId { get; private set; }
        public List<int> MemberIds { get; private set; }
        public int Difficulty { get; private set; }
        
        public List<NoteData> CombinedChart { get; private set; }

        public RhythmGameResultEvent? Result { get; private set; }

        public ConcertSession(string songId, List<int> memberIds, int difficulty, List<NoteData> combinedChart)
        {
            SongId = songId;
            MemberIds = memberIds;
            Difficulty = difficulty;
            CombinedChart = combinedChart;
            Result = null;
        }

        public void SetResult(RhythmGameResultEvent result)
        {
            Result = result;
        }
    }
}
