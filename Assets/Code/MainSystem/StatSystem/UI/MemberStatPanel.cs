using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.MemberStats;
using TMPro;

namespace Code.MainSystem.StatSystem.UI
{
    public class MemberStatPanel : MonoBehaviour, IUIElement<MemberStat>
    {
         [SerializeField] private TextMeshProUGUI memberNameText;
        [SerializeField] private Transform statContainer;
        [SerializeField] private StatUI statUIPrefab;
        
        private MemberStat _memberStat;
        private readonly List<StatUI> _statUIs = new();
        
        public void EnableFor(MemberStat stats)
        {
            _memberStat = stats;
            gameObject.SetActive(stats != null);
            
            if (_memberStat != null)
            {
                ClearStatUIs();
                SetupStatUIs();
            }
        }
        
        public void UpdateUI()
        {
            if (_memberStat != null)
            {
                foreach (var statUI in _statUIs)
                {
                    statUI.UpdateUI();
                }
            }
        }
        
        private void SetupStatUIs()
        {
            memberNameText.text = _memberStat.MemberType.ToString();
            
            CreateStatUI(StatType.Condition);
            CreateStatUI(StatType.Mental);
            
            switch (_memberStat.MemberType)
            {
                case MemberType.Guitar:
                    CreateStatUI(StatType.GuitarEndurance);
                    CreateStatUI(StatType.GuitarConcentration);
                    break;
                case MemberType.Drums:
                    CreateStatUI(StatType.DrumsSenseOfRhythm);
                    CreateStatUI(StatType.DrumsPower);
                    break;
                case MemberType.Bass:
                    CreateStatUI(StatType.BassDexterity);
                    CreateStatUI(StatType.BassSenseOfRhythm);
                    break;
                case MemberType.Vocal:
                    CreateStatUI(StatType.VocalVocalization);
                    CreateStatUI(StatType.VocalBreathing);
                    break;
                case MemberType.Piano:
                    CreateStatUI(StatType.PianoDexterity);
                    CreateStatUI(StatType.PianoStagePresence);
                    break;
            }
        }
        
        private void CreateStatUI(StatType statType)
        {
            BaseStat stat = _memberStat.GetStat(statType);
            if (stat != null)
            {
                StatUI ui = Instantiate(statUIPrefab, statContainer);
                ui.EnableFor(stat);
                _statUIs.Add(ui);
            }
        }
        
        private void ClearStatUIs()
        {
            foreach (var statUI in _statUIs)
                Destroy(statUI.gameObject);
            
            _statUIs.Clear();
        }
    }
}