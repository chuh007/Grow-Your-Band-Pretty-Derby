using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Editor
{
    [CustomEditor(typeof(TraitDataSO))]
    public class TraitDataSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset view = default;
        
        private TraitDataSO _targetSO;
        private VisualElement _root;
        private List<string> _cachedEffectTypes;
        private List<string> _cachedConditionTypes;

        public override VisualElement CreateInspectorGUI()
        {
            _targetSO = target as TraitDataSO;
            _root = new VisualElement();
            
            InspectorElement.FillDefaultInspector(_root, serializedObject, this);
            
            view.CloneTree(_root);
            
            _cachedEffectTypes = GetDerivedTypes(typeof(TraitEffect.AbstractTraitEffect));
            _cachedConditionTypes = GetDerivedTypes(typeof(TraitConditions.AbstractTraitCondition));
            
            MakeTraitEffectDropdown();
            MakeTraitConditionDropdown();
            
            return _root;
        }

        private List<string> GetDerivedTypes(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assem => assem.GetTypes())
                .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract)
                .Select(type => type.AssemblyQualifiedName)
                .ToList();
        }

        private void MakeTraitEffectDropdown()
        {
            DropdownField dropdown = _root.Q<DropdownField>("EffectDropdown");
            SetupDropdown(dropdown, _cachedEffectTypes, _targetSO.effectName, 
                newValue => _targetSO.effectName = newValue);
        }

        private void MakeTraitConditionDropdown()
        {
            DropdownField dropdown = _root.Q<DropdownField>("ConditionDropdown");
            
            SetupDropdown(dropdown, _cachedConditionTypes, _targetSO.conditionName, 
                newValue => _targetSO.conditionName = newValue);
        }
        
        private void SetupDropdown(DropdownField dropdown, List<string> choices, string currentValue, Action<string> onValueChanged)
        {
            dropdown.choices = choices;
            
            if (!dropdown.choices.Contains(currentValue))
            {
                currentValue = dropdown.choices.Count > 0 
                    ? dropdown.choices.First() 
                    : string.Empty;
                    
                onValueChanged(currentValue);
                EditorUtility.SetDirty(_targetSO);
            }

            dropdown.value = currentValue;
            
            dropdown.RegisterValueChangedCallback(evt =>
            {
                onValueChanged(evt.newValue);
                EditorUtility.SetDirty(_targetSO);
            });
        }
    }
}