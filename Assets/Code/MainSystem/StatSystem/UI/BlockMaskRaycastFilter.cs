using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.StatSystem.UI
{
    public class BlockMaskRaycastFilter : Image
    {
        private readonly List<RectTransform> _passThroughAreas = new();

        public void SetPassThroughAreas(List<RectTransform> areas)
        {
            _passThroughAreas.Clear();
            _passThroughAreas.AddRange(areas);
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            foreach (var area in _passThroughAreas)
            {
                if (area == null)
                    continue;

                if (RectTransformUtility.RectangleContainsScreenPoint(
                        area,
                        screenPoint,
                        eventCamera))
                {
                    return false;
                }
            }

            return true;
        }
    }
}