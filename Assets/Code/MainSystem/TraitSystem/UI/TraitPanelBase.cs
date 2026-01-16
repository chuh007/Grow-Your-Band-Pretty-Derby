using UnityEngine;

namespace Code.MainSystem.TraitSystem.UI
{
    public abstract class TraitPanelBase : MonoBehaviour
    {
        [SerializeField] protected GameObject panel;
        [SerializeField] private CanvasGroup canvasGroup;

        protected virtual void Show()
        {
            panel.SetActive(true);
        }

        protected virtual void Hide()
        {
            panel.SetActive(false);
        }

        public virtual void Cancel()
        {
            Hide();
        }
    }
}