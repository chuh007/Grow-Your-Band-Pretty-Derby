using UnityEngine;
using Reflex.Attributes;

namespace Code.MainSystem.TraitSystem.UI
{
    public abstract class TraitPanelBase : MonoBehaviour
    {
        [Inject] private TraitOverlayUI _overlayUI;
        [SerializeField] protected GameObject panel;

        protected virtual void Show()
        {
            _overlayUI.OnPanelOpened(panel);
            panel.SetActive(true);
        }

        protected virtual void Hide()
        {
            _overlayUI.OnPanelClosed(panel);
            panel.SetActive(false);
        }

        public virtual void Cancel()
        {
            Hide();
        }
    }
}