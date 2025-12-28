using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.MainSystem.StatSystem.UI
{
    public class SelectRequiredUI : MonoBehaviour
    {
        [SerializeField] private GameObject blockMaskPanel;
        [SerializeField] private Transform cloneRoot;
        [SerializeField] private List<GameObject> passthroughPanels;

        private readonly List<GameObject> _clones = new();
        private bool _initialized;

        private void Awake()
        {
            blockMaskPanel.SetActive(false);
        }

        private void OnEnable()
        {
            Bus<SelectRequiredEvent>.OnEvent += HandleSelectRequired;
        }

        private void OnDestroy()
        {
            Bus<SelectRequiredEvent>.OnEvent -= HandleSelectRequired;
        }

        private void HandleSelectRequired(SelectRequiredEvent evt)
        {
            blockMaskPanel.SetActive(true);

            if (!_initialized)
            {
                CreateVisualClones();
                _initialized = true;
            }

            SetClonesActive(true);
        }

        public void Close()
        {
            blockMaskPanel.SetActive(false);
            SetClonesActive(false);
        }

        private void CreateVisualClones()
        {
            foreach (var source in passthroughPanels)
            {
                if (source is null)
                    continue;

                var clone = CloneVisualHierarchy(source.transform, cloneRoot);
                clone.name = source.name + "_Clone";
                clone.SetActive(false);

                _clones.Add(clone);
            }
        }

        private void SetClonesActive(bool value)
        {
            foreach (var clone in _clones)
            {
                if (clone != null)
                    clone.SetActive(value);
            }
        }

        private GameObject CloneVisualHierarchy(Transform source, Transform parent)
        {
            var clone = new GameObject(source.name);
            clone.transform.SetParent(parent, false);

            CopyRectTransform(source, clone.transform);
            CopyVisuals(source, clone);

            foreach (Transform child in source)
            {
                CloneVisualHierarchy(child, clone.transform);
            }

            return clone;
        }

        private void CopyRectTransform(Transform source, Transform target)
        {
            var src = source as RectTransform;
            var dst = target.gameObject.AddComponent<RectTransform>();

            if (src is null)
                return;

            dst.anchorMin = src.anchorMin;
            dst.anchorMax = src.anchorMax;
            dst.pivot = src.pivot;
            dst.anchoredPosition = src.anchoredPosition;
            dst.sizeDelta = src.sizeDelta;
            dst.localRotation = src.localRotation;
            dst.localScale = src.localScale;
        }

        private void CopyVisuals(Transform source, GameObject target)
        {
            if (source.TryGetComponent<Image>(out var img))
            {
                var newImg = target.AddComponent<Image>();
                newImg.sprite = img.sprite;
                newImg.color = img.color;
                newImg.material = img.material;
                newImg.type = img.type;
                newImg.preserveAspect = img.preserveAspect;
                newImg.raycastTarget = false;
            }

            if (source.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                var newText = target.AddComponent<TextMeshProUGUI>();
                newText.text = text.text;
                newText.font = text.font;
                newText.fontSize = text.fontSize;
                newText.color = text.color;
                newText.alignment = text.alignment;
                newText.textWrappingMode = text.textWrappingMode;
                newText.raycastTarget = false;
            }
        }
    }
}