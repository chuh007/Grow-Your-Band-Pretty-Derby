using System;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;
using TMPro;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "Unit", menuName = "SO/Unit/Data")]
    public class UnitDataSO : ScriptableObject
    {
        public string unitName;
        public MemberType memberType;
        public string spriteAddressableKey;
        
        [Header("UI Settings")]
        public TMP_FontAsset handwritingFont;
        
        [Header("Addressable References")]
        public List<AssetReference> statsReferences;
        public List<AssetReference> personalPracticesReferences;
        public List<AssetReference> unitActionsReferences;
        
        [Header("Runtime Data")]
        public float maxCondition;
        public float currentCondition;
        public AssetReference TeamStat;
        
        [FormerlySerializedAs("teamSuccessComment")] [Header("Team Practice Comments")]
        public PracticeCommentDataSO successComment;
        [FormerlySerializedAs("teamFailComment")] public PracticeCommentDataSO failComment;
        
        [NonSerialized] public List<StatData> stats;
        [NonSerialized] public List<PersonalpracticeDataSO> personalPractices;
        [NonSerialized] public List<MemberActionData> unitActions;
        [NonSerialized] public StatData teamStat;
        [NonSerialized] private bool _isLoaded = false;

        /// <summary>
        /// AssetReference들을 로드해서 캐싱
        /// </summary>
        public async UniTask LoadAssets()
        {
            if (_isLoaded) return;
            
            if (stats == null)
                stats = new List<StatData>();
            else
                stats.Clear();
                
            if (personalPractices == null)
                personalPractices = new List<PersonalpracticeDataSO>();
            else
                personalPractices.Clear();
                
            if (unitActions == null)
                unitActions = new List<MemberActionData>();
            else
                unitActions.Clear();
            
            if (statsReferences != null)
            {
                foreach (var statRef in statsReferences)
                {
                    if (statRef != null && statRef.RuntimeKeyIsValid())
                    {
                        try
                        {
                            var stat = await statRef.LoadAssetAsync<StatData>().Task;
                            if (stat != null) stats.Add(stat);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[UnitDataSO] Failed to load stat for {unitName}: {e.Message}");
                        }
                    }
                }
            }
            
            if (personalPracticesReferences != null)
            {
                foreach (var practiceRef in personalPracticesReferences)
                {
                    if (practiceRef != null && practiceRef.RuntimeKeyIsValid())
                    {
                        try
                        {
                            var practice = await practiceRef.LoadAssetAsync<PersonalpracticeDataSO>().Task;
                            if (practice != null) personalPractices.Add(practice);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[UnitDataSO] Failed to load practice for {unitName}: {e.Message}");
                        }
                    }
                }
            }
            
            if (unitActionsReferences != null)
            {
                foreach (var actionRef in unitActionsReferences)
                {
                    if (actionRef != null && actionRef.RuntimeKeyIsValid())
                    {
                        try
                        {
                            var action = await actionRef.LoadAssetAsync<MemberActionData>().Task;
                            if (action != null) unitActions.Add(action);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[UnitDataSO] Failed to load action for {unitName}: {e.Message}");
                        }
                    }
                }
            }
            
            if (TeamStat != null && TeamStat.RuntimeKeyIsValid())
            {
                try
                {
                    teamStat = await TeamStat.LoadAssetAsync<StatData>().Task;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[UnitDataSO] Failed to load team stat for {unitName}: {e.Message}");
                }
            }

            _isLoaded = true;
            Debug.Log($"[UnitDataSO] {unitName} assets loaded successfully");
        }
        
        public void UnloadAssets()
        {
            if (!_isLoaded) return; 
            
            if (statsReferences != null)
            {
                foreach (var statRef in statsReferences)
                {
                    if (statRef != null && statRef.IsValid())
                    {
                        statRef.ReleaseAsset();
                    }
                }
            }

            if (personalPracticesReferences != null)
            {
                foreach (var practiceRef in personalPracticesReferences)
                {
                    if (practiceRef != null && practiceRef.IsValid())
                    {
                        practiceRef.ReleaseAsset();
                    }
                }
            }

            if (unitActionsReferences != null)
            {
                foreach (var actionRef in unitActionsReferences)
                {
                    if (actionRef != null && actionRef.IsValid())
                    {
                        actionRef.ReleaseAsset();
                    }
                }
            }
            
            if (TeamStat != null && TeamStat.IsValid())
            {
                TeamStat.ReleaseAsset();
            }

            stats?.Clear();
            personalPractices?.Clear();
            unitActions?.Clear();
            teamStat = null;
            _isLoaded = false;

            Debug.Log($"[UnitDataSO] {unitName} assets unloaded");
        }

        private void OnDestroy()
        {
            UnloadAssets();
        }
    }
}