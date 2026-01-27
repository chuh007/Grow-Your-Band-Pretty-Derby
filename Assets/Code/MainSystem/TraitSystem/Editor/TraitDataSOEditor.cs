using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Code.MainSystem.TraitSystem.Data;
using UnityEditor.UIElements;

namespace Code.MainSystem.TraitSystem.Editor
{
    [CustomEditor(typeof(TraitDataSO))]
    public class TraitDataSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset view = default;
        
        private VisualElement _root;

        private Dictionary<string, string> _effectTypeDict = new();
        private Dictionary<string, string> _conditionTypeDict = new();

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            
            InspectorElement.FillDefaultInspector(_root, serializedObject, this);
            
            if (view != null)
                view.CloneTree(_root);
            
            _effectTypeDict = GetDerivedTypesDict(typeof(TraitEffect.AbstractTraitEffect));
            _conditionTypeDict = GetDerivedTypesDict(typeof(TraitConditions.AbstractTraitCondition));
            
            SetupDropdown("EffectDropdown", "effectName", _effectTypeDict);
            SetupDropdown("ConditionDropdown", "conditionName", _conditionTypeDict);
            
            return _root;
        }

        private Dictionary<string, string> GetDerivedTypesDict(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assem => assem.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract && type != baseType)
                .ToDictionary(
                    type => type.Name,
                    type => type.AssemblyQualifiedName
                );
        }

        private void SetupDropdown(string visualElementName, string propertyName, Dictionary<string, string> typeDict)
        {
            DropdownField dropdown = _root.Q<DropdownField>(visualElementName);
            if (dropdown == null) return;

            SerializedProperty property = serializedObject.FindProperty(propertyName);
            
            dropdown.choices = typeDict.Keys.ToList();
            
            string currentFullValue = property.stringValue;
            string currentDisplayName = typeDict.FirstOrDefault(x => x.Value == currentFullValue).Key;

            if (string.IsNullOrEmpty(currentDisplayName) && dropdown.choices.Count > 0)
            {
                currentDisplayName = dropdown.choices[0];
                UpdateProperty(property, typeDict[currentDisplayName]);
            }

            dropdown.value = currentDisplayName;
            
            dropdown.RegisterValueChangedCallback(evt =>
            {
                if (typeDict.TryGetValue(evt.newValue, out string fullTypeName))
                {
                    UpdateProperty(property, fullTypeName);
                }
            });
        }

        private void UpdateProperty(SerializedProperty property, string value)
        {
            property.stringValue = value;
            serializedObject.ApplyModifiedProperties();
        }
    }
}