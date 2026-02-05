#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen.MemberData
{
    public class PracticeDataManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<PersonalpracticeDataSO> allPracticeData;
        private Dictionary<MemberType, List<PersonalpracticeDataSO>> memberPracticeDict;
        private List<PersonalpracticeDataSO> teamPracticeData;
        
        private MemberType selectedMember = MemberType.Vocal;
        private bool showTeamPractice = false;
        private string searchFilter = "";
        private PersonalpracticeDataSO editingPractice;

        [MenuItem("Tools/Practice Data Manager")]
        public static void ShowWindow()
        {
            GetWindow<PracticeDataManagerWindow>("훈련 데이터 매니저");
        }

        private void OnEnable()
        {
            LoadAllPracticeData();
        }

        private void LoadAllPracticeData()
        {
            string[] guids = AssetDatabase.FindAssets("t:PersonalpracticeDataSO");
            allPracticeData = new List<PersonalpracticeDataSO>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PersonalpracticeDataSO data = AssetDatabase.LoadAssetAtPath<PersonalpracticeDataSO>(path);
                if (data != null)
                    allPracticeData.Add(data);
            }

            OrganizeData();
        }

        private void OrganizeData()
        {
            memberPracticeDict = new Dictionary<MemberType, List<PersonalpracticeDataSO>>();
            teamPracticeData = new List<PersonalpracticeDataSO>();

            foreach (var data in allPracticeData)
            {
                if (data.name.Contains("Team") || data.name.Contains("팀"))
                {
                    teamPracticeData.Add(data);
                }
                else
                {
                    foreach (MemberType memberType in System.Enum.GetValues(typeof(MemberType)))
                    {
                        if (data.name.Contains(memberType.ToString()))
                        {
                            if (!memberPracticeDict.ContainsKey(memberType))
                                memberPracticeDict[memberType] = new List<PersonalpracticeDataSO>();
                            
                            memberPracticeDict[memberType].Add(data);
                            break;
                        }
                    }
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("훈련 데이터 매니저", headerStyle);
            EditorGUILayout.Space(5);

            if (editingPractice != null)
            {
                DrawEditMode();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            searchFilter = EditorGUILayout.TextField("검색", searchFilter);
            if (GUILayout.Button("새로고침", GUILayout.Width(80)))
            {
                LoadAllPracticeData();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(!showTeamPractice, "개인 훈련", "Button"))
                showTeamPractice = false;
            if (GUILayout.Toggle(showTeamPractice, "팀 훈련", "Button"))
                showTeamPractice = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (!showTeamPractice)
            {
                DrawPersonalPractice();
            }
            else
            {
                DrawTeamPractice();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("통계", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"전체 훈련 데이터: {allPracticeData.Count}개");
            EditorGUILayout.LabelField($"개인 훈련: {allPracticeData.Count - teamPracticeData.Count}개");
            EditorGUILayout.LabelField($"팀 훈련: {teamPracticeData.Count}개");
            EditorGUILayout.EndVertical();
        }

        private void DrawEditMode()
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField($"편집 중: {editingPractice.name}", titleStyle);
            if (GUILayout.Button("돌아가기", GUILayout.Width(80)))
            {
                editingPractice = null;
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            SerializedObject so = new SerializedObject(editingPractice);
            so.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("기본 정보", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("PracticeStatType"), new GUIContent("스탯 타입"));
            EditorGUILayout.PropertyField(so.FindProperty("PracticeStatName"), new GUIContent("훈련 이름"));
            EditorGUILayout.LabelField("설명");
            var desc = so.FindProperty("PersonalpracticeDescription");
            desc.stringValue = EditorGUILayout.TextArea(desc.stringValue, GUILayout.Height(60));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("수치 설정", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("StaminaReduction"), new GUIContent("체력 소모"));
            EditorGUILayout.PropertyField(so.FindProperty("statIncrease"), new GUIContent("스탯 증가량"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("성공 코멘트", EditorStyles.boldLabel);
            DrawCommentProperty(so.FindProperty("PersonalsuccessComment"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("실패 코멘트", EditorStyles.boldLabel);
            DrawCommentProperty(so.FindProperty("PersonalfaillComment"));
            EditorGUILayout.EndVertical();

            so.ApplyModifiedProperties();
            
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("저장", GUILayout.Height(30)))
            {
                EditorUtility.SetDirty(editingPractice);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("저장 완료", "변경사항이 저장되었습니다.", "확인");
            }
        }

        private void DrawCommentProperty(SerializedProperty commentProp)
        {
            EditorGUILayout.PropertyField(commentProp.FindPropertyRelative("icon"), new GUIContent("아이콘"));
            EditorGUILayout.LabelField("대사");
            var comment = commentProp.FindPropertyRelative("comment");
            comment.stringValue = EditorGUILayout.TextArea(comment.stringValue, GUILayout.Height(40));
            EditorGUILayout.LabelField("생각");
            var thoughts = commentProp.FindPropertyRelative("thoughts");
            thoughts.stringValue = EditorGUILayout.TextArea(thoughts.stringValue, GUILayout.Height(40));
        }

        private void DrawPersonalPractice()
        {
            selectedMember = (MemberType)EditorGUILayout.EnumPopup("멤버 선택", selectedMember);
            EditorGUILayout.Space(5);

            if (!memberPracticeDict.ContainsKey(selectedMember))
            {
                EditorGUILayout.HelpBox($"{selectedMember}의 훈련 데이터가 없습니다.", MessageType.Info);
                return;
            }

            var practices = memberPracticeDict[selectedMember];
            var filteredPractices = string.IsNullOrEmpty(searchFilter) 
                ? practices 
                : practices.Where(p => p.name.ToLower().Contains(searchFilter.ToLower())).ToList();

            EditorGUILayout.LabelField($"{selectedMember} 훈련 목록 ({filteredPractices.Count}개)", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            foreach (var practice in filteredPractices)
            {
                DrawPracticeCard(practice);
            }
        }

        private void DrawTeamPractice()
        {
            var filteredPractices = string.IsNullOrEmpty(searchFilter)
                ? teamPracticeData
                : teamPracticeData.Where(p => p.name.ToLower().Contains(searchFilter.ToLower())).ToList();

            EditorGUILayout.LabelField($"팀 훈련 목록 ({filteredPractices.Count}개)", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            foreach (var practice in filteredPractices)
            {
                DrawPracticeCard(practice);
            }
        }

        private void DrawPracticeCard(PersonalpracticeDataSO practice)
        {
            if (practice == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(practice.PracticeStatName, EditorStyles.boldLabel, GUILayout.Width(150));
            EditorGUILayout.LabelField($"[{practice.PracticeStatType}]", GUILayout.Width(80));
            
            if (GUILayout.Button("선택", GUILayout.Width(50)))
            {
                Selection.activeObject = practice;
                EditorGUIUtility.PingObject(practice);
            }
            
            if (GUILayout.Button("편집", GUILayout.Width(50)))
            {
                editingPractice = practice;
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField($"파일명: {practice.name}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"체력: -{practice.StaminaReduction} | 스탯: +{practice.statIncrease}", EditorStyles.miniLabel);
            
            if (!string.IsNullOrEmpty(practice.PersonalpracticeDescription))
            {
                EditorGUILayout.LabelField(practice.PersonalpracticeDescription, EditorStyles.wordWrappedMiniLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(3);
        }
    }
}
#endif