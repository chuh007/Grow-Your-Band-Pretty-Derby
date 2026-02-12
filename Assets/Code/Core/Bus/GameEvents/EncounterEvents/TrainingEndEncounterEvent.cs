using Code.MainSystem.MainScreen.MemberData;

namespace Code.Core.Bus.GameEvents.EncounterEvents
{
    public struct TrainingEndEncounterEvent : IEvent
    {
        public PersonalpracticeDataSO TrainingData;

        public TrainingEndEncounterEvent(PersonalpracticeDataSO trainingData)
        {
            TrainingData = trainingData;
        }
    }
}