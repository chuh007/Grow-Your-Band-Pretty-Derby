using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Data
{
    public class TraitDatabase : MonoBehaviour, ITraitDatabase
    {
        [SerializeField] private string traitLabel;
        public bool IsInitialized { get; private set; }
        
        private Dictionary<int, TraitDataSO> _map = new();
        
        public async Task InitializeAsync()
        {
            List<TraitDataSO> allTraits = await GameManager.Instance.LoadAllAddressablesAsync<TraitDataSO>(traitLabel);

            _map.Clear();
            foreach (var trait in allTraits.Where(trait => trait != null))
                _map[trait.IDHash] = trait;
        }

        public TraitDataSO Get(int traitID)
        {
            return _map.GetValueOrDefault(traitID);
        }
    }
}