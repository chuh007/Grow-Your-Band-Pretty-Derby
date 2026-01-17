using System;
using System.Linq;
using UnityEditor;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.TraitConditions;
using Code.MainSystem.TraitSystem.TraitEffect;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Editor
{
    [CustomEditor(typeof(TraitDataSO))]
    public class TraitDataSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _condition;
        private SerializedProperty _effect;
        
        private bool _showBasicInfo = true;
        private bool _showPointSystem = true;
        private bool _showEffectValues = true;
        private bool _showConditionSetup = true;
        private bool _showEffectSetup = true;

        private GUIStyle _headerStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _boxStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _removeButtonStyle;

        private void OnEnable()
        {
            _condition = serializedObject.FindProperty("Condition");
            _effect = serializedObject.FindProperty("Effect");
        }

        public override void OnInspectorGUI()
        {
            InitializeStyles();
            
            serializedObject.Update();
            
            DrawTratHeader();
            
            EditorGUILayout.Space(5);
            DrawBasicInfoSection();
            
            EditorGUILayout.Space(5);
            DrawPointSystemSection();
            
            EditorGUILayout.Space(5);
            DrawEffectValuesSection();
            
            EditorGUILayout.Space(10);
            DrawConditionSection();
            
            EditorGUILayout.Space(5);
            DrawEffectSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeStyles()
        {
            _headerStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.8f, 0.9f, 1f) }
            };

            _sectionStyle ??= new GUIStyle(EditorStyles.foldoutHeader)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            _boxStyle ??= new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };

            _buttonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30
            };

            _removeButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = new Color(1f, 0.5f, 0.5f) },
                hover = { textColor = Color.red },
                fontStyle = FontStyle.Bold,
                fixedHeight = 25
            };
        }

        private void DrawTratHeader()
        {
            EditorGUILayout.BeginVertical(_boxStyle);
            
            EditorGUILayout.LabelField("🎯 Trait Data Configuration", _headerStyle, GUILayout.Height(30));
            
            var trait = (TraitDataSO)target;
            if (trait.TraitType != TraitType.NoneTrait)
            {
                var pointColor = trait.Point >= 0 ? "green" : "red";
                EditorGUILayout.LabelField(
                    $"<b>{trait.TraitName}</b> | Type: {trait.TraitType} | Point: <color={pointColor}>{trait.Point}</color>", 
                    new GUIStyle(EditorStyles.centeredGreyMiniLabel) { richText = true }
                );
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawBasicInfoSection()
        {
            EditorGUILayout.BeginVertical(_boxStyle);
            
            _showBasicInfo = EditorGUILayout.Foldout(_showBasicInfo, "📋 Basic Information", true, _sectionStyle);
            
            if (_showBasicInfo)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TraitID"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TraitType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TraitName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TraitIcon"));
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Description", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DescriptionEffect"), GUIContent.none);
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawPointSystemSection()
        {
            EditorGUILayout.BeginVertical(_boxStyle);
            
            _showPointSystem = EditorGUILayout.Foldout(_showPointSystem, "⚖️ Point & Level System", true, _sectionStyle);
            
            if (_showPointSystem)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Point"));
                var point = serializedObject.FindProperty("Point").intValue;
                var pointLabel = point >= 0 ? "Positive" : "Negative";
                var pointColor = point >= 0 ? Color.green : Color.red;
                
                var originalColor = GUI.color;
                GUI.color = pointColor;
                EditorGUILayout.LabelField($"({pointLabel})", GUILayout.Width(80));
                GUI.color = originalColor;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsRemovable"));
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Level"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxLevel"));
                EditorGUILayout.EndHorizontal();
                
                var level = serializedObject.FindProperty("Level").intValue;
                var maxLevel = serializedObject.FindProperty("MaxLevel").intValue;
                
                if (maxLevel == -1)
                {
                    EditorGUILayout.HelpBox("MaxLevel: -1 = No level up possible", MessageType.Info);
                }
                else if (level >= maxLevel)
                {
                    EditorGUILayout.HelpBox("Max level reached", MessageType.Warning);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawEffectValuesSection()
        {
            EditorGUILayout.BeginVertical(_boxStyle);
            
            _showEffectValues = EditorGUILayout.Foldout(_showEffectValues, "🔢 Effect Values (N1, N2, N3...)", true, _sectionStyle);
            
            if (_showEffectValues)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("traitEffectType"), new GUIContent("Effect Timing"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("conditionType"), new GUIContent("Condition Type"));
                
                EditorGUILayout.Space(5);
                
                var effectsProp = serializedObject.FindProperty("Effects");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Effect Values", EditorStyles.boldLabel);
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    effectsProp.arraySize++;
                }
                if (GUILayout.Button("-", GUILayout.Width(30)) && effectsProp.arraySize > 0)
                {
                    effectsProp.arraySize--;
                }
                EditorGUILayout.EndHorizontal();
                
                if (effectsProp.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("No effect values set", MessageType.Info);
                }
                else
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < effectsProp.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"N{i + 1}", GUILayout.Width(30));
                        EditorGUILayout.PropertyField(effectsProp.GetArrayElementAtIndex(i), GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionSection()
        {
            var trait = (TraitDataSO)target;
            
            EditorGUILayout.BeginVertical(_boxStyle);
            
            _showConditionSetup = EditorGUILayout.Foldout(_showConditionSetup, "🔍 Condition Setup (Advanced)", true, _sectionStyle);
            
            if (_showConditionSetup)
            {
                EditorGUI.indentLevel++;
                
                if (trait.Condition != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox($"✓ {FormatTypeName(trait.Condition.GetType().Name)}", MessageType.Info);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(5);
                    DrawSerializeReferenceFields(_condition);
                    
                    EditorGUILayout.Space(5);
                    
                    if (GUILayout.Button("✖ Remove Condition", _removeButtonStyle))
                    {
                        Undo.RecordObject(trait, "Remove Condition");
                        trait.Condition = null;
                        EditorUtility.SetDirty(trait);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("⚠ No condition set (Always active)", MessageType.Warning);
                }
                
                EditorGUILayout.Space(5);
                
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
                
                if (GUILayout.Button(trait.Condition == null ? "➕ Add Condition" : "🔄 Change Condition", _buttonStyle))
                {
                    ShowConditionMenu(trait);
                }
                
                GUI.backgroundColor = originalColor;
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawEffectSection()
        {
            var trait = (TraitDataSO)target;
            
            EditorGUILayout.BeginVertical(_boxStyle);
            
            _showEffectSetup = EditorGUILayout.Foldout(_showEffectSetup, "✨ Effect Setup (Advanced)", true, _sectionStyle);
            
            if (_showEffectSetup)
            {
                EditorGUI.indentLevel++;
                
                if (trait.Effect != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox($"✓ {FormatTypeName(trait.Effect.GetType().Name)}", MessageType.Info);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(5);
                    DrawSerializeReferenceFields(_effect);
                    
                    EditorGUILayout.Space(5);
                    
                    if (GUILayout.Button("✖ Remove Effect", _removeButtonStyle))
                    {
                        Undo.RecordObject(trait, "Remove Effect");
                        trait.Effect = null;
                        EditorUtility.SetDirty(trait);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("⚠ No effect set", MessageType.Warning);
                }
                
                EditorGUILayout.Space(5);
                
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 0.9f, 0.7f);
                
                if (GUILayout.Button(trait.Effect == null ? "➕ Add Effect" : "🔄 Change Effect", _buttonStyle))
                {
                    ShowEffectMenu(trait);
                }
                
                GUI.backgroundColor = originalColor;
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void ShowConditionMenu(TraitDataSO trait)
        {
            var menu = new GenericMenu();
            
            var conditionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ITraitCondition).IsAssignableFrom(type)
                               && !type.IsInterface
                               && !type.IsAbstract)
                .OrderBy(type => type.Name);

            if (!conditionTypes.Any())
            {
                menu.AddDisabledItem(new GUIContent("No conditions available"));
            }
            else
            {
                foreach (var type in conditionTypes)
                {
                    var menuPath = GetMenuPath(type, "Condition");
                    
                    menu.AddItem(new GUIContent(menuPath), false, () =>
                    {
                        Undo.RecordObject(trait, "Change Condition");
                        trait.Condition = (ITraitCondition)Activator.CreateInstance(type);
                        EditorUtility.SetDirty(trait);
                    });
                }
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
                               && !type.IsAbstract)
                .OrderBy(type => type.Name);

            if (!effectTypes.Any())
            {
                menu.AddDisabledItem(new GUIContent("No effects available"));
            }
            else
            {
                foreach (var type in effectTypes)
                {
                    var menuPath = GetMenuPath(type, "Effect");
                    
                    menu.AddItem(new GUIContent(menuPath), false, () =>
                    {
                        Undo.RecordObject(trait, "Change Effect");
                        trait.Effect = (ITraitEffect)Activator.CreateInstance(type);
                        EditorUtility.SetDirty(trait);
                    });
                }
            }

            menu.ShowAsContext();
        }

        private void DrawSerializeReferenceFields(SerializedProperty property)
        {
            if (property.managedReferenceValue == null)
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Parameters", EditorStyles.miniBoldLabel);
            
            EditorGUI.indentLevel++;

            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            bool enterChildren = true;
            bool hasFields = false;

            while (iterator.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(iterator, endProperty))
                    break;

                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
                hasFields = true;
            }

            if (!hasFields)
            {
                EditorGUILayout.LabelField("No parameters", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
        }

        private string FormatTypeName(string typeName)
        {
            typeName = typeName.Replace("Condition", "").Replace("Effect", "");
            return System.Text.RegularExpressions.Regex.Replace(typeName, "([a-z])([A-Z])", "$1 $2");
        }

        private string GetMenuPath(Type type, string suffix)
        {
            var typeName = type.Name.Replace(suffix, "");
            
            if (type.Namespace != null)
            {
                if (type.Namespace.Contains("Training"))
                    return $"Training/{FormatTypeName(typeName)}";
                if (type.Namespace.Contains("Ensemble"))
                    return $"Ensemble/{FormatTypeName(typeName)}";
                if (type.Namespace.Contains("Performance"))
                    return $"Performance/{FormatTypeName(typeName)}";
                if (type.Namespace.Contains("Stat"))
                    return $"Stats/{FormatTypeName(typeName)}";
            }
            
            return $"General/{FormatTypeName(typeName)}";
        }
    }
}