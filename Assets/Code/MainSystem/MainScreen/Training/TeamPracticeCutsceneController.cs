using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamPracticeCutsceneController : MonoBehaviour
    {
        public PlayableDirector director;
        public TimelineAsset timelineAsset;

        [System.Serializable]
        public class MemberObjectBinding
        {
            public MemberType memberType;
            public GameObject characterObject;
        }

        [Header("Member Bindings")]
        public List<MemberObjectBinding> memberObjects;

        [Header("Result UI")]
        public GameObject resultUI;

        private void Start()
        {
            ApplyTimelineBindings();
            director.stopped += OnTimelineEnd;
            director.Play();
            //resultUI.SetActive(false);
        }

        private void ApplyTimelineBindings()
        {
            var selected = TeamPracticeResultCache.SelectedMembers;
            
            foreach (var binding in memberObjects)
            {
                binding.characterObject.SetActive(false);
            }
            
            foreach (var unit in selected)
            {
                var binding = memberObjects.Find(b => b.memberType == unit.memberType);
                if (binding == null) continue;

                binding.characterObject.SetActive(true);

                var track = FindTrackByName(binding.memberType.ToString());
                if (track != null)
                {
                    director.SetGenericBinding(track, binding.characterObject);
                }
            }
        }
        
        private TrackAsset FindTrackByName(string name)
        {
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track.name == name)
                    return track;
            }
            return null;
        }


        private void OnTimelineEnd(PlayableDirector director)
        {
            foreach (var binding in memberObjects)
            {
                if (!binding.characterObject.activeInHierarchy) continue;

                var animator = binding.characterObject.GetComponent<Animator>();
                if (animator == null) continue;

                string animName = GetResultAnimationName(binding.memberType);
                animator.Play(animName, 0, 0f);
            }

            //resultUI.SetActive(true);
        }

        private string GetResultAnimationName(MemberType member)
        {
            var unit = TeamPracticeResultCache.SelectedMembers.Find(m => m.memberType == member);
            if (unit == null) return "Idle";

            return TeamPracticeResultCache.IsSuccess ? "Succse" : "Faill";
        }

        public void OnClickReturnToMain()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
