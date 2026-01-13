using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeCommentPage : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject commentItemPrefab;
        [SerializeField] private Transform commentParent;

        private readonly List<GameObject> spawnedComments = new();

        public async UniTask ShowComments(List<CommentData> commentDataList)
        {
            ClearOldComments();

            foreach (var comment in commentDataList)
            {
                var go = Instantiate(commentItemPrefab, commentParent);
                go.GetComponent<PracticeCommentItemUI>().Setup(comment);
                spawnedComments.Add(go);
            }

            await UniTask.Yield();
        }

        private void ClearOldComments()
        {
            foreach (var go in spawnedComments)
                Destroy(go);

            spawnedComments.Clear();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}