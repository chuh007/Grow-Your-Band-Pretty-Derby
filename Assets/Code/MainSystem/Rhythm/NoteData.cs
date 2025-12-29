namespace Code.MainSystem.Rhythm
{
    [System.Serializable]
    public class NoteData
    {
        public double Time;      
        public int LaneIndex;    
        public int Type;
        public int MemberId;

        public NoteData(double time, int laneIndex, int type = 0, int memberId = 0)
        {
            Time = time;
            LaneIndex = laneIndex;
            Type = type;
            MemberId = memberId;
        }
    }
}