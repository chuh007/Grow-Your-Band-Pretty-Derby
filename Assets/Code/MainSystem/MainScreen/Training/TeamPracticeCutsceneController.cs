using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.EncounterEvents;
using Code.MainSystem.Encounter;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using Code.SubSystem.Lobby.Album;
using Code.SubSystem.Lobby.Album.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 팀 연습 컷씬(타임라인) 제어만 담당
    /// 
    /// 책임:
    /// - 타임라인 재생/스킵
    /// - 멤버별 캐릭터 바인딩
    /// - 멤버별 오디오 재생 및 동기화
    /// - 타임라인 종료 후 결과 표시 시퀀스 시작
    /// 
    /// 책임 아님:
    /// - 코멘트 생성 (TeamPracticeCommentBuilder)
    /// - 결과 데이터 가져오기 (TeamPracticeResultData)
    /// - 결과 UI 표시 (PracticeResultWindow)
    /// </summary>
    public class TeamPracticeCutsceneController : MonoBehaviour, IPointerDownHandler
    {
        #region Serialized Fields

        [Header("Timeline Components")]
        [SerializeField] private PlayableDirector director;
        [SerializeField] private TimelineAsset timelineAsset;

        [Header("Character Bindings")]
        [SerializeField] private List<MemberObjectBinding> memberObjects;

        [Header("Member Audios")]
        [SerializeField] private List<MemberAudioBinding> memberAudios = new List<MemberAudioBinding>();

        [Header("Result Components")]
        [SerializeField] private PracticeResultWindow resultWindow;
        [SerializeField] private TeamPracticeCommentBuilder commentBuilder;

        [Header("Settings")]
        [SerializeField] private bool allowSkip = true;
        [SerializeField] private string mainSceneName = "Lch";
        [SerializeField] private float postTimelineDelay = 1.0f;
        [SerializeField] private float preSceneTransitionDelay = 0.5f;

        #endregion

        #region Nested Types

        [System.Serializable]
        public class MemberObjectBinding
        {
            public MemberType memberType;
            public GameObject characterObject;
        }

        [System.Serializable]
        public class MemberAudioBinding
        {
            public MemberType memberType;
            public AudioSource audioSource;
        }

        #endregion

        #region Private Fields

        private bool _isTimelinePlaying = true;
        private bool _hasSkipped = false;
        private TeamPracticeResultData _resultData;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // DataManager에서 결과 데이터 로드
            _resultData = TeamPracticeDataManager.Instance.GetResultData();
            
            ValidateDependencies();
            InitializeTimeline();
        }

        private void OnDestroy()
        {
            if (director != null)
            {
                director.stopped -= OnTimelineEnd;
            }
            
            // 오디오 정리
            StopAllMemberAudios();
            
            // 씬을 떠날 때 데이터 정리
            if (TeamPracticeDataManager.Instance != null)
            {
                TeamPracticeDataManager.Instance.ClearResultData();
            }
        }

        #endregion

        #region Initialization

        private void ValidateDependencies()
        {
            if (director == null)
                Debug.LogError("[TeamPracticeCutscene] PlayableDirector is missing!");

            if (resultWindow == null)
                Debug.LogError("[TeamPracticeCutscene] PracticeResultWindow is missing!");

            if (_resultData == null || !_resultData.IsValid())
                Debug.LogError("[TeamPracticeCutscene] Result data is missing or invalid!");

            if (commentBuilder == null)
                Debug.LogError("[TeamPracticeCutscene] CommentBuilder is missing!");
                
            if (AlbumManager.Instance == null)
                Debug.LogError("[TeamPracticeCutscene] AlbumManager instance is missing!");
        }

        private void InitializeTimeline()
        {
            ApplyCharacterBindings();
            SetupMemberAudios();
            
            director.stopped += OnTimelineEnd;
            director.Play();
            
            // 타임라인 시작과 동시에 오디오 재생
            PlayAllMemberAudios();

            Debug.Log("[TeamPracticeCutscene] Timeline started");
        }

        #endregion

        #region Timeline Control

        /// <summary>
        /// 선택된 멤버들만 타임라인에 바인딩
        /// </summary>
        private void ApplyCharacterBindings()
        {
            if (_resultData == null || _resultData.selectedMembers == null)
            {
                Debug.LogError("[TeamPracticeCutscene] Cannot apply bindings - no result data");
                return;
            }

            // 모든 캐릭터 오브젝트 비활성화
            DeactivateAllCharacters();

            // 선택된 멤버만 활성화 및 바인딩
            foreach (var unit in _resultData.selectedMembers)
            {
                BindMemberToTimeline(unit.memberType);
            }

            Debug.Log($"[TeamPracticeCutscene] Bound {_resultData.selectedMembers.Count} members to timeline");
        }

        private void DeactivateAllCharacters()
        {
            foreach (var binding in memberObjects)
            {
                if (binding.characterObject != null)
                {
                    binding.characterObject.SetActive(false);
                }
            }
        }

        private void BindMemberToTimeline(MemberType memberType)
        {
            var binding = memberObjects.Find(b => b.memberType == memberType);
            if (binding == null)
            {
                Debug.LogWarning($"[TeamPracticeCutscene] No binding found for {memberType}");
                return;
            }

            binding.characterObject.SetActive(true);

            var track = FindTrackByName(memberType.ToString());
            if (track != null)
            {
                director.SetGenericBinding(track, binding.characterObject);
            }
            else
            {
                Debug.LogWarning($"[TeamPracticeCutscene] No track found for {memberType}");
            }
        }

        private TrackAsset FindTrackByName(string trackName)
        {
            if (timelineAsset == null) return null;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track.name == trackName)
                    return track;
            }

            return null;
        }

        #endregion

        #region Audio Control

        /// <summary>
        /// 선택된 멤버들의 오디오 설정
        /// </summary>
        private void SetupMemberAudios()
        {
            if (_resultData == null || _resultData.selectedMembers == null)
            {
                Debug.LogError("[TeamPracticeCutscene] Cannot setup audio - no result data");
                return;
            }

            if (AlbumManager.Instance == null)
            {
                Debug.LogError("[TeamPracticeCutscene] AlbumManager is not available");
                return;
            }

            foreach (var unit in _resultData.selectedMembers)
            {
                var audioBinding = memberAudios.Find(a => a.memberType == unit.memberType);
                if (audioBinding == null || audioBinding.audioSource == null)
                {
                    Debug.LogWarning($"[TeamPracticeCutscene] No audio source found for {unit.memberType}");
                    continue;
                }

                // AlbumManager에서 해당 멤버의 파트 오디오 가져오기
                AudioClip partClip = AlbumManager.Instance.GetMemberPartAudio(unit.memberType);
                if (partClip != null)
                {
                    audioBinding.audioSource.clip = partClip;
                    Debug.Log($"[TeamPracticeCutscene] Loaded audio clip for {unit.memberType}");
                }
                else
                {
                    Debug.LogWarning($"[TeamPracticeCutscene] No audio clip found for {unit.memberType}");
                }
            }
        }

        /// <summary>
        /// 모든 멤버 오디오 재생
        /// </summary>
        private void PlayAllMemberAudios()
        {
            if (_resultData == null || _resultData.selectedMembers == null)
                return;

            foreach (var unit in _resultData.selectedMembers)
            {
                var audioBinding = memberAudios.Find(a => a.memberType == unit.memberType);
                if (audioBinding != null && audioBinding.audioSource != null && audioBinding.audioSource.clip != null)
                {
                    audioBinding.audioSource.Play();
                    Debug.Log($"[TeamPracticeCutscene] Playing audio for {unit.memberType}");
                }
            }
        }

        /// <summary>
        /// 모든 멤버 오디오 정지
        /// </summary>
        private void StopAllMemberAudios()
        {
            foreach (var audioBinding in memberAudios)
            {
                if (audioBinding.audioSource != null && audioBinding.audioSource.isPlaying)
                {
                    audioBinding.audioSource.Stop();
                }
            }
            
            Debug.Log("[TeamPracticeCutscene] All member audios stopped");
        }

        #endregion

        #region Skip Handling

        public void OnPointerDown(PointerEventData eventData)
        {
            if (allowSkip && _isTimelinePlaying && !_hasSkipped)
            {
                SkipTimeline();
            }
        }

        private void SkipTimeline()
        {
            if (!_isTimelinePlaying || _hasSkipped) return;

            _hasSkipped = true;
            
            // 타임라인 스킵
            director.time = director.duration;
            director.Evaluate();
            
            // 오디오도 함께 정지
            StopAllMemberAudios();

            Debug.Log("[TeamPracticeCutscene] Timeline and audios skipped");
        }

        #endregion

        #region Timeline Callbacks

        private async void OnTimelineEnd(PlayableDirector finishedDirector)
        {
            _isTimelinePlaying = false;
            
            // 타임라인이 끝나면 오디오도 정지
            StopAllMemberAudios();
            
            Debug.Log("[TeamPracticeCutscene] Timeline ended");

            // 결과 애니메이션 재생
            PlayResultAnimations();

            await UniTask.Delay((int)(postTimelineDelay * 1000));

            // 결과 표시 시퀀스
            await ShowResults();

            if (EncounterManager.Instance.TryTeamPracticeEncounter(_resultData.selectedMembers))
                return;
            
            // 메인 씬으로 복귀
            await ReturnToMainScene();
        }

        /// <summary>
        /// 각 캐릭터의 성공/실패 애니메이션 재생
        /// </summary>
        private void PlayResultAnimations()
        {
            foreach (var binding in memberObjects)
            {
                if (!binding.characterObject.activeInHierarchy) continue;

                var animator = binding.characterObject.GetComponent<Animator>();
                if (animator == null) continue;

                string animName = _resultData.isSuccess ? "Succse" : "Faill";
                animator.Play(animName, 0, 0f);
            }

            Debug.Log($"[TeamPracticeCutscene] Playing result animations (Success: {_resultData.isSuccess})");
        }

        #endregion

        #region Result Display

        /// <summary>
        /// 각 멤버별 결과 표시 (순차 진행)
        /// </summary>
        private async UniTask ShowResults()
        {
            if (_resultData.selectedMembers == null || _resultData.selectedMembers.Count == 0)
            {
                Debug.LogError("[TeamPracticeCutscene] No members to show results for");
                return;
            }

            foreach (var unit in _resultData.selectedMembers)
            {
                await ShowResultForMember(unit);
            }
            Debug.Log("[TeamPracticeCutscene] All results displayed");
        }

        /// <summary>
        /// 특정 멤버의 결과 표시
        /// </summary>
        private async UniTask ShowResultForMember(UnitDataSO unit)
        {
            // 코멘트 생성 (코멘트 빌더에 위임)
            CommentManager.Instance.ClearCurrent();
            commentBuilder.BuildCommentForMember(unit, _resultData);
            CommentManager.Instance.SetupComments();

            // 결과 창 표시 (UI 컴포넌트에 위임)
            await resultWindow.ShowTeamPracticeResult(unit, _resultData);
        }

        #endregion

        #region Scene Transition

        private async UniTask ReturnToMainScene()
        {
            await UniTask.Delay((int)(preSceneTransitionDelay * 1000));

            Debug.Log($"[TeamPracticeCutscene] Loading main scene: {mainSceneName}");
            SceneManager.LoadScene(mainSceneName);
        }

        /// <summary>
        /// 수동 씬 복귀용 (버튼 클릭 등)
        /// </summary>
        public void OnClickReturnToMain()
        {
            SceneManager.LoadScene(mainSceneName);
        }

        #endregion
    }
}