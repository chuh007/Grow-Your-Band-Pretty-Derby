using System.Collections.Generic;
using System.Linq;
using Code.MainSystem.TraitSystem.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.MainSystem.TraitSystem.Editor
{
    public class TraitEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset mainView = default;
        [SerializeField] private VisualTreeAsset itemView = default;

        private List<TraitDataSO> _traits = new();
        private SerializedObject _serializedTrait;
        
        private ListView _traitListView;
        private ListView _impactListView;
        private VisualElement _detailPanel;

        [MenuItem("Tools/Trait/Trait Editor")]
        public static void ShowWindow() 
            => GetWindow<TraitEditor>("Trait Editor");

        public void CreateGUI()
        {
            if (mainView == null || itemView == null)
            {
                rootVisualElement.Add(new Label("UXML 파일이 할당되지 않았습니다."));
                return;
            }

            mainView.CloneTree(rootVisualElement);

            _traitListView = rootVisualElement.Q<ListView>("trait-list");
            _impactListView = rootVisualElement.Q<ListView>("impact-list");
            _detailPanel = rootVisualElement.Q<VisualElement>("base-settings");

            SetupTraitListView();
            SetupImpactListView();
            
            rootVisualElement.Q<Button>("add-trait-btn").clicked += CreateNewTrait;
            rootVisualElement.Q<Button>("add-pair-btn").clicked += AddEffectPair;

            RefreshTraitList();
        }

        private void SetupTraitListView()
        {
            _traitListView.makeItem = () => new Label();
            _traitListView.bindItem = (e, i) =>
            {
                var label = e as Label;
                var trait = _traits[i];
                if (label != null) 
                    label.text = string.IsNullOrEmpty(trait.TraitName) ? trait.name : trait.TraitName;
            };

            _traitListView.selectionChanged += (objs) => 
                DrawTraitDetail(objs.FirstOrDefault() as TraitDataSO);
        }

        private void SetupImpactListView()
        {
            _impactListView.makeItem = () => itemView.CloneTree();
            _impactListView.bindItem = BindImpactItem;
        }

        private void BindImpactItem(VisualElement element, int i)
        {
            if (_serializedTrait == null) return;

            var effectsProp = _serializedTrait.FindProperty("Effects");
            var impactsProp = _serializedTrait.FindProperty("Impacts");

            if (i >= effectsProp.arraySize || i >= impactsProp.arraySize) return;

            element.Q<Label>("id-label").text = $"N{i + 1}";
            
            SetField(element, "effect-field", effectsProp.GetArrayElementAtIndex(i));
            
            var impactElem = impactsProp.GetArrayElementAtIndex(i);
            SetField(element, "target-field", impactElem.FindPropertyRelative("Target"));
            SetField(element, "calc-field", impactElem.FindPropertyRelative("CalcType"));
            SetField(element, "tag-field", impactElem.FindPropertyRelative("RequiredTag"));
            
            var removeBtn = element.Q<Button>("remove-btn");
            removeBtn.clicked += () => RemoveEffectPair(i);
        }

        private void SetField(VisualElement root, string name, SerializedProperty prop)
        {
            var field = root.Q<PropertyField>(name);
            if (field == null)
                return;
            
            field.BindProperty(prop);
            field.label = "";
        }

        private void DrawTraitDetail(TraitDataSO target)
        {
            _detailPanel.Unbind();
    
            if (target == null)
            {
                _detailPanel.style.display = DisplayStyle.None;
                _serializedTrait = null;
                return;
            }

            _detailPanel.style.display = DisplayStyle.Flex;
            
            _serializedTrait = new SerializedObject(target);
            _detailPanel.Bind(_serializedTrait);
            
            _impactListView.itemsSource = target.Effects;
            _impactListView.Rebuild();
            
            _detailPanel.TrackSerializedObjectValue(_serializedTrait, _ => {
                _traitListView.RefreshItem(_traitListView.selectedIndex);
            });
        }

        private void AddEffectPair()
        {
            if (_serializedTrait == null) return;

            _serializedTrait.Update();
            var effectsProp = _serializedTrait.FindProperty("Effects");
            var impactsProp = _serializedTrait.FindProperty("Impacts");

            int newIndex = effectsProp.arraySize;
            effectsProp.InsertArrayElementAtIndex(newIndex);
            impactsProp.InsertArrayElementAtIndex(newIndex);

            _serializedTrait.ApplyModifiedProperties();
            _impactListView.Rebuild();
        }

        private void RemoveEffectPair(int index)
        {
            if (_serializedTrait == null) return;

            _serializedTrait.Update();
            var effectsProp = _serializedTrait.FindProperty("Effects");
            var impactsProp = _serializedTrait.FindProperty("Impacts");

            if (index < effectsProp.arraySize)
            {
                effectsProp.DeleteArrayElementAtIndex(index);
                impactsProp.DeleteArrayElementAtIndex(index);
            }

            _serializedTrait.ApplyModifiedProperties();
            _impactListView.Rebuild();
        }

        private void RefreshTraitList()
        {
            _traits = AssetDatabase.FindAssets("t:TraitDataSO")
                .Select(guid => AssetDatabase.LoadAssetAtPath<TraitDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(t => t.TraitName)
                .ToList();

            _traitListView.itemsSource = _traits;
            _traitListView.Rebuild();
        }

        private void CreateNewTrait()
        {
            string path = EditorUtility.SaveFilePanelInProject("New Trait", "NewTrait", "asset", "Save Trait");
            if (string.IsNullOrEmpty(path)) return;

            var asset = CreateInstance<TraitDataSO>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            RefreshTraitList();
            
            _traitListView.selectedIndex = _traits.IndexOf(asset);
        }
    }
}