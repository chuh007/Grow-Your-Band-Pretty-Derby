using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.Turn;
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

        private async void Start()
        {
            if (StatManager.Instance.IsInitialized)
                TurnManager.Instance.UpdateTarget();
        }

        private void HandleTargetChange(TargetChangeEvent evt)
        {
            if (evt.ChangeTarget == "")
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
                description.SetText(evt.ChangeTarget);
            }
        }

        private void HandleTargetSetting(TargetSettingEvent evt)
        {
            if (evt.IsTargetSet)
            {
                description.gameObject.SetActive(true);
                icon.gameObject.SetActive(true);
                targetText.gameObject.SetActive(true);
                bar.SetActive(true);
                title.SetText(evt.Title);
                icon.sprite = evt.Icon;
                targetText.SetText(evt.Target.ToString());
                if (evt.Target <= 0)
                {
                    targetText.gameObject.SetActive(false);
                    icon.gameObject.SetActive(false);
                    description.SetText("목표 달성!");
                }
            }
            else
            {
                title.SetText(evt.Title);
                description.gameObject.SetActive(false);
                icon.gameObject.SetActive(false);
                targetText.gameObject.SetActive(false);
                bar.SetActive(false);
                if (evt.Target <= 0)
                {
                    targetText.gameObject.SetActive(false);
                    icon.gameObject.SetActive(false);
                    description.gameObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            Bus<TargetSettingEvent>.OnEvent -= HandleTargetSetting;
            Bus<TargetChangeEvent>.OnEvent -= HandleTargetChange;
        }
    }
}