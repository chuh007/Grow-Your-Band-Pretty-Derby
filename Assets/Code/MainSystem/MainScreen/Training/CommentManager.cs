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

        private readonly List<CommentData> _pendingComments = new();
        private Dictionary<string, List<CommentData>> _previousCommentsByMember = new();
        private bool _isTeamTraining = false;
        private Dictionary<string, List<CommentData>> _setupComments;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
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

        public Dictionary<string, List<CommentData>> GetSetupComments()
        {
            return _setupComments;
        }

        public List<CommentData> GetPreviousCommentsByMember(string memberName)
        {
            if (_previousCommentsByMember.ContainsKey(memberName))
            {
                return _previousCommentsByMember[memberName];
            }
            return null;
        }

        public void SaveCurrentCommentsAsPrevious()
        {
            if (_setupComments != null && _setupComments.Count > 0)
            {
                foreach (var kvp in _setupComments)
                {
                    _previousCommentsByMember[kvp.Key] = new List<CommentData>(kvp.Value);
                }
                
                Debug.Log($"[CommentManager] Saved comments for {_previousCommentsByMember.Count} members as previous");
            }
        }

        public void ClearAllComments()
        {
            _pendingComments.Clear();
            _setupComments = null;
            _isTeamTraining = false;
            
            Debug.Log("[CommentManager] All comments force cleared");
        }

        public void ClearPreviousComments()
        {
            _previousCommentsByMember.Clear();
            Debug.Log("[CommentManager] Previous comments cleared");
        }
    }
}