using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

public class TeamTrainingType : ITeamTraingType, ITeamStatChangeProvider
{
    private Dictionary<MemberType, UnitDataSO> _unitDataSOs = new Dictionary<MemberType, UnitDataSO>();
    
    public TeamTrainingType(List<UnitDataSO> unitDataList)
    {
        foreach (var unit in unitDataList)
        {
            _unitDataSOs[unit.memberType] = unit;
        }
    }

    public string GetProgressImageKey()
    {
        return "Concert/Image/Progress";
    }

    public string GetIdleImageKey(MemberType memberType)
    {
        return _unitDataSOs[memberType].TeamIdleSpriteKey;
    }

    public string GetResultImageKey(bool isSuccess, MemberType memberType)
    {
        return isSuccess ? _unitDataSOs[memberType].TeamSuccseSpriteKey : _unitDataSOs[memberType].TeamFaillSpriteKey;
    }

    public string GetProgressImageKey(MemberType memberType)
    {
        return _unitDataSOs[memberType].TeamProggresSpriteKey;
    }

    public string GetIdlePrefabKey()
    {
        return "Concert/SD/Idle";
    }

    public string GetResultUIPrefabKey()
    {
        return "Concert/SD/Result";
    }

    public Dictionary<UnitDataSO, List<(string name, Sprite icon, int baseValue, int delta)>> GetAllStatChanges(
        List<UnitDataSO> units,
        StatManager statManager,
        bool isSuccess)
    {
        var result = new Dictionary<UnitDataSO, List<(string name, Sprite icon, int baseValue, int delta)>>();

        foreach (var unit in units)
        {
            int baseHarmony = statManager.GetTeamStat(StatType.TeamHarmony).CurrentValue;
            int delta = isSuccess ? 5 : 0;

            Sprite icon = statManager.GetTeamStat(StatType.TeamHarmony).StatIcon;

            result[unit] = new List<(string, Sprite, int, int)>
            {
                ("하모니", icon, baseHarmony, delta)
            };
        }

        return result;
    }
}