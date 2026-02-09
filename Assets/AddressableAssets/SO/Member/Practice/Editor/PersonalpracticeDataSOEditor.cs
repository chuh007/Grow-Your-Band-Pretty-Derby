#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CustomEditor(typeof(PersonalpracticeDataSO))]
    public class PersonalpracticeDataSOEditor : UnityEditor.Editor
    {
        private SerializedProperty practiceStatType;
        private SerializedProperty practiceStatName;
        private SerializedProperty personalpracticeDescription;
        private SerializedProperty staminaReduction;
        private SerializedProperty statIncrease;
        private SerializedProperty personalsuccessComment;
        private SerializedProperty personalfaillComment;

        private bool showSuccessComment = true;
        private bool showFailComment = true;

        private void OnEnable()
        {
            practiceStatType = serializedObject.FindProperty("PracticeStatType");
            practiceStatName = serializedObject.FindProperty("PracticeStatName");
            personalpracticeDescription = serializedObject.FindProperty("PersonalpracticeDescription");
            staminaReduction = serializedObject.FindProperty("StaminaReduction");
            statIncrease = serializedObject.FindProperty("statIncrease");
            personalsuccessComment = serializedObject.FindProperty("PersonalsuccessComment");
            personalfaillComment = serializedObject.FindProperty("PersonalfaillComment");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PersonalpracticeDataSO data = (PersonalpracticeDataSO)target;
            
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("훈련 데이터 설정", headerStyle);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("기본 정보", EditorStyles.boldLabel);
            EditorGUILayout.Space(3);

            EditorGUILayout.PropertyField(practiceStatType, new GUIContent("스탯 타입"));
            EditorGUILayout.PropertyField(practiceStatName, new GUIContent("훈련 이름"));
            
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("설명", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            personalpracticeDescription.stringValue = EditorGUILayout.TextArea(
                personalpracticeDescription.stringValue, 
                GUILayout.Height(60)
            );
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("수치 설정", EditorStyles.boldLabel);
            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(staminaReduction, new GUIContent("체력 소모"));
            EditorGUILayout.LabelField($"(-{staminaReduction.floatValue})", GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(statIncrease, new GUIContent("스탯 증가량"));
            EditorGUILayout.LabelField($"(+{statIncrease.floatValue})", GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginVertical(boxStyle);
            showSuccessComment = EditorGUILayout.Foldout(showSuccessComment, "성공 코멘트", true, EditorStyles.foldoutHeader);
            if (showSuccessComment)
            {
                EditorGUI.indentLevel++;
                DrawCommentFields(personalsuccessComment);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginVertical(boxStyle);
            showFailComment = EditorGUILayout.Foldout(showFailComment, "실패 코멘트", true, EditorStyles.foldoutHeader);
            if (showFailComment)
            {
                EditorGUI.indentLevel++;
                DrawCommentFields(personalfaillComment);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("요약", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"훈련: {practiceStatName.stringValue}");
            EditorGUILayout.LabelField($"타입: {practiceStatType.enumDisplayNames[practiceStatType.enumValueIndex]}");
            EditorGUILayout.LabelField($"효과: 스탯 +{statIncrease.floatValue} / 체력 -{staminaReduction.floatValue}");
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCommentFields(SerializedProperty commentProp)
        {
            EditorGUILayout.PropertyField(commentProp.FindPropertyRelative("icon"), new GUIContent("아이콘"));
            
            EditorGUILayout.LabelField("대사", EditorStyles.boldLabel);
            var commentText = commentProp.FindPropertyRelative("comment");
            commentText.stringValue = EditorGUILayout.TextArea(commentText.stringValue, GUILayout.Height(40));

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("생각", EditorStyles.boldLabel);
            var thoughtsText = commentProp.FindPropertyRelative("thoughts");
            thoughtsText.stringValue = EditorGUILayout.TextArea(thoughtsText.stringValue, GUILayout.Height(40));
        }
    }
}
#endif