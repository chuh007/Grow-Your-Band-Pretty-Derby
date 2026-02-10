using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class InjuryEffect : MultiStatModifierEffect, ITurnProcessListener
    {
        private int _elapsedTurns = 0;

        public void OnTurnPassed()
        {
            _elapsedTurns++;

            if (_elapsedTurns < (int)GetValue(2)) 
                return;
            
            Bus<TraitRemoveRequested>.Raise(new TraitRemoveRequested(_activeTrait.Owner,
                _activeTrait.Data.TraitID));
            _elapsedTurns = 0;
        }
    }
}