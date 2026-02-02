using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Code.MainSystem.MainScreen.Training
{
    public class CommentManager : MonoBehaviour
    {
        public static CommentManager instance;

        [SerializeField] private PracticeCommentPage commentPage;

        private readonly List<CommentData> _pendingComments = new();
        private bool _isTeamTraining = false;
        private Dictionary<string, List<CommentData>> _setupComments;
        private CancellationTokenSource _showCTS;

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

        public void SetupComments()
        {
            _setupComments = null;
            
            if (_pendingComments.Count == 0)
            {
                return;
            }

            _setupComments = new Dictionary<string, List<CommentData>>();
        
            foreach (var comment in _pendingComments)
            {
                string key = string.IsNullOrEmpty(comment.memberName) ? "Unknown" : comment.memberName;
            
                if (!_setupComments.ContainsKey(key))
                {
                    _setupComments[key] = new List<CommentData>();
                }
                _setupComments[key].Add(comment);
            }
            
            Debug.Log($"[CommentManager] Setup {_setupComments.Count} comment groups from {_pendingComments.Count} pending comments");
        }

        public async UniTask ShowAllComments()
        {
            if (_setupComments == null || _setupComments.Count == 0) 
            {
                Debug.Log("[CommentManager] No comments to show");
                if (!_isTeamTraining)
                {
                    Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
                }
                return;
            }

            _showCTS?.Cancel();
            _showCTS?.Dispose();
            _showCTS = new CancellationTokenSource();
        
            if (commentPage == null || !commentPage.gameObject.scene.isLoaded)
            {
                commentPage = FindObjectOfType<PracticeCommentPage>();
            }

            if (commentPage == null)
            {
                Debug.LogError("현재 씬에서 PracticeCommentPage를 찾을 수 없습니다!");
                
                if (!_isTeamTraining)
                {
                    Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
                }
                return;
            }

            try
            {
                foreach (var kvp in _setupComments)
                {
                    _showCTS.Token.ThrowIfCancellationRequested();
                    Debug.Log($"[CommentManager] Showing comments for {kvp.Key}");
                    await commentPage.ShowComments(kvp.Value, kvp.Key).AttachExternalCancellation(_showCTS.Token);
                }
                
                Debug.Log("[CommentManager] All comments shown successfully");
                if (!_isTeamTraining)
                {
                    Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[CommentManager] Comments skipped by user");
                if (!_isTeamTraining)
                {
                    Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
                }
            }
            finally
            {
                commentPage.gameObject.SetActive(false);
                
                _pendingComments.Clear();
                _setupComments = null;
                
                Debug.Log("[CommentManager] Comments cleaned up");
            }
        }

        public void ClearAllComments()
        {
            _showCTS?.Cancel();
            
            if (commentPage != null)
            {
                commentPage.ClearComments();
            }
            
            _pendingComments.Clear();
            _setupComments = null;
            _isTeamTraining = false;
            
            Debug.Log("[CommentManager] All comments force cleared");
        }

        private void OnDestroy()
        {
            _showCTS?.Cancel();
            _showCTS?.Dispose();
        }
    }
}