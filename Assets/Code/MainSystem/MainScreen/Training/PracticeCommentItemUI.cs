using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeCommentItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private Transform statChangeRoot;
        [SerializeField] private GameObject statChangeItemPrefab;

        public void Setup(CommentData data)
        {
            titleText.text = data.title;
            contentText.text = data.content;

            foreach (Transform child in statChangeRoot)
                Destroy(child.gameObject);

            foreach (var stat in data.statChanges)
            {
                var go = Instantiate(statChangeItemPrefab, statChangeRoot);

                var icon = go.transform.Find("Icon")?.GetComponent<Image>();
                var value = go.transform.Find("Value")?.GetComponent<TextMeshProUGUI>();

                if (icon != null && stat.icon != null)
                    icon.sprite = stat.icon;

                if (value != null)
                    value.text = $"{stat.statName}: {(stat.delta >= 0 ? "+" : "")}{stat.delta}";
            }
        }
    }
}