using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Outing
{
    public class OutingResultUI : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO resultSender;
        [SerializeField] private TextMeshProUGUI resultText;
        
        // 포메팅용. 스텟 이름은 일반 텍스트. 증가량은 녹색
        private static readonly string RESULT_FORMAT = "{0} <color=#00FFAC>+{1}</color>"; 
        
        public async UniTask ShowResultUI()
        {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("결과\n");
            foreach (var stat in resultSender.changeStats)
            {
                resultBuilder.Append(string.Format(RESULT_FORMAT, stat.targetStat.ToString(), stat.variation));
                resultBuilder.AppendLine();
            }
            resultText.SetText(resultBuilder.ToString());
            await UniTask.Delay(1000);
            SceneManager.UnloadSceneAsync("OutingScene");
        }
    }
}