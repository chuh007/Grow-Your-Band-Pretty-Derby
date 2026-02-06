namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class OverzealousEffect : MultiStatModifierEffect
    {
        public void OnTrainingComplete(object context)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= GetValue(0))
            {
                // TODO: 행동권 GetValue(1)만큼 추가
            }
        }
    }
}