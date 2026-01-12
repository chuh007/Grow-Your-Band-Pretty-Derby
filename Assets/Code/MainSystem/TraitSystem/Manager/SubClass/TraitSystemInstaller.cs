using Code.MainSystem.TraitSystem.Interface;
using Reflex.Core;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class TraitSystemInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(TraitPointCalculator), typeof(ITraitPointCalculator));
        }
    }
}