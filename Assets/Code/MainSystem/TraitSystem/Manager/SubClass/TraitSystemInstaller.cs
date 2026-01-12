using UnityEngine;
using Reflex.Core;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class TraitSystemInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(TraitPointCalculator), typeof(ITraitPointCalculator));
            builder.AddSingleton(typeof(TraitDatabase), typeof(ITraitDatabase));
        }
    }
}