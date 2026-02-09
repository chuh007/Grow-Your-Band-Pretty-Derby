using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Code.MainSystem.Dialogue;
using Member.LS.Code.Dialogue;
using Member.LS.Code.Dialogue.Character;
using Code.MainSystem.Dialogue.DialogueEvent;

namespace Code.Editor.Dialogue
{
    /// <summary>
    /// DialogueInformationSO 전용 에디터 윈도우
    /// 다이얼로그 파일 관리 + 노드 생성/삭제, 연결, 작성을 쉽게 할 수 있음
    /// </summary>
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueInformationSO _currentDialogue;
        private SerializedObject _serializedDialogue;
        
        private int _selectedNodeIndex = -1;
        private Vector2 _nodeListScroll;
        private Vector2 _nodeEditScroll;
        private Vector2 _backgroundScroll;
        private Vector2 _dialogueManagerScroll;
        
        private bool _showBackgrounds = true;
        private bool _showNodeList = true;
        private bool _showNodeEdit = true;
        
        private const float LEFT_PANEL_WIDTH = 250f;
        private const float SPACING = 10f;
        private const float PREVIEW_SIZE = 80f;
        private const float MANAGER_ITEM_HEIGHT = 70f;
        
        // 관리자 모드
        private enum EditorMode { Manager, Editor }
        private EditorMode _currentMode = EditorMode.Manager;
        private List<DialogueInformationSO> _allDialogues = new List<DialogueInformationSO>();
        private string _searchFilter = "";
        private bool _needsRefresh = true;

