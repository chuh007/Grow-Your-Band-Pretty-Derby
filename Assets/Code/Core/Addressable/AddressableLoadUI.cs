using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.Core.Addressable
{
    public class AddressableLoadUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private RectTransform progressBar;
        [SerializeField] private TextMeshProUGUI loadingText;
        
        private int _totalSteps = 0;
        private int _currentStep = 0;

        public void ShowLoadingUI(int totalSteps)
        {
            _totalSteps = totalSteps;
            _currentStep = 0;
            
            Debug.Log($"[LoadingUI] ShowLoadingUI - Total Steps: {totalSteps}");
            
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }
            
            UpdateLoadingUI();
        }

        public void UpdateProgress(string message)
        {
            _currentStep++;
            
            Debug.Log($"[LoadingUI] Step {_currentStep}/{_totalSteps}: {message}");
            
            if (loadingText != null)
            {
                loadingText.text = message;
            }
            
            UpdateLoadingUI();
        }

        public void HideLoadingUI()
        {
            Debug.Log("[LoadingUI] HideLoadingUI called");
            
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }

        private void UpdateLoadingUI()
        {
            if (_totalSteps == 0) return;

            float progress = (float)_currentStep / _totalSteps;
            
            if (progressBar != null)
            {
                progressBar.localScale = new Vector3(progress, 1f, 1f);
            }
            
            if (_currentStep >= _totalSteps)
            {
                Debug.Log("[LoadingUI] Loading completed!");
                Invoke(nameof(HideLoadingUI), 0.5f);
            }
        }
    }
}