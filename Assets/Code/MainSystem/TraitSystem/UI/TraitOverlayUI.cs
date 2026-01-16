using UnityEngine;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitOverlayUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlayCanvasGroup;
        private int _panelCount;
        private const float AlphaPerPanel = 0.3f;

        private void Awake()
        {
            _panelCount = 0;
            overlayCanvasGroup.alpha = 0;
            overlayCanvasGroup.gameObject.SetActive(false);
        }

        public void OnPanelOpened()
        {
            Debug.Log("OnPanelOpened");
            _panelCount++;
            UpdateOverlayAlpha();
        }
    
        public void OnPanelClosed()
        {
            if (_panelCount > 0)
                _panelCount--;
    
            UpdateOverlayAlpha();
    
            if (_panelCount == 0)
                overlayCanvasGroup.gameObject.SetActive(false);
        }
    
        private void UpdateOverlayAlpha()
        {
            float newAlpha = Mathf.Min(_panelCount * AlphaPerPanel, 0.8f);
            Debug.Log($"Panel Count: {_panelCount}, New Alpha: {newAlpha}");
    
            overlayCanvasGroup.alpha = newAlpha;
            overlayCanvasGroup.gameObject.SetActive(true);
        }
    }
}