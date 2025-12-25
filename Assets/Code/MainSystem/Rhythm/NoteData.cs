namespace Code.MainSystem.Rhythm
{
    [System.Serializable]
    public class NoteData
    {
        public double Time;      
        public int LaneIndex;    
        public int Type;         

        public NoteData(double time, int laneIndex, int type = 0)
        {
            Time = time;
            LaneIndex = laneIndex;
            Type = type;
        }
    }
}