using System.Threading.Tasks;
using Code.Core.Addressable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Core
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        [field: SerializeField] public int Priority { get; private set; } = 1;
        //[SerializeField] private LoadingScreenUI LoadingScreenUI;
        bool isKey = false;
        
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if(_instance is null)
                    _instance = FindFirstObjectByType<GameManager>();
                return _instance;
            }
        }

        private GameResourceManager _resourceManager = new();

        private void Awake()
        {
            GameManager[] managers = FindObjectsByType<GameManager>(FindObjectsSortMode.None);

            if (managers.Length > 1)
            {
                int highPriority = managers[0].Priority;
                GameManager highPriorityManager = managers[0];
                for (int i = 0; i < managers.Length; i++)
                {
                    if (managers[i].Priority > highPriority)
                    {
                        Destroy(highPriorityManager.gameObject);
                        highPriority = managers[i].Priority;
                        highPriorityManager = managers[i];
                    }
                }
                _instance = highPriorityManager;
            }
            else
            {
                _instance = this;
            }
            
            _resourceManager = new GameResourceManager();
            _resourceManager.OnLoadCountChange += HandleLoadCountChange;
            
            DontDestroyOnLoad(gameObject);
            //_ = LoadProcess();
        }

        private void HandleLoadCountChange()
        {
            // if(LoadingScreenUI == null)return;
            // LoadingScreenUI.SetProgress(_resourceManager.LoadCount,_resourceManager.NeedLoadCount);
        }

        // private async Task LoadProcess()
        // {
        //     // isKey =false;
        //     // _resourceManager.LoadCount = 0;
        //     // _resourceManager.NeedLoadCount = 0;
        //     // await Task.WhenAll(_resourceManager.LoadAllAsync<AudioClip>("BGM"),
        //     //     _resourceManager.LoadAllAsync<GameObject>("default"));
        //     // if (LoadingScreenUI != null)
        //     // {
        //     //     LoadingScreenUI.SetMessage("Loading complete..\npress any key to continue");
        //     //     isKey = true;
        //     // }
        // }

        public T LoadAsset<T>(string key) where T : Object
        {
            return _resourceManager.Load<T>(key);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _resourceManager?.Dispose();
            }
        }
    }
}