using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class TargetUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI targetText;
        [SerializeField] private GameObject bar;

        private void Awake()
        {
            Bus<TargetSettingEvent>.OnEvent += HandleTargetSetting;
            Bus<TargetChangeEvent>.OnEvent += HandleTargetChange;
        }

        private void HandleTargetChange(TargetChangeEvent evt)
        {
            if (evt.changeTarget == "")
            {
                description.SetText("출전 조건 완료");
                targetText.gameObject.SetActive(false);
                icon.gameObject.SetActive(false);
            }
            else
            {
                if (targetText.gameObject.activeInHierarchy == false)
                {
                    targetText.gameObject.SetActive(true);
                }

                if (icon.gameObject.activeInHierarchy == false)
                {
                    icon.gameObject.SetActive(true);
                }
                description.SetText(evt.changeTarget);
            }
        }

        private void HandleTargetSetting(TargetSettingEvent evt)
        {
            if (evt.isTargetSet)
            {
                description.gameObject.SetActive(true);
                icon.gameObject.SetActive(true);
                targetText.gameObject.SetActive(true);
                bar.SetActive(true);
                title.SetText(evt.title);
                icon.sprite = evt.icon;
                targetText.SetText(evt.target);
            }
            else
            {
                title.SetText(evt.title);
                description.gameObject.SetActive(false);
                icon.gameObject.SetActive(false);
                targetText.gameObject.SetActive(true);
                bar.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Bus<TargetSettingEvent>.OnEvent -= HandleTargetSetting;
            Bus<TargetChangeEvent>.OnEvent -= HandleTargetChange;
        }
    }
}