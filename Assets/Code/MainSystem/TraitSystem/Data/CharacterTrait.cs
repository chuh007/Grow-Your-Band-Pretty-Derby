using System.Collections.Generic;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Data
{
    public class CharacterTrait : MonoBehaviour
    {
        [SerializeField] private List<TraitDataSO> traitData;
        [SerializeField] private int currentLevel;
        
        public List<TraitDataSO> TraitData => traitData;
        public int CurrentLevel => currentLevel;
    }
}