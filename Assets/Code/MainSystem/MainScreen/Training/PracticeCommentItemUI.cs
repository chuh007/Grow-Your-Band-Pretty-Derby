using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeCommentItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private TextMeshProUGUI thoughtsText;
        [SerializeField] private List<StatUpDownComment> statChangeItems;

        public void Setup(CommentData data)
        {
            titleText.text = data.title;
            contentText.text = data.content;
            thoughtsText.text = string.IsNullOrEmpty(data.thoughts) ? "" : data.thoughts;

            for (int i = 0; i < statChangeItems.Count; i++)
            {
                if (i < data.statChanges.Count)
                {
                    statChangeItems[i].gameObject.SetActive(true);
                    statChangeItems[i].Setup(data.statChanges[i]);
                }
                else
                {
                    statChangeItems[i].gameObject.SetActive(false);
                }
            }
        }
    }
}