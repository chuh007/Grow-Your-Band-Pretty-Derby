using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class StatResultUI : MonoBehaviour
    {
        [SerializeField] private List<StatResultItemUI> resultItems;

        public async UniTask ShowStats(List<StatChangeResult> statResults, CancellationToken token = default)
        {
            foreach (var item in resultItems)
            {
                item.ResetUI();
            }
            
            for (int i = 0; i < resultItems.Count && i < statResults.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                
                var ui = resultItems[i];
                var data = statResults[i];

                ui.SetInitialData(data.statName, data.rightIcon); 
                await ui.AnimateToValue(data.leftIcon, data.currentValue).AttachExternalCancellation(token); 
                await UniTask.Delay(200, cancellationToken: token);
            }
        }

        public void ClearStats()
        {
            foreach (var item in resultItems)
            {
                item.ResetUI();
                item.StopAnimation();
            }
        }
    }
}