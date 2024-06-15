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
    private VisualTreeAsset treeAsset = null;           // UI

    private ChatView chatView;   
    private InspectorView inspectorView;        
    private HierarchyView hierarchyView;  
    private Button arrayAddBtn;
    private Button sortBtn;

    private ChatContainer chatContainer;

    [MenuItem("ChatSystem/ChatEditor")]
    public static void OpenWindow()
    {
        GetWindow<ChatEditor>("ChatEditor");
    }

    private void OnDestroy() 
    {
        if (chatContainer != null)
        {
            chatView.SaveChatData();
            Debug.Log("Close and save");
        }
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement; 

        VisualElement template = treeAsset.Instantiate();
        template.style.flexGrow = 1;      
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
        root.styleSheets.Add(styleSheet);     

        chatView = root.Q<ChatView>("chat-view");
        chatView.InitChatView(root.Q<Label>("ChatViewLabel"));
        inspectorView = root.Q<InspectorView>("inspector-view");        
        hierarchyView = root.Q<HierarchyView>("hierarchy-view");

        arrayAddBtn = root.Q<Button>("AddBtn");  
        arrayAddBtn.tooltip = "Add Human";
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;
        sortBtn = root.Q<Button>("SortBtn");
        sortBtn.tooltip = "Delete All Nodes";
        sortBtn.clickable.clicked += OnSortNodes;

        chatView.OnNodeSelected += OnSelectionNodeChanged;

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("Add human");
            chatContainer.HumanAndChatDictionary.Add("???", new List<Node>());
            string temp = chatContainer.nowHumanName;
            chatContainer.nowHumanName = "???";
            if (chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].Count == 0)
            {
                chatContainer.CreateNode(typeof(RootNode));
                if (chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName][0] is RootNode rootNode)
                {
                    rootNode.showName = "???";
                }
            }
            chatContainer.nowHumanName = temp;
            chatContainer.nameList.Add("???");
            chatContainer.nodeList.Add(new Nodes());
            hierarchyView.UpdateHierarchy();
        }
    }
    
    private void OnSortNodes()
    {
        if (chatContainer != null)
        {
            Debug.Log("Sort all nodes!");
        }
    }

    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        //chatView.SaveChatSystem();
        inspectorView.UpdateInspector(nodeView); 
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))    
            {
                // The current chat is an assistant's(HG) chat
                chatContainer.nowHumanName = "HG";

                hierarchyView.InitHierarchy(chatContainer, chatView, inspectorView);
                hierarchyView.UpdateHierarchy();      
                
                chatView.LoadChatData(chatContainer);

                chatView.PopulateView();
            }
        }
    }
}
