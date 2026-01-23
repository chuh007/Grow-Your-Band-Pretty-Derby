namespace Code.MainSystem.Rhythm
{
    [System.Serializable]
    public class NoteData
    {
        public double Time;      
        public int LaneIndex;    
        public int Type;
        public int MemberId;
        
        public int SequenceLength;
        public int BeatIndex;

        public NoteData(double time, int laneIndex, int type = 0, int memberId = 0, int sequenceLength = 7, int beatIndex = 0)
        {
            Time = time;
            LaneIndex = laneIndex;
            Type = type;
            MemberId = memberId;
            SequenceLength = sequenceLength;
            BeatIndex = beatIndex;
        }
    }
}