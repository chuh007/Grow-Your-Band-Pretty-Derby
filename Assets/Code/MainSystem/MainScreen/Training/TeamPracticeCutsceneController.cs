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
            public AnimationClip successClip;
            public AnimationClip failClip;
            public AnimationClip specialClip;
        }

        [Header("Member Object Bindings")]
        public List<MemberObjectBinding> memberObjects;

        [Header("Result UI")]
        public GameObject resultUI;

        private void Start()
        {
            ApplyBindings();
            director.stopped += OnTimelineEnd;
            director.Play();
            //resultUI.SetActive(false);
        }

        private void ApplyBindings()
        {
            var selected = TeamPracticeResultCache.SelectedMembers;
            bool isSuccess = TeamPracticeResultCache.IsSuccess;
            
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                foreach (var binding in memberObjects)
                {
                    if (track.name != binding.memberType.ToString())
                        continue;

                    var unit = selected.Find(m => m.memberType == binding.memberType);
                    if (unit == null)
                    {
                        binding.characterObject.SetActive(false);
                        continue;
                    }

                    binding.characterObject.SetActive(true);
                    director.SetGenericBinding(track, binding.characterObject);
                    
                    foreach (var clip in track.GetClips())
                    {
                        if (HasSpecialTrait(unit))
                        {
                            clip.asset = TimelineClipFromClip(binding.specialClip);
                        }
                        else
                        {
                            clip.asset = TimelineClipFromClip(isSuccess ? binding.successClip : binding.failClip);
                        }
                    }
                }
            }
        }

        private bool HasSpecialTrait(UnitDataSO unit)
        {
            //return unit.traits.Contains("TeamSuccessBoost");
            return false;
        }

        private AnimationPlayableAsset TimelineClipFromClip(AnimationClip clip)
        {
            return new AnimationPlayableAsset { clip = clip };
        }

        private void OnTimelineEnd(PlayableDirector director)
        {
            //resultUI.SetActive(true);
        }

        public void OnClickReturnToMain()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
