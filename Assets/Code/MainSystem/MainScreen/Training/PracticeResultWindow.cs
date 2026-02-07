using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine.EventSystems;
using System.Threading;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeResultWindow : MonoBehaviour, IPointerDownHandler
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject statItemPrefab;
        [SerializeField] private GameObject commentItemPrefab;

        [Header("Parents")]
        [SerializeField] private Transform statParent;
        [SerializeField] private Transform commentParent;

        [Header("Icons")]
        [SerializeField] private Sprite normalIcon;
        [SerializeField] private Sprite deltaIcon;

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Skip Settings")]
        [SerializeField] private bool allowSkip = true;
        [SerializeField] private float skipCooldown = 0.3f;
        [SerializeField] private float autoSkipTimeout = 15f;

        private bool _isPlaying = false;
        private CancellationTokenSource _skipCTS;
        private bool _hasSkipped = false;
        private float _lastSkipTime = 0f;
        private bool _isTeamTraining = false;
        private List<UnitDataSO> _currentUnits;

        private readonly List<GameObject> _spawnedStats = new();
        private readonly List<GameObject> _spawnedComments = new();

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_isPlaying && allowSkip && !_hasSkipped)
            {
                if (Input.GetKeyDown(KeyCode.Space) || 
                    Input.GetKeyDown(KeyCode.Return) || 
                    Input.GetKeyDown(KeyCode.KeypadEnter) ||
                    Input.GetMouseButtonDown(0))
                {
                    Debug.Log("[PracticeResultWindow] Skip triggered by input");
                    TriggerSkip();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("[PracticeResultWindow] OnPointerDown called");
            
            if (Time.time - _lastSkipTime < skipCooldown)
            {
                Debug.Log($"[PracticeResultWindow] Skip cooldown active ({skipCooldown}s)");
                return;
            }

            if (_isPlaying && allowSkip && !_hasSkipped)
            {
                Debug.Log("[PracticeResultWindow] Skip via OnPointerDown");
                TriggerSkip();
            }
        }

        private void TriggerSkip()
        {
            if (_hasSkipped) return;
            
            _hasSkipped = true;
            _lastSkipTime = Time.time;
            
            Debug.Log("[PracticeResultWindow] Skip triggered!");
            
            foreach (var commentGO in _spawnedComments)
            {
                var commentUI = commentGO.GetComponent<PracticeCommentItemUI>();
                if (commentUI != null && commentUI._currentData != null)
                {
                    commentUI.SkipToComplete(commentUI._currentData);
                }
            }
            
            _skipCTS?.Cancel();
        }

        public async UniTask Play(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatCurrentValue,
            float teamStatDelta,
            Dictionary<(MemberType memberType, StatType statType), int> statDeltaDict,
            bool hadAnyStatChanged,
            PersonalpracticeDataSO practiceData = null,
            bool isTeamTraining = false
        )
        {
            Debug.Log("[PracticeResultWindow] Play started");
            
            _isPlaying = true;
            _hasSkipped = false;
            _isTeamTraining = isTeamTraining;
            _currentUnits = allUnits;
            _skipCTS?.Cancel();
            _skipCTS?.Dispose();
            _skipCTS = new CancellationTokenSource();

            try
            {
                gameObject.SetActive(true);
                ClearAll();
                
                if (titleText != null && allUnits != null && allUnits.Count > 0)
                {
                    titleText.text = $"{allUnits[0].unitName} 훈련 결과";
                }
                
                await CreateChangedStats(
                    statManager,
                    allUnits,
                    conditionCurrent,
                    conditionDelta,
                    teamStat,
                    teamStatCurrentValue,
                    teamStatDelta,
                    statDeltaDict
                );
                
                await CreateComments();
                
                await WaitForClick();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[PracticeResultWindow] Skipped by user");
            }
            finally
            {
                EndWindow();
            }
            
            Debug.Log("[PracticeResultWindow] Play completed");
        }

        private async UniTask CreateChangedStats(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatCurrentValue,
            float teamStatDelta,
            Dictionary<(MemberType, StatType), int> statDeltaDict
        )
        {
            if (!Mathf.Approximately(conditionDelta, 0f))
            {
                await CreateStatItemWithGauge("컨디션", Mathf.RoundToInt(conditionCurrent), conditionDelta, deltaIcon);
            }
            
            if (teamStat != null && !Mathf.Approximately(teamStatDelta, 0f))
            {
                await CreateStatItemWithGauge(teamStat.statName, Mathf.RoundToInt(teamStatCurrentValue), teamStatDelta, deltaIcon);
            }
            
            foreach (var unit in allUnits)
            {
                foreach (var stat in unit.stats)
                {
                    BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                    if (baseStat == null) continue;

                    statDeltaDict.TryGetValue((unit.memberType, stat.statType), out int delta);

                    if (delta != 0)
                    {
                        await CreateStatItemWithGauge(stat.statName, baseStat.CurrentValue, delta, normalIcon);
                    }
                }
            }
        }

        private async UniTask CreateStatItemWithGauge(string statName, int currentValue, float delta, Sprite icon)
        {
            var go = Instantiate(statItemPrefab, statParent);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);

            var statUI = go.GetComponent<StatResultItemUI>();
            if (statUI != null)
            {
                statUI.SetInitialData(statName, icon);
                
                try
                {
                    await statUI.AnimateToValue(icon, currentValue, 0.5f, _skipCTS.Token);
                }
                catch (System.OperationCanceledException)
                {
                    statUI.ForceSetValue(icon, currentValue);
                }
            }
            
            _spawnedStats.Add(go);

            Debug.Log($"[PracticeResultWindow] Created stat: {statName} = {currentValue} ({delta:+0;-0})");
        }

        private async UniTask CreateComments()
        {
            var setupComments = CommentManager.instance.GetSetupComments();
            
            if (setupComments == null || setupComments.Count == 0)
            {
                Debug.Log("[PracticeResultWindow] No comments to display");
                return;
            }

            TMP_FontAsset handwritingFont = null;
            if (_currentUnits != null && _currentUnits.Count > 0 && _currentUnits[0].handwritingFont != null)
            {
                handwritingFont = _currentUnits[0].handwritingFont;
            }
            
            Debug.Log(handwritingFont);

            foreach (var kvp in setupComments)
            {
                _skipCTS.Token.ThrowIfCancellationRequested();

                foreach (var commentData in kvp.Value)
                {
                    var go = Instantiate(commentItemPrefab, commentParent);
                    go.transform.localScale = Vector3.one;
                    go.SetActive(true);
                    
                    var commentUI = go.GetComponent<PracticeCommentItemUI>();
                    
                    if (handwritingFont != null)
                    {
                        commentUI.SetHandwritingFont(handwritingFont);
                    }
                    
                    commentUI.Setup(commentData);
                    
                    _spawnedComments.Add(go);
                }
            }

            await UniTask.Yield();
        }

        private async UniTask WaitForClick()
        {
            Debug.Log("[PracticeResultWindow] Waiting for click... (Press SPACE/ENTER or click anywhere)");
            
            float waitStartTime = Time.time;
            
            var clickTask = UniTask.WaitUntil(() => _hasSkipped, cancellationToken: _skipCTS.Token);
            var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(autoSkipTimeout), cancellationToken: _skipCTS.Token);
            
            var completedIndex = await UniTask.WhenAny(clickTask, timeoutTask);
            
            if (completedIndex == 1)
            {
                Debug.Log($"[PracticeResultWindow] Auto-skip after {autoSkipTimeout} seconds timeout");
                _hasSkipped = true;
            }
            else
            {
                float elapsedTime = Time.time - waitStartTime;
                Debug.Log($"[PracticeResultWindow] Click detected after {elapsedTime:F2} seconds!");
            }
        }

        private void ClearAll()
        {
            foreach (var go in _spawnedStats)
                Destroy(go);
            _spawnedStats.Clear();

            foreach (var go in _spawnedComments)
                Destroy(go);
            _spawnedComments.Clear();
        }

        private void EndWindow()
        {
            Debug.Log("[PracticeResultWindow] EndWindow called");
            
            ClearAll();
            gameObject.SetActive(false);
            
            _isPlaying = false;
            _hasSkipped = false;
            _currentUnits = null;
            CommentManager.instance.ClearAllComments();
            
            if (!_isTeamTraining)
            {
                Debug.Log("[PracticeResultWindow] Raising CheckTurnEnd event");
                Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
            }
            
            _isTeamTraining = false;
            
            Debug.Log("[PracticeResultWindow] EndWindow completed");
        }

        private void OnDestroy()
        {
            _skipCTS?.Cancel();
            _skipCTS?.Dispose();
        }
    }
}