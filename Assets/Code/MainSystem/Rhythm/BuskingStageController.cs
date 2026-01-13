using UnityEngine;
using System.Collections.Generic;

namespace Code.MainSystem.Rhythm
{
    public class BuskingStageController : StageController
    {
        [SerializeField] private GameObject[] crowdGroups;

        protected override void OnProgressUpdated(float progress)
        {
            if (crowdGroups == null || crowdGroups.Length == 0) return;

            int activeCount = Mathf.FloorToInt(progress * crowdGroups.Length);
            
            for (int i = 0; i < crowdGroups.Length; i++)
            {
                if (crowdGroups[i] != null)
                {
                    bool shouldBeActive = i < activeCount;
                    if (crowdGroups[i].activeSelf != shouldBeActive)
                    {
                        crowdGroups[i].SetActive(shouldBeActive);
                    }
                }
            }
        }
    }
}
