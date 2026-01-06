using System.Collections.Generic;
using System.Linq;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class SelectRequiredUI : MonoBehaviour
    {
        [SerializeField] private GameObject blockMaskPanel;
        [SerializeField] private List<GameObject> passthroughPanels;

        private readonly List<Canvas> _addedCanvases = new();
        private readonly List<GraphicRaycaster> _addedRaycasters = new();

        private void Awake()
        {
            Bus<SelectRequiredEvent>.OnEvent += HandleSelectRequired;
            blockMaskPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<SelectRequiredEvent>.OnEvent -= HandleSelectRequired;
            Cleanup();
        }

        private void HandleSelectRequired(SelectRequiredEvent evt)
        {
            blockMaskPanel.SetActive(true);
            SetupPassthroughUI();
        }

        public void Close()
        {
            blockMaskPanel.SetActive(false);
            Cleanup();
        }

        private void SetupPassthroughUI()
        {
            foreach (var target in passthroughPanels)
            {
                if (target == null)
                    continue;

                var canvas = target.GetComponent<Canvas>();
                if (canvas == null)
                    canvas = target.AddComponent<Canvas>();

                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;

                var raycaster = target.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                    raycaster = target.AddComponent<GraphicRaycaster>();

                _addedCanvases.Add(canvas);
                _addedRaycasters.Add(raycaster);
            }
        }

        private void Cleanup()
        {
            foreach (var item in _addedRaycasters.Where(item => item != null))
                Destroy(item);

            foreach (var item in _addedCanvases.Where(item => item != null))
                Destroy(item);

            _addedRaycasters.Clear();
            _addedCanvases.Clear();
        }
    }
}