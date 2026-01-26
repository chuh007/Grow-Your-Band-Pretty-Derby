using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.MainScreen.Training
{
    public class CommentManager : MonoBehaviour
    {
        public static CommentManager instance;

        [SerializeField] private PracticeCommentPage commentPage;

        private readonly List<CommentData> _pendingComments = new();
        private bool _isTeamTraining = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddComment(CommentData data, bool isTeamTraining = false) 
        {
            _pendingComments.Add(data);
            _isTeamTraining = isTeamTraining;
        }

        public async UniTask ShowAllComments()
        {
            if (_pendingComments.Count == 0) return;
        
            if (commentPage == null || !commentPage.gameObject.scene.isLoaded)
            {
                commentPage = FindObjectOfType<PracticeCommentPage>();
            }

            if (commentPage == null)
            {
                Debug.LogError("현재 씬에서 PracticeCommentPage를 찾을 수 없습니다!");
                return;
            }
        
            var groupedByMember = new Dictionary<string, List<CommentData>>();
        
            foreach (var comment in _pendingComments)
            {
                string key = string.IsNullOrEmpty(comment.memberName) ? "Unknown" : comment.memberName;
            
                if (!groupedByMember.ContainsKey(key))
                {
                    groupedByMember[key] = new List<CommentData>();
                }
                groupedByMember[key].Add(comment);
            }
        
            foreach (var kvp in groupedByMember)
            {
                await commentPage.ShowComments(kvp.Value, kvp.Key);
            }

            commentPage.gameObject.SetActive(false);
            
            if (!_isTeamTraining)
            {
                Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
            }
        
            _pendingComments.Clear();
            _isTeamTraining = false;
        }
    }
}