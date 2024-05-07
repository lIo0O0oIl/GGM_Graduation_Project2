using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

public class ChatEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset treeAsset = null;           // UI 파일 넣어주기

    private ChatView chatView;        // 챗팅들 들어가 있는 곳.
    private InspectorView inspectorView;        // 인스팩터 일 것임.
    private IMGUIContainer hierarchyView;      // 코드기반 GUI, 위에꺼 말하는 것. 하이어라키.
    private Button arrayAddBtn;

    private SerializedObject chatObject;        // 에디터에서 사용하기 위한 직렬화
    private SerializedProperty chatProperty;        // 위에꺼의 속성들 모음.

    private ChatContainer chatContainer;

    [MenuItem("ChatSystem/ChatEditor")]
    public static void OpenWindow()
    {
        GetWindow<ChatEditor>("ChatEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;     // 루트로 설정해줌.

        // UXML 만들어주기
        VisualElement template = treeAsset.Instantiate();
        template.style.flexGrow = 1;        // 요소 안에 넣었을 때 얼마나 커질 것 인가
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
        root.styleSheets.Add(styleSheet);       // 이 스타일을 가져와줌.

        chatView = root.Q<ChatView>("chat-view");
        inspectorView = root.Q<InspectorView>("inspector-view");        // 인스펙터 이름으로 가져오기.
        hierarchyView = root.Q<IMGUIContainer>("hierarchy-view");       // 아래꺼 가져오기 hierarchy 임.
        hierarchyView.onGUIHandler = () =>         // hierarchyView 뷰 갱신. 
        {
            if (chatObject != null && chatObject.targetObject != null)
            {
                chatObject.Update();        // 갱신해주기
                EditorGUILayout.PropertyField(chatProperty);
                chatObject.ApplyModifiedProperties();
            }
        };
        arrayAddBtn = root.Q<Button>("AddBtn");     // 버튼 가져오기
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;

        chatView.OnNodeSelected += OnSelectionNodeChanged;      // 노드를 누른 것이 달라지면 이 이벤트 호출

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("배열 추가해주기");
        }
    }

    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        Debug.Log("눌림");
        inspectorView.UpdateSelection(nodeView);        // 이 노드를 눌렀다고 인스펙터에 알려줌.
    }

    private void OnSelectionChange()        // 에디터를 킨 상태에서 무언가를 선택했을 때
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))      // 하이어라키 창에서 가져오기
            {
                chatContainer.ChangeNowChapter(0);      // 일단 0으로 가정하고
                chatView.PopulateView(chatContainer);           // 채워줘라

                Debug.Log($"{chatContainer.Chapters.Length}만큼 리스트가 생성되어야 함.");

                chatObject = new SerializedObject(chatContainer);       // 직렬화 해주기
                chatProperty = chatObject.FindProperty("hierarchy");       // 속성 찾아서 넣어주기
                //chatProperty = chatObject.FindProperty("chapters");       // 속성 찾아서 넣어주기
            }
        }
    }
}
