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
    private HierarchyView hierarchyView;      // 코드기반 GUI, 위에꺼 말하는 것. 하이어라키.
    private Button arrayAddBtn;
    private Button dangerBtn;

    private ChatContainer chatContainer;

    [MenuItem("ChatSystem/ChatEditor")]
    public static void OpenWindow()
    {
        GetWindow<ChatEditor>("ChatEditor");
    }

    private void OnDestroy()
    {
        //if (chatContainer != null)
        {
            chatView.SaveChatSystem();      // 창을 끌 때 지금까지 해준 것 저장해주기
        }
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
        hierarchyView = root.Q<HierarchyView>("hierarchy-view");       // 아래꺼 가져오기 hierarchy 임.
       
        arrayAddBtn = root.Q<Button>("AddBtn");     // 버튼 가져오기
        arrayAddBtn.tooltip = "Chapter Add";
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;
        dangerBtn = root.Q<Button>("ClearBtn");
        dangerBtn.tooltip = "All Nodes Delete";
        dangerBtn.clickable.clicked += OnClearNodes;

        chatView.OnNodeSelected += OnSelectionNodeChanged;      // 노드를 누른 것이 달라지면 이 이벤트 호출

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("배열 추가해주기");
            chatContainer.MainChapter.Add(new Chapter());
            hierarchyView.UpdateHierarchy(chatContainer, chatView);
        }
    }

    private void OnClearNodes()
    {
        if (chatContainer != null)
        {
            /*  foreach (var node in chatContainer.nodes)
              {
                  if (node is AskNode)
                  {
                      chatContainer.nodes.Remove(node);
                  }
              }*/
            Debug.Log("노드의 개수는 " + chatContainer.nodes.Count + "개 입니다.");
            Close();
        }
    }

    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        inspectorView.UpdateInspector(nodeView);        // 이 노드를 눌렀다고 인스펙터에 알려줌.
    }

    private void OnSelectionChange()        // 에디터를 킨 상태에서 무언가를 선택했을 때
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))      // 하이어라키 창에서 가져오기
            {
                //Debug.Log(chatContainer.nodes.Count + "개의 노드가 존재함.");

                chatContainer.ChangeNowChapter(0);      // 가장 처음은 0번째

                hierarchyView.UpdateHierarchy(chatContainer, chatView);      // 하이어라키 만들어주기
                
                chatView.LoadChatSystem(chatContainer);     // 데이터 로드 해주기
                chatView.PopulateView();           // 데이터를 기반으로 보이는 것 만들어주기

                Debug.Log($"{chatContainer.MainChapter.Count}만큼 하이어라키가 생성되어야 함.");
            }
        }
    }
}
