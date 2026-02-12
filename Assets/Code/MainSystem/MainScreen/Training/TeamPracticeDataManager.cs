using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 팀 연습 결과 데이터를 씬 간 전달하는 싱글톤 매니저
    /// DontDestroyOnLoad를 사용하여 씬 전환에도 데이터 유지
    /// </summary>
    public class TeamPracticeDataManager : MonoBehaviour
    {
        public static TeamPracticeDataManager Instance;

        private TeamPracticeResultData _currentResultData;

        /// <summary>
        /// 결과 데이터 저장
        /// </summary>
        public void SetResultData(TeamPracticeResultData data)
        {
            _currentResultData = data;
            Debug.Log($"[TeamPracticeDataManager] Result data saved - Success: {data.isSuccess}, Members: {data.selectedMembers.Count}");
        }

        /// <summary>
        /// 결과 데이터 가져오기
        /// </summary>
        public TeamPracticeResultData GetResultData()
        {
            if (_currentResultData == null)
            {
                Debug.LogError("[TeamPracticeDataManager] No result data available!");
            }
            return _currentResultData;
        }

        /// <summary>
        /// 데이터 존재 여부 확인
        /// </summary>
        public bool HasResultData()
        {
            return _currentResultData != null && _currentResultData.IsValid();
        }

        /// <summary>
        /// 데이터 초기화 (사용 후 정리)
        /// </summary>
        public void ClearResultData()
        {
            if (_currentResultData != null)
            {
                _currentResultData.Clear();
                _currentResultData = null;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}