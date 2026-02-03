using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.IO;

namespace Code.MainSystem.StatSystem.BaseStats.Editor
{
    public class StatRankTableEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset view = default;

        private ListView _assetListView;
        private ListView _enumListView;
        private VisualElement _detailContainer;
        private TextField _enumInputField;

        private List<StatRankTable> _foundAssets = new();
        private List<string> _currentEnumNames = new();

        private const string AssetPath = "Assets/AddressableAssets/SO/StatData/RankData";
        private const string EnumFilePath = "Assets/Code/MainSystem/StatSystem/BaseStats/StatRankType.cs";

        [MenuItem("Tools/Stat/StatRank Manager")]
        public static void ShowWindow() => GetWindow<StatRankTableEditor>("Stat Rank Manager").Show();

        public void CreateGUI()
        {
            if (view == null)
                return;

            view.CloneTree(rootVisualElement);

            _assetListView = rootVisualElement.Q<ListView>("asset-list-view");
            _enumListView = rootVisualElement.Q<ListView>("enum-list-view");
            _detailContainer = rootVisualElement.Q<VisualElement>("detail-container");
            _enumInputField = rootVisualElement.Q<TextField>("enum-input-field");

            _assetListView.makeItem = () => new Label();
            _assetListView.bindItem = (e, i) => ((Label)e).text = _foundAssets[i].name;
            _assetListView.itemsSource = _foundAssets;
            _assetListView.selectionChanged += OnAssetSelected;

            rootVisualElement.Q<Button>("add-asset-btn").clicked += CreateNewAsset;
            rootVisualElement.Q<Button>("remove-asset-btn").clicked += RemoveSelectedAsset;

            _enumListView.makeItem = () => new Label();
            _enumListView.bindItem = (e, i) => ((Label)e).text = _currentEnumNames[i];
            _enumListView.itemsSource = _currentEnumNames;
            _enumListView.selectionChanged += (_) =>
            {
                if (_enumListView.selectedItem is string val) _enumInputField.value = val;
            };

            rootVisualElement.Q<Button>("add-enum-btn").clicked += () => AddEnumValue(_enumInputField.value);
            rootVisualElement.Q<Button>("remove-enum-btn").clicked += () => RemoveEnumValue(_enumInputField.value);

            RefreshAssetList();
            RefreshEnumList();
        }

        private void OnAssetSelected(IEnumerable<object> selectedItems)
        {
            _detailContainer.Clear();
            if (_assetListView.selectedItem is not StatRankTable selected) return;

            TextField nameField = new TextField("Asset Name") { value = selected.name };
            nameField.RegisterCallback<FocusOutEvent>(_ => RenameAsset(selected, nameField.value));
            _detailContainer.Add(nameField);

            _detailContainer.Add(new VisualElement
                { style = { height = 10, borderBottomWidth = 1, borderBottomColor = Color.gray, marginBottom = 10 } });
            _detailContainer.Add(new InspectorElement(new SerializedObject(selected)));
        }

        private void RefreshAssetList()
        {
            _foundAssets.Clear();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(StatRankTable)}");
            foreach (var guid in guids)
                _foundAssets.Add(AssetDatabase.LoadAssetAtPath<StatRankTable>(AssetDatabase.GUIDToAssetPath(guid)));
            _assetListView.RefreshItems();
        }

        private void RefreshEnumList()
        {
            _currentEnumNames.Clear();
            _currentEnumNames.AddRange(Enum.GetNames(typeof(StatRankType)));
            _enumListView.RefreshItems();
        }

        private void AddEnumValue(string n)
        {
            n = n.Trim();
            if (string.IsNullOrEmpty(n) || _currentEnumNames.Contains(n))
                return;

            _currentEnumNames.Add(n);
            UpdateEnumFile(_currentEnumNames);
        }

        private void RemoveEnumValue(string n)
        {
            if (n == "None" || !_currentEnumNames.Contains(n) ||
                !EditorUtility.DisplayDialog("삭제", $"'{n}'랭크 삭제?", "Yes", "No"))
                return;

            _currentEnumNames.Remove(n);
            UpdateEnumFile(_currentEnumNames);
        }

        private void UpdateEnumFile(List<string> list)
        {
            string content = $@"namespace Code.MainSystem.StatSystem.BaseStats
{{
    public enum StatRankType
    {{
        {string.Join(",\n        ", list)}
    }}
}}";
            File.WriteAllText(EnumFilePath,
                content);
            AssetDatabase.Refresh();
        }

        private void RenameAsset(StatRankTable a, string n)
        {
            if (string.IsNullOrEmpty(n) || a.name == n)
                return;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(a), n);
            AssetDatabase.SaveAssets();
            RefreshAssetList();
        }

        private void CreateNewAsset()
        {
            if (!Directory.Exists(AssetPath))
                Directory.CreateDirectory(AssetPath);

            string p = AssetDatabase.GenerateUniqueAssetPath($"{AssetPath}/NewStatRank.asset");
            AssetDatabase.CreateAsset(CreateInstance<StatRankTable>(), p);
            AssetDatabase.SaveAssets();
            RefreshAssetList();
        }

        private void RemoveSelectedAsset()
        {
            if (_assetListView.selectedItem is not StatRankTable item ||
                !EditorUtility.DisplayDialog("삭제", $"{item.name}를 삭제 하시겠습니까?", "Yes", "No"))
                return;

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
            RefreshAssetList();
            _detailContainer.Clear();
        }
    }
}