using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Outing
{
    // 외출 버튼에 달아놓는 컴포넌트
    [RequireComponent(typeof(SceneLoadButton))]
    public class OutingButton : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO resultSender;
        [SerializeField] private MainScreen.MainScreen mainScreen;
        
        private SceneLoadButton _loadButton;

        private void Awake()
        {
            _loadButton = GetComponent<SceneLoadButton>();
        }

        public void LoadAndDataSend()
        {
            resultSender.targetMember = mainScreen.UnitSelector.CurrentUnit;
            _loadButton.SceneLoadAdditive("OutingScene");
        }
    }
}