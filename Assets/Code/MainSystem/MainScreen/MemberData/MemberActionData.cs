using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEditor.Animations;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "Member", menuName = "SO/Member/Action", order = 0)]
    public  class MemberActionData : ScriptableObject
    {
        public MemberType memberType;
        public AnimatorController animator;
        public Sprite sprite;
        public StatType statType;
        public string startAnimationName;
        [TextArea]
        public string description;
    }
}