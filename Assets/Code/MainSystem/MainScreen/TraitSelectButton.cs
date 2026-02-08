using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.TraitSystem.UI;

namespace Code.MainSystem.MainScreen
{
    public class TraitSelectButton : MonoBehaviour
    {
        [SerializeField] private TraitMainPanelUI traitUI;
        [SerializeField] private MainScreen mainScreen;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickMember);
        }
        
        private void OnClickMember()
        {
            traitUI.EnableFor(mainScreen.UnitSelector.CurrentUnit.memberType);
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

    }
}