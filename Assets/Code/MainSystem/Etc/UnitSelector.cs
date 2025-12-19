using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using UnityEngine;

namespace Code.MainSystem.Etc
{
    public class UnitSelector
    {
        private readonly Dictionary<string, UnitDataSO> unitDict = new();
        public UnitDataSO CurrentUnit { get; private set; }

        public void Init(List<UnitDataSO> units)
        {
            unitDict.Clear();
            foreach (var unit in units)
            {
                string key = unit.memberType.ToString();
                if (!unitDict.ContainsKey(key))
                    unitDict.Add(key, unit);
            }
        }

        public bool TryGetUnit(string type, out UnitDataSO unit)
        {
            bool found = unitDict.TryGetValue(type, out unit);
            if (found)
                CurrentUnit = unit;
            return found;
        }
    }
}