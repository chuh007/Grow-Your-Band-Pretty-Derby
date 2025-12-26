using Code.Core.Bus;
using UnityEngine;

namespace Code.Core.Bus.GameEvents
{
    public struct ImageDisplayEvent : IEvent
    {
        public GameObject ImagePrefabToDisplay { get; }

        public ImageDisplayEvent(GameObject imagePrefabToDisplay)
        {
            ImagePrefabToDisplay = imagePrefabToDisplay;
        }
    }
}