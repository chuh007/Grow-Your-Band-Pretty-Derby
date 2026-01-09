using System;
using Code.Core;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Bottom
{
    public class BottomTab : MonoBehaviour
    {
        private int currentModeIndex = 0;
        [SerializeField] private GameObject mode1Buttons;
        [SerializeField] private GameObject mode2Buttons;
        [SerializeField] private GameObject mode2Panel;
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo teamPracticeCompo;
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

        public void SwitchMode(int modeIndex)
        {
            ExitMode(currentModeIndex);
            
            currentModeIndex = modeIndex;
            EnterMode(currentModeIndex);
        }

        private void EnterMode(int currentMode)
        {
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