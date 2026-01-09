using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public interface ITrainingType
    {
        string GetIdleImageKey();
        string GetResultImageKey(bool isSuccess);
        string GetProgressImageKey();

        string GetIdlePrefabKey();   
        string GetResultUIPrefabKey();
    }

    public interface ITeamTraingType
    {
        string GetIdleImageKey(MemberType memberType);
        string GetResultImageKey(bool isSuccess,MemberType memberType);
        string GetProgressImageKey(MemberType memberType);

        string GetIdlePrefabKey();   
        string GetResultUIPrefabKey();
    }


    public interface IStatChangeProvider
    {
        List<(string name, Sprite icon, int baseValue, int delta)> GetStatChanges(
            UnitDataSO unit, StatManager statManager, bool isSuccess);
    }

    public interface ITeamStatChangeProvider
    {
        Dictionary<UnitDataSO, List<(string name, Sprite icon, int baseValue, int delta)>> GetAllStatChanges(
            List<UnitDataSO> units, StatManager statManager, bool isSuccess);
    }



}