using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeCommentPage : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject commentItemPrefab;
        [SerializeField] private Transform commentParent;
        [SerializeField] private TextMeshProUGUI titleText;

        private readonly List<GameObject> spawnedComments = new();
        private UniTaskCompletionSource _clickTcs;
        private bool _isWaitingForClick = false; 

        public async UniTask ShowComments(List<CommentData> commentDataList, string name)
        {
            gameObject.SetActive(true);
            titleText.SetText(name + "의 훈련일지");
            ClearOldComments();

            foreach (var comment in commentDataList)
            {
                var go = Instantiate(commentItemPrefab, commentParent);
                
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
        
                go.GetComponent<PracticeCommentItemUI>().Setup(comment);
                spawnedComments.Add(go);
            }
            
            _clickTcs = new UniTaskCompletionSource();
            _isWaitingForClick = true; 
            
            Debug.Log($"[PracticeCommentPage] {name} 댓글 표시 완료, 클릭 대기 중...");
            
            await _clickTcs.Task;
            
            _isWaitingForClick = false; 
            Debug.Log($"[PracticeCommentPage] {name} 클릭 감지됨!");
        }

        private void ClearOldComments()
        {
            foreach (var go in spawnedComments)
                Destroy(go);

            spawnedComments.Clear();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"[PracticeCommentPage] 클릭 이벤트 발생! _isWaitingForClick: {_isWaitingForClick}, _clickTcs != null: {_clickTcs != null}");
            
            if (_isWaitingForClick && _clickTcs != null)
            {
                _clickTcs.TrySetResult();
                Debug.Log("[PracticeCommentPage] 클릭 처리 완료!");
            }
        }
    }
}