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
                string key = unit.memberType.ToString().ToLower();
                if (!unitDict.ContainsKey(key))
                    unitDict.Add(key, unit);
            }
        }

        public bool TryGetUnit(string type, out UnitDataSO unit)
        {
            bool found = unitDict.TryGetValue(type.ToLower(), out unit);
            if (found)
                CurrentUnit = unit;
            return found;
        }

        /// <summary>
        /// 현재 선택된 유닛을 외부에서 설정
        /// (캐러셀에서 선택된 유닛과 동기화하기 위해 사용)
        /// </summary>
        public void SetCurrentUnit(UnitDataSO unit)
        {
            CurrentUnit = unit;
        }
    }
}