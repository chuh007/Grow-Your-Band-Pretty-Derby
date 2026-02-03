using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;

namespace Code.MainSystem.StatSystem.BaseStats.Editor
{
    public class StatRankTableEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset view = default;
        
        private ListView _assetListView;
        private VisualElement _detailContainer;
        private List<StatRankTable> _foundAssets = new();
        
        private const string AssetPath = "Assets/AddressableAssets/StatData/RankData";

        [MenuItem("Tools/Stat/StatRank Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<StatRankTableEditor>();
            window.titleContent = new GUIContent("Stat Rank Manager");
            window.minSize = new Vector2(750, 450);
            window.Show();
        }

        public void CreateGUI()
        {
            if (view == null)
                return;
            
            view.CloneTree(rootVisualElement);

            _assetListView = rootVisualElement.Q<ListView>("asset-list-view");
            _detailContainer = rootVisualElement.Q<VisualElement>("detail-container");

            _assetListView.makeItem = () => new Label();
            _assetListView.bindItem = (e, i) => ((Label)e).text = _foundAssets[i].name;
            _assetListView.itemsSource = _foundAssets;
            _assetListView.selectionChanged += OnAssetSelected;

            rootVisualElement.Q<Button>("add-asset-btn").clicked += CreateNewAsset;
            rootVisualElement.Q<Button>("remove-asset-btn").clicked += RemoveSelectedAsset;

            RefreshAssetList();
        }

        private void RefreshAssetList()
        {
            _foundAssets.Clear();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(StatRankTable)}");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                _foundAssets.Add(AssetDatabase.LoadAssetAtPath<StatRankTable>(path));
            }
            _assetListView.RefreshItems();
        }

        private void OnAssetSelected(IEnumerable<object> selectedItems)
        {
            _detailContainer.Clear();
            StatRankTable selected = _assetListView.selectedItem as StatRankTable;
            if (selected == null)
                return;

            TextField nameField = new TextField("Asset Name") { value = selected.name };

            nameField.RegisterCallback<FocusOutEvent>(_ => RenameAsset(selected, nameField.value));

            _detailContainer.Add(nameField);

            VisualElement spacer = new VisualElement
                { style = { height = 10, borderBottomWidth = 1, borderBottomColor = Color.gray, marginBottom = 10 } };
            _detailContainer.Add(spacer);

            SerializedObject so = new SerializedObject(selected);
            InspectorElement inspector = new InspectorElement(so);
            _detailContainer.Add(inspector);
        }

        private void RenameAsset(StatRankTable asset, string newName)
        {
            if (string.IsNullOrEmpty(newName) || asset.name == newName)
                return;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.RenameAsset(assetPath, newName);
        }

        private void CreateNewAsset()
        {
            if (!Directory.Exists(AssetPath))
                Directory.CreateDirectory(AssetPath);
            
            StatRankTable newAsset = CreateInstance<StatRankTable>();
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{AssetPath}/NewStatRank.asset");
            AssetDatabase.CreateAsset(newAsset, uniquePath);
            AssetDatabase.SaveAssets();
            RefreshAssetList();
        }

        private void RemoveSelectedAsset()
        {
            StatRankTable selected = _assetListView.selectedItem as StatRankTable;
            
            if (selected == null)
                return;
            
            if (!EditorUtility.DisplayDialog("Delete Asset", $"정말로 {selected.name}을 삭제하시겠습니까?", "Yes", "No"))
                return;
            
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selected));
            RefreshAssetList();
            _detailContainer.Clear();
        }
    }
}