using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class StatResultUI : MonoBehaviour
    {
        [SerializeField] private List<StatResultItemUI> resultItems;

        public async UniTask ShowStats(List<StatChangeResult> statResults)
        {
            foreach (var item in resultItems)
            {
                item.ResetUI();
            }
            
            for (int i = 0; i < resultItems.Count && i < statResults.Count; i++)
            {
                var ui = resultItems[i];
                var data = statResults[i];

                ui.SetInitialData(data.statName, data.rightIcon); 
                await ui.AnimateToValue(data.leftIcon, data.currentValue); 
                await UniTask.Delay(200);
            }
        }

    }

}