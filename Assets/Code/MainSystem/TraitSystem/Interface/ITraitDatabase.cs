using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitDatabase
    {
        TraitDataSO Get(int traitID);
    }
}