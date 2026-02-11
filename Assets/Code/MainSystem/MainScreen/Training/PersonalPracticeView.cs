using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

public class PersonalPracticeView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject succesImage;
    [SerializeField] private GameObject failImage;
    [SerializeField] private Sprite conditoinSprite;

    private bool isSkipped;

    public async UniTask Play(
        MemberActionData actionData,
        bool isSuccess,
        PersonalpracticeDataSO dataSo,
        float currentCondition,
        StatManager statManager,
        string name,
        Action<int> onComplete
    )
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
            BaseStat stat = statManager.GetMemberStat(actionData.memberType, dataSo.PracticeStatType);
            var commentData = dataSo.PersonalsuccessComment;
            
            var statChanges = new List<StatChangeInfo>
            {
                new StatChangeInfo(string.Empty, (int)dataSo.statIncrease, stat.StatIcon),
                new StatChangeInfo(string.Empty, -(int)dataSo.StaminaReduction, conditoinSprite)
            };

            var comment = new CommentData(
                $"{name}의 훈련일지",
                commentData.comment,
                statChanges,
                PracticenType.Personal,
                commentData.thoughts,
                name 
            );

            CommentManager.Instance.AddComment(comment); 
            
            Bus<StatIncreaseDecreaseEvent>.Raise(
                new StatIncreaseDecreaseEvent(true, dataSo.statIncrease.ToString(), stat.StatIcon, stat.StatName)
            );
            await WaitOrSkip(1.0f);

            Bus<StatIncreaseDecreaseEvent>.Raise(
                new StatIncreaseDecreaseEvent(false, dataSo.StaminaReduction.ToString(), conditoinSprite, "컨디션")
            );
        }
        else
        {
            var commentData = dataSo.PersonalfaillComment;

            var statChanges = new List<StatChangeInfo>
            {
                new StatChangeInfo(string.Empty, -(int)dataSo.StaminaReduction, conditoinSprite)
            };

            var comment = new CommentData(
                $"{name}의 훈련일지",
                commentData.comment,
                statChanges,
                PracticenType.Personal,
                commentData.thoughts,
                name 
            );

            CommentManager.Instance.AddComment(comment); 
            Bus<StatIncreaseDecreaseEvent>.Raise(
                new StatIncreaseDecreaseEvent(false, dataSo.StaminaReduction.ToString(), conditoinSprite, "컨디션")
            );
        }

        await WaitOrSkip(1.2f);
        onComplete?.Invoke((int)actionData.memberType);
    }

    private async UniTask WaitOrSkip(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds && !isSkipped)
        {
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }

        Bus<StopEvent>.Raise(new StopEvent());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isSkipped = true;
    }
}
