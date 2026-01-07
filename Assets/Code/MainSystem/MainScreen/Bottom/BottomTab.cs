using Code.Core;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Bottom
{
    public class BottomTab : MonoBehaviour
    {
        private int currentModeIndex = 0;
        [SerializeField] private GameObject mode1Buttons;

        public void SwitchMode(int modeIndex)
        {
            ExitMode(currentModeIndex);
            
            currentModeIndex = modeIndex;
            EnterMode(currentModeIndex);
        }

        private void EnterMode(object currentMode)
        {
            switch (currentMode)
            {
                case 1:
                    mode1Buttons.SetActive(true);
                    break;
                case 2:
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

        private void ExitMode(object currentMode)
        {
            switch (currentMode)
            {
                case 1:
                    mode1Buttons.SetActive(false);
                    break;
                case 2:
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