        [MenuItem("Tools/Dialogue Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<DialogueEditorWindow>("Dialogue Editor");
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            _needsRefresh = true;
        }

        private void OnGUI()
        {
            DrawModeToggle();
            
            if (_currentMode == EditorMode.Manager)
            {
                DrawDialogueManager();
            }
            else
            {
                DrawDialogueEditor();
            }
        }

        /// <summary>
        /// 모드 전환 버튼
        /// </summary>
        private void DrawModeToggle()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUIStyle buttonStyle = EditorStyles.toolbarButton;
                
                if (GUILayout.Toggle(_currentMode == EditorMode.Manager, "📋 다이얼로그 관리", buttonStyle))
                {
                    if (_currentMode != EditorMode.Manager)
                    {
                        _currentMode = EditorMode.Manager;
                        _needsRefresh = true;
                    }
                }
                
                GUI.enabled = _currentDialogue != null;
                if (GUILayout.Toggle(_currentMode == EditorMode.Editor, "✏️ 에디터", buttonStyle))
                {
                    if (_currentMode != EditorMode.Editor && _currentDialogue != null)
                    {
                        _currentMode = EditorMode.Editor;
                    }
                }
                GUI.enabled = true;
                
                GUILayout.FlexibleSpace();
                
                if (_currentMode == EditorMode.Manager)
                {
                    if (GUILayout.Button("🔄", EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        _needsRefresh = true;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        #region Dialogue Manager Mode

        /// <summary>
        /// 다이얼로그 관리자 화면
        /// </summary>
        private void DrawDialogueManager()
        {
            if (_needsRefresh)
            {
                RefreshDialogueList();
                _needsRefresh = false;
            }
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                DrawManagerToolbar();
                GUILayout.Space(5);
                DrawDialogueGrid();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 관리자 툴바 (검색, 새로만들기 등)
        /// </summary>
        private void DrawManagerToolbar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField($"📂 다이얼로그 파일 ({_allDialogues.Count}개)", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("🔍", GUILayout.Width(20));
                    _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
                    
                    if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
                    {
                        _searchFilter = "";
                        GUI.FocusControl(null);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("➕ 새 다이얼로그 생성", GUILayout.Height(30)))
                    {
                        CreateNewDialogueInManager();
                    }
                    
                    if (GUILayout.Button("📁 폴더 열기", GUILayout.Height(30), GUILayout.Width(100)))
                    {
                        string path = "Assets";
                        if (_allDialogues.Count > 0 && _allDialogues[0] != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(_allDialogues[0]);
                            path = Path.GetDirectoryName(assetPath);
                        }
                        EditorUtility.RevealInFinder(path);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 다이얼로그 그리드 표시
        /// </summary>
        private void DrawDialogueGrid()
        {
            var filteredDialogues = _allDialogues
                .Where(d => d != null && (string.IsNullOrEmpty(_searchFilter) || 
                       d.name.ToLower().Contains(_searchFilter.ToLower())))
                .ToList();
            
            if (filteredDialogues.Count == 0)
            {
                DrawEmptyState();
                return;
            }
            
            _dialogueManagerScroll = EditorGUILayout.BeginScrollView(_dialogueManagerScroll);
            {
                foreach (var dialogue in filteredDialogues)
                {
                    DrawDialogueItem(dialogue);
                    GUILayout.Space(5);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 빈 상태 표시
        /// </summary>
        private void DrawEmptyState()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.BeginVertical();
            {
                if (string.IsNullOrEmpty(_searchFilter))
                {
                    GUILayout.Label("📭 다이얼로그 파일이 없습니다", EditorStyles.boldLabel);
                    GUILayout.Space(10);
                    if (GUILayout.Button("새 다이얼로그 만들기", GUILayout.Width(200), GUILayout.Height(30)))
                    {
                        CreateNewDialogueInManager();
                    }
                }
                else
                {
                    GUILayout.Label($"🔍 '{_searchFilter}'에 대한 결과가 없습니다", EditorStyles.boldLabel);
                    GUILayout.Space(10);
                    if (GUILayout.Button("검색 초기화", GUILayout.Width(200)))
                    {
                        _searchFilter = "";
                    }
                }
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// 개별 다이얼로그 아이템 표시
        /// </summary>
        private void DrawDialogueItem(DialogueInformationSO dialogue)
        {
            bool isSelected = _currentDialogue == dialogue;
            Color bgColor = isSelected ? new Color(0.3f, 0.6f, 1f, 0.3f) : new Color(0.25f, 0.25f, 0.25f, 1f);
            
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(MANAGER_ITEM_HEIGHT));
            EditorGUI.DrawRect(rect, bgColor);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    // 왼쪽: 정보
                    EditorGUILayout.BeginVertical(GUILayout.Width(position.width - 200));
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("📄", GUILayout.Width(20));
                            EditorGUILayout.LabelField(dialogue.name, EditorStyles.boldLabel);
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        string assetPath = AssetDatabase.GetAssetPath(dialogue);
                        EditorGUILayout.LabelField($"📁 {assetPath}", EditorStyles.miniLabel);
                        
                        // 통계 정보
                        int nodeCount = 0;
                        int backgroundCount = 0;
                        
                        var so = new SerializedObject(dialogue);
                        var nodesProp = so.FindProperty("<DialogueDetails>k__BackingField");
                        if (nodesProp == null) nodesProp = so.FindProperty("DialogueDetails");
                        if (nodesProp != null) nodeCount = nodesProp.arraySize;
                        
                        var bgProp = so.FindProperty("<DialogueBackground>k__BackingField");
                        if (bgProp == null) bgProp = so.FindProperty("DialogueBackground");
                        if (bgProp != null) backgroundCount = bgProp.arraySize;
                        
                        EditorGUILayout.LabelField($"💬 노드: {nodeCount}개 | 🖼️ 배경: {backgroundCount}개", EditorStyles.miniLabel);
                    }
                    EditorGUILayout.EndVertical();
                    
                    // 오른쪽: 버튼
                    EditorGUILayout.BeginVertical(GUILayout.Width(180));
                    {
                        if (GUILayout.Button("✏️ 편집", GUILayout.Height(25)))
                        {
                            OpenDialogueForEditing(dialogue);
                        }
                        
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("📋 복제", GUILayout.Height(20)))
                            {
                                DuplicateDialogue(dialogue);
                            }
                            
                            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                            if (GUILayout.Button("🗑️ 삭제", GUILayout.Height(20)))
                            {
                                DeleteDialogue(dialogue);
                            }
                            GUI.backgroundColor = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 프로젝트의 모든 다이얼로그 파일 찾기
        /// </summary>
        private void RefreshDialogueList()
        {
            _allDialogues.Clear();
            
            string[] guids = AssetDatabase.FindAssets("t:DialogueInformationSO");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DialogueInformationSO dialogue = AssetDatabase.LoadAssetAtPath<DialogueInformationSO>(path);
                if (dialogue != null)
                {
                    _allDialogues.Add(dialogue);
                }
            }
            
            _allDialogues = _allDialogues.OrderBy(d => d.name).ToList();
        }

        /// <summary>
        /// 관리자에서 새 다이얼로그 생성
        /// </summary>
        private void CreateNewDialogueInManager()
        {
            string defaultPath = "Assets";
            if (_allDialogues.Count > 0 && _allDialogues[0] != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(_allDialogues[0]);
                defaultPath = Path.GetDirectoryName(assetPath);
            }
            
            string path = EditorUtility.SaveFilePanelInProject(
                "새 다이얼로그 생성",
                "NewDialogue",
                "asset",
                "다이얼로그 이름을 입력하세요",
                defaultPath);
            
            if (!string.IsNullOrEmpty(path))
            {
                var newDialogue = CreateInstance<DialogueInformationSO>();
                AssetDatabase.CreateAsset(newDialogue, path);
                AssetDatabase.SaveAssets();
                
                _needsRefresh = true;
                OpenDialogueForEditing(newDialogue);
                
                ShowNotification(new GUIContent($"✅ '{Path.GetFileNameWithoutExtension(path)}' 생성됨!"));
            }
        }

        /// <summary>
        /// 다이얼로그 복제
        /// </summary>
        private void DuplicateDialogue(DialogueInformationSO source)
        {
            string sourcePath = AssetDatabase.GetAssetPath(source);
            string directory = Path.GetDirectoryName(sourcePath);
            string fileName = Path.GetFileNameWithoutExtension(sourcePath);
            
            string newPath = EditorUtility.SaveFilePanelInProject(
                "다이얼로그 복제",
                fileName + "_Copy",
                "asset",
                "복제할 파일 이름을 입력하세요",
                directory);
            
            if (!string.IsNullOrEmpty(newPath))
            {
                if (AssetDatabase.CopyAsset(sourcePath, newPath))
                {
                    AssetDatabase.SaveAssets();
                    _needsRefresh = true;
                    ShowNotification(new GUIContent($"✅ '{Path.GetFileNameWithoutExtension(newPath)}' 복제됨!"));
                }
                else
                {
                    EditorUtility.DisplayDialog("오류", "다이얼로그 복제에 실패했습니다.", "확인");
                }
            }
        }

        /// <summary>
        /// 다이얼로그 삭제
        /// </summary>
        private void DeleteDialogue(DialogueInformationSO dialogue)
        {
            if (EditorUtility.DisplayDialog(
                "다이얼로그 삭제",
                $"'{dialogue.name}'을(를) 정말 삭제하시겠습니까?\n\n이 작업은 되돌릴 수 없습니다.",
                "삭제",
                "취소"))
            {
                string path = AssetDatabase.GetAssetPath(dialogue);
                
                if (_currentDialogue == dialogue)
                {
                    _currentDialogue = null;
                    _serializedDialogue = null;
                }
                
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
                
                _needsRefresh = true;
                ShowNotification(new GUIContent($"🗑️ '{dialogue.name}' 삭제됨"));
            }
        }

        /// <summary>
        /// 다이얼로그를 편집 모드로 열기
        /// </summary>
        private void OpenDialogueForEditing(DialogueInformationSO dialogue)
        {
            _currentDialogue = dialogue;
            _serializedDialogue = new SerializedObject(_currentDialogue);
            _selectedNodeIndex = -1;
            _currentMode = EditorMode.Editor;
        }

        #endregion

        #region Dialogue Editor Mode

        /// <summary>
        /// 다이얼로그 에디터 화면
        /// </summary>
        private void DrawDialogueEditor()
        {
            DrawEditorToolbar();
            
            if (_currentDialogue == null)
            {
                DrawNoDialogueSelected();
                return;
            }
            
            if (_serializedDialogue == null || _serializedDialogue.targetObject == null)
            {
                _serializedDialogue = new SerializedObject(_currentDialogue);
            }
            
            _serializedDialogue.Update();
            
            EditorGUILayout.BeginHorizontal();
            {
                DrawLeftPanel();
                DrawRightPanel();
            }
            EditorGUILayout.EndHorizontal();
            
            _serializedDialogue.ApplyModifiedProperties();
        }

        /// <summary>
        /// 에디터 모드 툴바
        /// </summary>
        private void DrawEditorToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.Label("파일:", GUILayout.Width(35));
                
                DialogueInformationSO newDialogue = (DialogueInformationSO)EditorGUILayout.ObjectField(
                    _currentDialogue, typeof(DialogueInformationSO), false, GUILayout.Width(200));
                
                if (newDialogue != _currentDialogue)
                {
                    _currentDialogue = newDialogue;
                    _selectedNodeIndex = -1;
                    if (_currentDialogue != null)
                    {
                        _serializedDialogue = new SerializedObject(_currentDialogue);
                    }
                }
                
                GUILayout.FlexibleSpace();
                
                if (_currentDialogue != null)
                {
                    if (GUILayout.Button("💾 저장", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        EditorUtility.SetDirty(_currentDialogue);
                        AssetDatabase.SaveAssets();
                        ShowNotification(new GUIContent("✅ 저장됨!"));
                    }
                    
                    if (GUILayout.Button("📁", EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        EditorGUIUtility.PingObject(_currentDialogue);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 파일이 선택되지 않았을 때 표시
        /// </summary>
        private void DrawNoDialogueSelected()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUILayout.BeginVertical();
            GUILayout.Label("📭 다이얼로그가 선택되지 않았습니다", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("툴바에서 파일을 선택하거나 관리자 모드로 돌아가세요");
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("📋 관리자로 돌아가기", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    _currentMode = EditorMode.Manager;
                    _needsRefresh = true;
                }
                
                if (GUILayout.Button("➕ 새로 만들기", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    CreateNewDialogue();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// 새 DialogueInformationSO 생성
        /// </summary>
        private void CreateNewDialogue()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "새 다이얼로그 생성",
                "NewDialogue",
                "asset",
                "다이얼로그 이름을 입력하세요");
            
            if (!string.IsNullOrEmpty(path))
            {
                var newDialogue = CreateInstance<DialogueInformationSO>();
                AssetDatabase.CreateAsset(newDialogue, path);
                AssetDatabase.SaveAssets();
                
                _currentDialogue = newDialogue;
                _serializedDialogue = new SerializedObject(_currentDialogue);
                _selectedNodeIndex = -1;
                _needsRefresh = true;
            }
        }

        /// <summary>
        /// 왼쪽 패널 (배경 리스트 + 노드 리스트)
        /// </summary>
        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(LEFT_PANEL_WIDTH));
            {
                DrawBackgroundSection();
                GUILayout.Space(SPACING);
                DrawNodeListSection();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 배경 이미지 섹션
        /// </summary>
        private void DrawBackgroundSection()
        {
            _showBackgrounds = EditorGUILayout.Foldout(_showBackgrounds, "🖼️ 배경 이미지", true, EditorStyles.foldoutHeader);
            
            if (!_showBackgrounds) return;
            
            var bgProp = _serializedDialogue.FindProperty("<DialogueBackground>k__BackingField");
            if (bgProp == null)
            {
                bgProp = _serializedDialogue.FindProperty("DialogueBackground");
            }
            
            if (bgProp == null)
            {
                EditorGUILayout.HelpBox("배경 프로퍼티를 찾을 수 없습니다.", MessageType.Error);
                return;
            }
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                _backgroundScroll = EditorGUILayout.BeginScrollView(_backgroundScroll, GUILayout.MaxHeight(200));
                {
                    for (int i = 0; i < bgProp.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            var element = bgProp.GetArrayElementAtIndex(i);
                            EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(25));
                            EditorGUILayout.PropertyField(element, GUIContent.none);
                            
                            if (GUILayout.Button("×", GUILayout.Width(20)))
                            {
                                bgProp.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
                
                if (GUILayout.Button("➕ 배경 추가"))
                {
                    bgProp.arraySize++;
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 노드 리스트 섹션
        /// </summary>
        private void DrawNodeListSection()
        {
            var nodesProp = _serializedDialogue.FindProperty("<DialogueDetails>k__BackingField");
            if (nodesProp == null)
            {
                nodesProp = _serializedDialogue.FindProperty("DialogueDetails");
            }
            
            if (nodesProp == null)
            {
                EditorGUILayout.HelpBox("노드 프로퍼티를 찾을 수 없습니다.", MessageType.Error);
                return;
            }
            
            _showNodeList = EditorGUILayout.Foldout(_showNodeList, $"💬 노드 목록 ({nodesProp.arraySize})", true, EditorStyles.foldoutHeader);
            
            if (!_showNodeList) return;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("➕ 새 노드"))
                    {
                        AddNewNode(nodesProp);
                    }
                    
                    GUI.enabled = _selectedNodeIndex >= 0 && _selectedNodeIndex < nodesProp.arraySize;
                    if (GUILayout.Button("🗑️ 삭제"))
                    {
                        DeleteNode(nodesProp, _selectedNodeIndex);
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5);
                
                _nodeListScroll = EditorGUILayout.BeginScrollView(_nodeListScroll);
                {
                    for (int i = 0; i < nodesProp.arraySize; i++)
                    {
                        DrawNodeListItem(nodesProp, i);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 노드 리스트 개별 아이템
        /// </summary>
        private void DrawNodeListItem(SerializedProperty nodesProp, int index)
        {
            var node = nodesProp.GetArrayElementAtIndex(index);
            var dialogueProp = node.FindPropertyRelative("<DialogueDetail>k__BackingField");
            if (dialogueProp == null) dialogueProp = node.FindPropertyRelative("DialogueDetail");
            
            var characterProp = node.FindPropertyRelative("<CharacterInformSO>k__BackingField");
            if (characterProp == null) characterProp = node.FindPropertyRelative("CharacterInformSO");
            
            string preview = dialogueProp != null ? dialogueProp.stringValue : "";
            if (preview.Length > 20) preview = preview.Substring(0, 20) + "...";
            
            CharacterInformationSO character = characterProp?.objectReferenceValue as CharacterInformationSO;
            string characterName = character != null ? character.CharacterName : "없음";
            
            bool isSelected = _selectedNodeIndex == index;
            Color bgColor = isSelected ? new Color(0.3f, 0.5f, 0.8f, 0.5f) : Color.clear;
            
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (isSelected)
            {
                EditorGUI.DrawRect(rect, bgColor);
            }
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"[{index}]", EditorStyles.boldLabel, GUILayout.Width(30));
                    GUILayout.Label(characterName, GUILayout.Width(80));
                }
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Label(preview, EditorStyles.wordWrappedMiniLabel);
                
                if (GUILayout.Button("✏️ 편집", GUILayout.Height(20)))
                {
                    _selectedNodeIndex = index;
                }
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(2);
        }

        /// <summary>
        /// 오른쪽 패널 (선택된 노드 편집)
        /// </summary>
        private void DrawRightPanel()
        {
            EditorGUILayout.BeginVertical();
            {
                var nodesProp = _serializedDialogue.FindProperty("<DialogueDetails>k__BackingField");
                if (nodesProp == null) nodesProp = _serializedDialogue.FindProperty("DialogueDetails");
                
                if (nodesProp == null)
                {
                    EditorGUILayout.HelpBox("노드 프로퍼티를 찾을 수 없습니다.", MessageType.Error);
                    return;
                }
                
                if (_selectedNodeIndex < 0 || _selectedNodeIndex >= nodesProp.arraySize)
                {
                    DrawNoNodeSelected();
                }
                else
                {
                    DrawNodeEditor(nodesProp, _selectedNodeIndex);
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 노드가 선택되지 않았을 때
        /// </summary>
        private void DrawNoNodeSelected()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("노드를 선택하거나 새로 만드세요", EditorStyles.centeredGreyMiniLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// 선택된 노드 편집 UI
        /// </summary>
        private void DrawNodeEditor(SerializedProperty nodesProp, int index)
        {
            var node = nodesProp.GetArrayElementAtIndex(index);
            
            EditorGUILayout.LabelField($"✏️ 노드 [{index}] 편집", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            _nodeEditScroll = EditorGUILayout.BeginScrollView(_nodeEditScroll);
            {
                DrawNodeBasicInfo(node);
                GUILayout.Space(10);
                DrawNodeDialogue(node);
                GUILayout.Space(10);
                DrawNodeEvents(node);
                GUILayout.Space(10);
                DrawNodeChoices(node, index);
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 노드 기본 정보 (캐릭터, 감정, 배경, 네임태그)
        /// </summary>
        private void DrawNodeBasicInfo(SerializedProperty node)
        {
            EditorGUILayout.LabelField("📌 기본 정보", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var characterProp = node.FindPropertyRelative("<CharacterInformSO>k__BackingField");
                if (characterProp == null) characterProp = node.FindPropertyRelative("CharacterInformSO");
                
                var emotionProp = node.FindPropertyRelative("<CharacterEmotion>k__BackingField");
                if (emotionProp == null) emotionProp = node.FindPropertyRelative("CharacterEmotion");
                
                var nameTagProp = node.FindPropertyRelative("<NameTagPosition>k__BackingField");
                if (nameTagProp == null) nameTagProp = node.FindPropertyRelative("NameTagPosition");
                
                var bgIndexProp = node.FindPropertyRelative("<BackgroundIndex>k__BackingField");
                if (bgIndexProp == null) bgIndexProp = node.FindPropertyRelative("BackgroundIndex");
                
                EditorGUILayout.PropertyField(characterProp, new GUIContent("캐릭터"));
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(emotionProp, new GUIContent("감정"), GUILayout.Width(position.width * 0.5f));
                    
                    // 캐릭터 미리보기
                    CharacterInformationSO character = characterProp?.objectReferenceValue as CharacterInformationSO;
                    if (character != null && emotionProp != null)
                    {
                        CharacterEmotionType emotion = (CharacterEmotionType)emotionProp.enumValueIndex;
                        if (character.CharacterEmotions != null && character.CharacterEmotions.TryGetValue(emotion, out Sprite sprite))
                        {
                            if (sprite != null)
                            {
                                Rect previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE);
                                GUI.Box(previewRect, "");
                                GUI.DrawTexture(previewRect, sprite.texture, ScaleMode.ScaleToFit);
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.PropertyField(nameTagProp, new GUIContent("네임태그 위치"));
                
                // 배경 선택 (드롭다운)
                DrawBackgroundSelector(bgIndexProp);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 배경 선택 드롭다운
        /// </summary>
        private void DrawBackgroundSelector(SerializedProperty bgIndexProp)
        {
            var bgProp = _serializedDialogue.FindProperty("<DialogueBackground>k__BackingField");
            if (bgProp == null) bgProp = _serializedDialogue.FindProperty("DialogueBackground");
            
            if (bgProp == null || bgProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("⚠ 먼저 배경 이미지를 추가하세요!", MessageType.Warning);
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("배경", GUILayout.Width(EditorGUIUtility.labelWidth));
                
                int currentBg = bgIndexProp.intValue;
                if (currentBg < 0 || currentBg >= bgProp.arraySize)
                {
                    currentBg = 0;
                    bgIndexProp.intValue = 0;
                }
                
                string[] bgOptions = new string[bgProp.arraySize];
                for (int i = 0; i < bgProp.arraySize; i++)
                {
                    var bg = bgProp.GetArrayElementAtIndex(i).objectReferenceValue;
                    bgOptions[i] = bg != null ? $"[{i}] {bg.name}" : $"[{i}] 없음";
                }
                
                int newBg = EditorGUILayout.Popup(currentBg, bgOptions);
                if (newBg != currentBg)
                {
                    bgIndexProp.intValue = newBg;
                }
                
                // 배경 미리보기
                var bgElement = bgProp.GetArrayElementAtIndex(currentBg);
                if (bgElement.objectReferenceValue != null)
                {
                    Sprite sprite = bgElement.objectReferenceValue as Sprite;
                    if (sprite != null)
                    {
                        Rect previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE);
                        GUI.Box(previewRect, "");
                        GUI.DrawTexture(previewRect, sprite.texture, ScaleMode.ScaleToFit);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 대사 입력
        /// </summary>
        private void DrawNodeDialogue(SerializedProperty node)
        {
            EditorGUILayout.LabelField("💬 대사", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var dialogueProp = node.FindPropertyRelative("<DialogueDetail>k__BackingField");
                if (dialogueProp == null) dialogueProp = node.FindPropertyRelative("DialogueDetail");
                
                if (dialogueProp != null)
                {
                    dialogueProp.stringValue = EditorGUILayout.TextArea(dialogueProp.stringValue, GUILayout.Height(80));
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 이벤트 리스트
        /// </summary>
        private void DrawNodeEvents(SerializedProperty node)
        {
            var eventsProp = node.FindPropertyRelative("<Events>k__BackingField");
            if (eventsProp == null) eventsProp = node.FindPropertyRelative("Events");
            
            if (eventsProp == null) return;
            
            EditorGUILayout.LabelField($"⚡ 이벤트 ({eventsProp.arraySize})", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                for (int i = 0; i < eventsProp.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var element = eventsProp.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(element, new GUIContent($"[{i}]"));
                        
                        if (GUILayout.Button("×", GUILayout.Width(20)))
                        {
                            eventsProp.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                if (GUILayout.Button("➕ 이벤트 추가"))
                {
                    eventsProp.arraySize++;
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 선택지 편집
        /// </summary>
        private void DrawNodeChoices(SerializedProperty node, int currentNodeIndex)
        {
            var choicesProp = node.FindPropertyRelative("<Choices>k__BackingField");
            if (choicesProp == null) choicesProp = node.FindPropertyRelative("Choices");
            
            if (choicesProp == null) return;
            
            EditorGUILayout.LabelField($"🔀 선택지 ({choicesProp.arraySize})", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                for (int i = 0; i < choicesProp.arraySize; i++)
                {
                    DrawChoice(choicesProp.GetArrayElementAtIndex(i), i, currentNodeIndex);
                    GUILayout.Space(5);
                }
                
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("➕ 선택지 추가"))
                    {
                        choicesProp.arraySize++;
                        var newChoice = choicesProp.GetArrayElementAtIndex(choicesProp.arraySize - 1);
                        
                        var textProp = newChoice.FindPropertyRelative("<ChoiceText>k__BackingField");
                        if (textProp == null) textProp = newChoice.FindPropertyRelative("ChoiceText");
                        if (textProp != null) textProp.stringValue = "새 선택지";
                        
                        var nextProp = newChoice.FindPropertyRelative("<NextNodeIndex>k__BackingField");
                        if (nextProp == null) nextProp = newChoice.FindPropertyRelative("NextNodeIndex");
                        if (nextProp != null) nextProp.intValue = -1;
                    }
                    
                    if (choicesProp.arraySize > 0 && GUILayout.Button("➖ 마지막 제거"))
                    {
                        choicesProp.arraySize--;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 개별 선택지 편집
        /// </summary>
        private void DrawChoice(SerializedProperty choice, int choiceIndex, int currentNodeIndex)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var textProp = choice.FindPropertyRelative("<ChoiceText>k__BackingField");
                if (textProp == null) textProp = choice.FindPropertyRelative("ChoiceText");
                
                var nextNodeProp = choice.FindPropertyRelative("<NextNodeIndex>k__BackingField");
                if (nextNodeProp == null) nextNodeProp = choice.FindPropertyRelative("NextNodeIndex");
                
                var eventsProp = choice.FindPropertyRelative("<Events>k__BackingField");
                if (eventsProp == null) eventsProp = choice.FindPropertyRelative("Events");
                
                EditorGUILayout.LabelField($"선택지 {choiceIndex + 1}", EditorStyles.miniLabel);
                
                if (textProp != null)
                {
                    EditorGUILayout.PropertyField(textProp, new GUIContent("텍스트"));
                }
                
                // 다음 노드 선택
                if (nextNodeProp != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(nextNodeProp, new GUIContent("다음 노드"));
                        
                        int nextNode = nextNodeProp.intValue;
                        var nodesProp = _serializedDialogue.FindProperty("<DialogueDetails>k__BackingField");
                        if (nodesProp == null) nodesProp = _serializedDialogue.FindProperty("DialogueDetails");
                        
                        if (nextNode == -1)
                        {
                            GUILayout.Label($"→ [{currentNodeIndex + 1}]", GUILayout.Width(60));
                        }
                        else if (nodesProp != null && nextNode >= 0 && nextNode < nodesProp.arraySize)
                        {
                            GUILayout.Label($"→ [{nextNode}] ✓", GUILayout.Width(60));
                        }
                        else
                        {
                            GUILayout.Label("⚠ 오류", GUILayout.Width(60));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                // 선택지 이벤트
                if (eventsProp != null)
                {
                    EditorGUILayout.PropertyField(eventsProp, new GUIContent($"이벤트 ({eventsProp.arraySize})"), false);
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 새 노드 추가
        /// </summary>
        private void AddNewNode(SerializedProperty nodesProp)
        {
            int newIndex = nodesProp.arraySize;
            nodesProp.arraySize++;
            
            var newNode = nodesProp.GetArrayElementAtIndex(newIndex);
            
            // 프로퍼티 초기화
            var charProp = newNode.FindPropertyRelative("<CharacterInformSO>k__BackingField");
            if (charProp == null) charProp = newNode.FindPropertyRelative("CharacterInformSO");
            if (charProp != null) charProp.objectReferenceValue = null;
            
            var dialogueProp = newNode.FindPropertyRelative("<DialogueDetail>k__BackingField");
            if (dialogueProp == null) dialogueProp = newNode.FindPropertyRelative("DialogueDetail");
            if (dialogueProp != null) dialogueProp.stringValue = "";
            
            var nameTagProp = newNode.FindPropertyRelative("<NameTagPosition>k__BackingField");
            if (nameTagProp == null) nameTagProp = newNode.FindPropertyRelative("NameTagPosition");
            if (nameTagProp != null) nameTagProp.enumValueIndex = 0;
            
            var bgProp = newNode.FindPropertyRelative("<BackgroundIndex>k__BackingField");
            if (bgProp == null) bgProp = newNode.FindPropertyRelative("BackgroundIndex");
            if (bgProp != null) bgProp.intValue = 0;
            
            var emotionProp = newNode.FindPropertyRelative("<CharacterEmotion>k__BackingField");
            if (emotionProp == null) emotionProp = newNode.FindPropertyRelative("CharacterEmotion");
            if (emotionProp != null) emotionProp.enumValueIndex = 0;
            
            _selectedNodeIndex = newIndex;
            _serializedDialogue.ApplyModifiedProperties();
        }

        /// <summary>
        /// 노드 삭제
        /// </summary>
        private void DeleteNode(SerializedProperty nodesProp, int index)
        {
            if (index < 0 || index >= nodesProp.arraySize) return;
            
            if (EditorUtility.DisplayDialog("노드 삭제", $"노드 [{index}]를 삭제하시겠습니까?", "삭제", "취소"))
            {
                nodesProp.DeleteArrayElementAtIndex(index);
                _selectedNodeIndex = -1;
                _serializedDialogue.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}