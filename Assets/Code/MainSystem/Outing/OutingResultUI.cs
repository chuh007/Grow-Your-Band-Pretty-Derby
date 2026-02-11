using System.Collections.Generic;
using System.Text;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.StatSystem.BaseStats;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    public class OutingResultUI : MonoBehaviour
    {
        [SerializeField] private DialogCutsceneSenderSO resultSender;
        [SerializeField] private Button closeButton;
        
        private string _sceneName;
        
        public void ShowResultUI(string SceneName)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseOutingScene);
            
            _sceneName = SceneName;
        }
        
        private void CloseOutingScene()
        {
            closeButton.onClick.RemoveAllListeners();
            Bus<CutsceneEndEvent>.Raise(new CutsceneEndEvent());
            // SceneManager.UnloadSceneAsync(_sceneName);
            SceneManager.LoadScene("Lch");
        }
    }
}