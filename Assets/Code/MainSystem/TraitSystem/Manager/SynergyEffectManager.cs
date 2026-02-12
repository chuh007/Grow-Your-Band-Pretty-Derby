using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Synergy;

namespace Code.MainSystem.TraitSystem.Manager
{
    public class SynergyEffectManager : MonoBehaviour
    {
        public static SynergyEffectManager Instance { get; private set; }

        private readonly List<IScoreSynergy> _scoreEffects = new();
        private readonly List<IFeverSynergy> _feverEffects = new();
        private readonly List<IJudgmentSynergy> _judgeEffects = new();
        private readonly List<IRecoverySynergy> _recoveryEffects = new();

        private HashSet<TraitTag> _activeTags = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        public void UpdateSynergies(IEnumerable<TraitGroupStatus> statuses)
        {
            _scoreEffects.Clear();
            _feverEffects.Clear();
            _judgeEffects.Clear();
            _recoveryEffects.Clear();
            _activeTags.Clear();

            foreach (var status in statuses)
            {
                if (status.CurrentCount > 0)
                    ApplySynergy(status.GroupData.GroupType, status.CurrentCount);
            }
        }

        private void ApplySynergy(TraitTag traitTag, int count)
        {
            _activeTags.Add(traitTag);
            switch (traitTag)
            {
                case TraitTag.Teamwork:
                    if (count >= 3) _scoreEffects.Add(new TeamworkEffect(count >= 5 ? 0.4f : 0.2f));
                    break;
                case TraitTag.Support:
                    if (count >= 3) _feverEffects.Add(new SupportEffect());
                    break;
                case TraitTag.Stability:
                    if (count >= 3) _judgeEffects.Add(new StabilityEffect());
                    break;
                case TraitTag.Energy:
                    if (count >= 3) _recoveryEffects.Add(new EnergyEffect(count >= 6 ? 25 : 50));
                    break;
                case TraitTag.Genius:
                    if (count >= 4)
                    {
                        var e = new GeniusEffect();
                        _scoreEffects.Add(e);
                        _judgeEffects.Add(e);
                    }
                    break;
                case TraitTag.Solo:
                    if (count >= 2)
                    {
                        var e = new SoloEffect();
                        _scoreEffects.Add(e);
                        _recoveryEffects.Add(e);
                    }
                    break;
                case TraitTag.Mastery:
                    if (count >= 3) _judgeEffects.Add(new MasteryEffect());
                    break;
                case TraitTag.Immersion:
                    if (count >= 3) _judgeEffects.Add(new ImmersionEffect());
                    break;
                case TraitTag.GuitarSolo:
                    if (count >= 5)
                    {
                        var e = new GuitarSoloEffect();
                        _scoreEffects.Add(e);
                        _judgeEffects.Add(e);
                    }
                    break;
            }
        }

        public float GetTotalExtraScore(float baseScore, float harmony, float concentration) 
            => _scoreEffects.Sum(e => e.GetExtraScore(baseScore, harmony, concentration));

        public string GetFinalJudgment(string judge, int combo) 
            => _judgeEffects.Aggregate(judge, (current, e) => e.OverrideJudgment(current, combo));

        public bool IsComboPenaltyDisabled(string judge)
            => _judgeEffects.Any(e => e.IsPenaltyDisabled(judge));

        public float GetFeverBonus(bool isPartChanged) =>
            _feverEffects.Sum(e => e.GetFeverDurationBonus(isPartChanged)) + (isPartChanged ? 2f : 0f);

        public void OnCombo(int combo, float totalStats)
            => _recoveryEffects.ForEach(e => e.OnComboTick(combo, totalStats));
        public void OnPerfect(float combo) 
            => _recoveryEffects.ForEach(e => e.OnPerfectHit(combo));

        public bool HasTag(TraitTag traitTag) 
            => _activeTags.Contains(traitTag);
    }
}