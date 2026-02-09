using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Dialogue.UI
{
    /// <summary>
    /// 선택지 버튼 UI 스크립트
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DialogueChoiceButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI choiceText;
        
        private Button _button;
        private DialogueChoice _choiceData;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        /// <summary>
        /// 선택지 데이터 설정
        /// </summary>
        /// <param name="choice">표시할 선택지 데이터</param>
        public void Setup(DialogueChoice choice)
        {
            _choiceData = choice;
            if (choiceText != null)
            {
                choiceText.text = choice.ChoiceText;
            }
        }

        private void OnClick()
        {
            Bus<DialogueChoiceSelectedEvent>.Raise(new DialogueChoiceSelectedEvent(_choiceData.NextNodeIndex, _choiceData.Events));
        }
    }
}