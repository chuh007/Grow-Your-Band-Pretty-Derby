using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 훈련 코멘트를 관리하는 싱글톤 매니저
    /// 
    /// 주요 책임:
    /// - 현재 턴의 코멘트 수집 및 그룹화
    /// - 이전 턴 코멘트 보관 및 조회
    /// - 멤버별 코멘트 필터링
    /// 
    /// 사용 흐름:
    /// 1. AddComment() - 코멘트 추가
    /// 2. SetupComments() - 멤버별 그룹화
    /// 3. GetCurrentComments() - 현재 코멘트 조회
    /// 4. SaveAsPrevious() - 다음 턴을 위해 보관
    /// 5. ClearCurrent() - 현재 코멘트 클리어
    /// </summary>
    public class CommentManager : MonoBehaviour
    {
        public static CommentManager Instance { get; private set; }

        #region Private Fields

        /// <summary>
        /// 아직 그룹화되지 않은 대기 중인 코멘트들
        /// </summary>
        private readonly List<CommentData> _pendingComments = new();

        /// <summary>
        /// 현재 턴의 멤버별 그룹화된 코멘트 (SetupComments 후 채워짐)
        /// </summary>
        private Dictionary<string, List<CommentData>> _currentCommentsByMember;

        /// <summary>
        /// 이전 턴의 멤버별 코멘트 (참고용 보관)
        /// </summary>
        private Dictionary<string, List<CommentData>> _previousCommentsByMember = new();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeSingleton();
        }

        private void OnDestroy()
        {
            ClearAll();
        }

        #endregion

        #region Initialization

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[CommentManager] Initialized");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Public API - Adding Comments

        /// <summary>
        /// 코멘트 추가 (pending 상태로)
        /// SetupComments()를 호출하기 전까지는 그룹화되지 않음
        /// </summary>
        /// <param name="data">추가할 코멘트 데이터</param>
        public void AddComment(CommentData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[CommentManager] Attempted to add null comment");
                return;
            }

            _pendingComments.Add(data);
            Debug.Log($"[CommentManager] Comment added for {data.memberName} (Pending: {_pendingComments.Count})");
        }

        /// <summary>
        /// 여러 코멘트를 한 번에 추가
        /// </summary>
        public void AddComments(IEnumerable<CommentData> comments)
        {
            if (comments == null) return;

            foreach (var comment in comments)
            {
                AddComment(comment);
            }
        }

        #endregion

        #region Public API - Setup & Grouping

        /// <summary>
        /// pending 상태의 코멘트들을 멤버별로 그룹화
        /// 이 메서드 호출 후 GetCurrentComments()로 조회 가능
        /// </summary>
        public void SetupComments()
        {
            _currentCommentsByMember = null;

            if (_pendingComments.Count == 0)
            {
                Debug.Log("[CommentManager] No pending comments to setup");
                return;
            }

            _currentCommentsByMember = GroupCommentsByMember(_pendingComments);

            Debug.Log($"[CommentManager] Setup {_currentCommentsByMember.Count} comment groups from {_pendingComments.Count} pending comments");
        }

        /// <summary>
        /// 코멘트들을 멤버 이름별로 그룹화
        /// </summary>
        private Dictionary<string, List<CommentData>> GroupCommentsByMember(List<CommentData> comments)
        {
            var grouped = new Dictionary<string, List<CommentData>>();

            foreach (var comment in comments)
            {
                string memberName = GetMemberKey(comment);

                if (!grouped.ContainsKey(memberName))
                {
                    grouped[memberName] = new List<CommentData>();
                }

                grouped[memberName].Add(comment);
            }

            return grouped;
        }

        /// <summary>
        /// 코멘트의 멤버 키 생성 (null-safe)
        /// </summary>
        private string GetMemberKey(CommentData comment)
        {
            return string.IsNullOrEmpty(comment.memberName) ? "Unknown" : comment.memberName;
        }

        #endregion

        #region Public API - Querying

        /// <summary>
        /// 현재 턴의 그룹화된 코멘트 조회
        /// SetupComments() 호출 후에만 유효
        /// </summary>
        public Dictionary<string, List<CommentData>> GetCurrentComments()
        {
            if (_currentCommentsByMember == null)
            {
                Debug.LogWarning("[CommentManager] Current comments not setup yet. Call SetupComments() first.");
                return new Dictionary<string, List<CommentData>>();
            }

            return _currentCommentsByMember;
        }

        /// <summary>
        /// 특정 멤버의 현재 코멘트 조회
        /// </summary>
        public List<CommentData> GetCurrentCommentsByMember(string memberName)
        {
            if (_currentCommentsByMember == null)
            {
                Debug.LogWarning("[CommentManager] Current comments not setup yet");
                return new List<CommentData>();
            }

            return _currentCommentsByMember.TryGetValue(memberName, out var comments)
                ? comments
                : new List<CommentData>();
        }

        /// <summary>
        /// 이전 턴의 특정 멤버 코멘트 조회
        /// </summary>
        public List<CommentData> GetPreviousCommentsByMember(string memberName)
        {
            return _previousCommentsByMember.TryGetValue(memberName, out var comments)
                ? comments
                : new List<CommentData>();
        }

        /// <summary>
        /// pending 중인 코멘트 개수
        /// </summary>
        public int GetPendingCount() => _pendingComments.Count;

        /// <summary>
        /// 현재 코멘트가 setup 되었는지 확인
        /// </summary>
        public bool IsSetup() => _currentCommentsByMember != null;

        #endregion

        #region Public API - Persistence

        /// <summary>
        /// 현재 코멘트들을 이전 코멘트로 저장
        /// 다음 턴 시작 전에 호출하여 이전 내역 보관
        /// </summary>
        public void SaveCurrentAsPrevious()
        {
            if (_currentCommentsByMember == null || _currentCommentsByMember.Count == 0)
            {
                Debug.LogWarning("[CommentManager] No current comments to save");
                return;
            }

            // Deep copy
            _previousCommentsByMember.Clear();
            foreach (var kvp in _currentCommentsByMember)
            {
                _previousCommentsByMember[kvp.Key] = new List<CommentData>(kvp.Value);
            }

            Debug.Log($"[CommentManager] Saved {_previousCommentsByMember.Count} member groups as previous");
        }

        #endregion

        #region Public API - Clearing

        /// <summary>
        /// 현재 코멘트만 클리어 (pending + current)
        /// 이전 코멘트는 유지
        /// </summary>
        public void ClearCurrent()
        {
            _pendingComments.Clear();
            _currentCommentsByMember = null;

            Debug.Log("[CommentManager] Current comments cleared");
        }

        /// <summary>
        /// 이전 코멘트만 클리어
        /// </summary>
        public void ClearPrevious()
        {
            _previousCommentsByMember.Clear();
            Debug.Log("[CommentManager] Previous comments cleared");
        }

        /// <summary>
        /// 모든 코멘트 클리어 (현재 + 이전)
        /// </summary>
        public void ClearAll()
        {
            ClearCurrent();
            ClearPrevious();

            Debug.Log("[CommentManager] All comments cleared");
        }

        #endregion

        #region Debug

        /// <summary>
        /// 디버그용: 현재 상태 출력
        /// </summary>
        [ContextMenu("Print Comment Status")]
        public void PrintStatus()
        {
            Debug.Log("=== Comment Manager Status ===");
            Debug.Log($"Pending Comments: {_pendingComments.Count}");
            Debug.Log($"Current Groups: {(_currentCommentsByMember?.Count ?? 0)}");
            Debug.Log($"Previous Groups: {_previousCommentsByMember.Count}");

            if (_currentCommentsByMember != null)
            {
                foreach (var kvp in _currentCommentsByMember)
                {
                    Debug.Log($"  {kvp.Key}: {kvp.Value.Count} comments");
                }
            }
        }

        #endregion
    }
}