using System;
using UnityEngine;

namespace Code.SubSystem.Lobby
{
    public class Menu : MonoBehaviour
    {
        public MenuType type;
        public RectTransform rectTrm;

        private void Awake()
        {
            rectTrm = GetComponent<RectTransform>();
        }
        
        public virtual void Activate()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}