using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{

    public class TraitRegistry : MonoBehaviour, ITraitRegistry
    {
        [SerializeField] private string label;
        private readonly Dictionary<int, TraitDataSO> _map = new();

        public async Task Initialize()
        {
            Debug.Assert(label != null, $"trait registry label is null in {name}");
            
            List<TraitDataSO> assets = await GameManager.Instance.LoadAllAddressablesAsync<TraitDataSO>(label);

            _map.Clear();
            foreach (var asset in assets.Where(a => a != null))
                _map[asset.IDHash] = asset;
        }

        /// <summary>
        /// Hash로 TraitDataSO를 가져옴
        /// </summary>
        /// <param name="hash">가져올 특성의 hash</param>
        /// <returns>특성 반환</returns>
        public TraitDataSO Get(int hash)
            => _map.GetValueOrDefault(hash);

        /// <summary>
        /// 찾고자 하는 특성이 있는지 확인
        /// </summary>
        /// <param name="traitHash">찾고자 하는 특성의 hash</param>
        /// <returns>있으면 true 없으면 false</returns>
        public bool Contains(int traitHash)
            => _map.ContainsKey(traitHash);
    }
}