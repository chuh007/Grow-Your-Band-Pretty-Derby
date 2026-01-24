using System.Collections.Generic;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IModifierProvider
    {
        IEnumerable<T> GetModifiers<T>() where T : class;
        void RegisterModifier(object modifier);
        void UnregisterModifier(object modifier);
    }
}