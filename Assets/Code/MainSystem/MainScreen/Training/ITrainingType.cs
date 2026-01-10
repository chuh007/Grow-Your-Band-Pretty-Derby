using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    
    /// <summary>
    /// 팀훈련이랑 개인 훈련이 곂치는 부분이있어서 만든 인터페이스인데
    /// 만들다보니 곂치는데 매개변수가달라져서 좀 이상해짐
    /// 어드레서블이랑 그런거빼고 스탯관련된건 공통적으로써서 인터페이스로 만듬
    /// </summary>
    
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