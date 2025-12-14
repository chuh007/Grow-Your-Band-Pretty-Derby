using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;

namespace Code.MainSystem.Etc
{
    public class UnitSelector
    {
        private readonly Dictionary<string, UnitDataSO> unitDict;
        public UnitDataSO CurrentUnit { get; private set; }

        public UnitSelector(List<UnitDataSO> unitList)
        {
            unitDict = new Dictionary<string, UnitDataSO>();

            foreach (var unit in unitList)
            {
                var key = unit.memberType.ToString();
                if (!unitDict.ContainsKey(key))
                {
                    unitDict.Add(key, unit);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Duplicate key detected: {key}");
                }
            }
        }

        public bool TryGetUnit(string type, out UnitDataSO unit)
        {
            bool found = unitDict.TryGetValue(type, out unit);
            if (found)
            {
                CurrentUnit = unit;
            }
            return found;
        }
    }
}