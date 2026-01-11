using System;
using System.Linq;
using UnityEditor;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Editor
{
    [CustomEditor(typeof(TraitDataSO))]
    public class TraitDataSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _condition;
        private SerializedProperty _effect;

        private void OnEnable()
        {
            _condition = serializedObject.FindProperty("Condition");
            _effect = serializedObject.FindProperty("Effect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Condition Setup", EditorStyles.boldLabel);
            DrawConditionSection();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Effect Setup", EditorStyles.boldLabel);
            DrawEffectSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditionSection()
        {
            var trait = (TraitDataSO)target;
            
            if (trait.Condition != null)
            {
                EditorGUILayout.HelpBox($"Current: {trait.Condition.GetType().Name}", MessageType.Info);
                
                DrawSerializeReferenceFields(_condition);

                if (GUILayout.Button("Remove Condition", GUILayout.Height(25)))
                {
                    Undo.RecordObject(trait, "Remove Condition");
                    trait.Condition = null;
                    EditorUtility.SetDirty(trait);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No condition set (always active)", MessageType.Warning);
            }
            
            if (GUILayout.Button("Add/Change Condition", GUILayout.Height(30)))
            {
                ShowConditionMenu(trait);
            }
        }

        private void DrawEffectSection()
        {
            var trait = (TraitDataSO)target;
            
            if (trait.Effect != null)
            {
                EditorGUILayout.HelpBox($"Current: {trait.Effect.GetType().Name}", MessageType.Info);
                
                DrawSerializeReferenceFields(_effect);

                if (GUILayout.Button("Remove Effect", GUILayout.Height(25)))
                {
                    Undo.RecordObject(trait, "Remove Effect");
                    trait.Effect = null;
                    EditorUtility.SetDirty(trait);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No effect set", MessageType.Warning);
            }
            
            if (GUILayout.Button("Add/Change Effect", GUILayout.Height(30)))
                ShowEffectMenu(trait);
        }

        private void ShowConditionMenu(TraitDataSO trait)
        {
            var menu = new GenericMenu();
            
            var conditionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ITraitCondition).IsAssignableFrom(type)
                               && !type.IsInterface
                               && !type.IsAbstract);

            foreach (var type in conditionTypes)
            {
                var typeName = type.Name.Replace("Condition", "");
                menu.AddItem(new GUIContent(typeName), false, () =>
                {
                    Undo.RecordObject(trait, "Change Condition");
                    trait.Condition = (ITraitCondition)Activator.CreateInstance(type);
                    EditorUtility.SetDirty(trait);
                });
            }

            menu.ShowAsContext();
        }

        private void ShowEffectMenu(TraitDataSO trait)
        {
            var menu = new GenericMenu();
            
            var effectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ITraitEffect).IsAssignableFrom(type)
                               && !type.IsInterface
                               && !type.IsAbstract);

            foreach (var type in effectTypes)
            {
                var typeName = type.Name.Replace("Effect", "");
                menu.AddItem(new GUIContent(typeName), false, () =>
                {
                    Undo.RecordObject(trait, "Change Effect");
                    trait.Effect = (ITraitEffect)Activator.CreateInstance(type);
                    EditorUtility.SetDirty(trait);
                });
            }

            menu.ShowAsContext();
        }

        private void DrawSerializeReferenceFields(SerializedProperty property)
        {
            if (property.managedReferenceValue == null)
                return;

            EditorGUI.indentLevel++;

            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(iterator, endProperty))
                    break;

                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }

            EditorGUI.indentLevel--;
        }
    }
}