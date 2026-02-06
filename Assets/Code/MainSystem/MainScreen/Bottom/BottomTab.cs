using System;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Bottom
{
    
    /// <summary>
    /// 하단 탭을 사용하는 행동들이 버튼에 들어왔을때나 다른버튼을 클릭했을때 행동을 처리해줄수있는코드
    /// (이거 인젝트로 사용하면 편하겠다 아니면 이벤트에다가 행동까지 싸줘서하는게 더 좋을거같기도하고 구조바꿀때 참고해야겠다)
    /// </summary>
    public class BottomTab : MonoBehaviour
    {
        private int currentModeIndex = 0;
        [SerializeField] private GameObject mode1Buttons;
        [SerializeField] private GameObject mode2Buttons;
        [SerializeField] private GameObject mode2Panel;
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo teamPracticeCompo;
        [SerializeField] private RestSelectCompo restSelectCompo;

        //연결 약하게할려고 이벤트사용함
        public Action<int> ExitModeEvent; 
        public Action<int> EnterModeEvent;

        private void OnEnable()
        {
            ExitModeEvent += ExitMode;
            EnterModeEvent += EnterMode;
        }

        private void OnDisable()
        {
            ExitModeEvent -= ExitMode;
            EnterModeEvent -= EnterMode;
        }
        
        //버튼에다가 붙여주는용
        public void SwitchMode(int modeIndex)
        {
            ExitMode(currentModeIndex);
            
            currentModeIndex = modeIndex;
            EnterMode(currentModeIndex);
        }

        private void EnterMode(int currentMode)
        {
            
            //버튼을 클릭했을때 들어오는 행동들 1.개인연습,2합주,3.휴식,4.외출,5.특성관리,6.??
            switch (currentMode)
            {
                case 1:
                    mode1Buttons.SetActive(true);
                    break;
                case 2:
                    mode2Buttons.SetActive(true);
                    mode2Panel.SetActive(true);
                    break;
                case 3:
                    restSelectCompo.Rest();
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }

        private void ExitMode(int currentMode)
        {
            switch (currentMode)
            {
                //다른버튼을 클릭했을때 들어오는 행동들 1.개인연습,2합주,3.휴식,4.외출,5.특성관리,6.??
                case 1:
                    mode1Buttons.SetActive(false);
                    personalPracticeCompo.ResetPreview();
                    break;
                case 2:
                    mode2Panel.SetActive(false);
                    mode2Buttons.SetActive(false);
                    teamPracticeCompo.OnClickBack();
                    break;
                case 3:
                    restSelectCompo.Close();
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }
    }
}