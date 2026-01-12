using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Reflex.Attributes;
using UnityEditor.Animations;

public class PersonalPracticeView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject succesImage;
    [SerializeField] private GameObject failImage;
    [SerializeField] private Sprite conditoinSprite;

    private bool isSkipped;

    public async UniTask Play(MemberActionData actionData, bool isSuccess,PersonalpracticeDataSO dataSo, float currentConditoin,StatManager statManager)
    {
        animator.runtimeAnimatorController = actionData.animator;
        isSkipped = false;

        characterImage.sprite = actionData.sprite;
        animator.Play(actionData.startAnimationName);

        await WaitOrSkip(1.0f);
        
        succesImage.SetActive(isSuccess);
        failImage.SetActive(!isSuccess);
        
        animator.Play(isSuccess ? "Success" : "Fail");

        if (isSuccess)
        {
            Debug.Log("이벤트가요");
            BaseStat stat = statManager.GetMemberStat(actionData.memberType, dataSo.PracticeStatType);

            Bus<StatIncreaseDecreaseEvent>.Raise(new StatIncreaseDecreaseEvent(true,dataSo.statIncrease.ToString(),
                stat.StatIcon,stat.StatName));
            await WaitOrSkip(1.0f);
            Bus<StatIncreaseDecreaseEvent>.Raise(new StatIncreaseDecreaseEvent(false,dataSo.statIncrease.ToString(),conditoinSprite,"컨디션"));
            
        }
        else
        {
            Bus<StatIncreaseDecreaseEvent>.Raise(new StatIncreaseDecreaseEvent(false,dataSo.statIncrease.ToString(),conditoinSprite,"컨디션"));
        }
        
        await WaitOrSkip(1.2f);
    }

    private async UniTask WaitOrSkip(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds && !isSkipped)
        {
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isSkipped = true;
    }
}