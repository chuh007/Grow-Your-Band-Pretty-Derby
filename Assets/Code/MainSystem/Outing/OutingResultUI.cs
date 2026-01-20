using System.Collections.Generic;
using System.Text;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    public class OutingResultUI : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO resultSender;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button closeButton;
        
        // 포메팅용. 스텟 이름은 일반 텍스트. 증가량은 녹색
        private static readonly string RESULT_FORMAT = "{0} <color=#00FFAC>+{1}</color>";
        private static readonly string SKILL_RESULT_FORMAT = "<color=#00FFAC>{0}</color> 획득";
        
        public void ShowResultUI()
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseOutingScene);
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("결과\n");
            foreach (var stat in resultSender.changeStats)
            {
                resultBuilder.Append(string.Format(RESULT_FORMAT, stat.targetStat.ToString(), stat.variation));
                resultBuilder.AppendLine();
            }
            
            foreach (var skill in resultSender.addedTraits)
            {
                resultBuilder.Append(string.Format(SKILL_RESULT_FORMAT, skill.ToString()));
            }
            resultText.SetText(resultBuilder.ToString());
        }
        
        private void CloseOutingScene()
        {
            closeButton.onClick.RemoveAllListeners();
            Bus<OutingEndEvent>.Raise(new OutingEndEvent()); 
            SceneManager.UnloadSceneAsync("OutingScene");
        }
    }
}