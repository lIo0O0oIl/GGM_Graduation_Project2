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
    private Button dangerBtn;

    private ChatContainer chatContainer;

    [MenuItem("ChatSystem/ChatEditor")]
    public static void OpenWindow()
    {
        GetWindow<ChatEditor>("ChatEditor");
    }

    // private void OnDestroy() {  }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement; 

        VisualElement template = treeAsset.Instantiate();
        template.style.flexGrow = 1;      
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
        root.styleSheets.Add(styleSheet);     

        chatView = root.Q<ChatView>("chat-view");
        inspectorView = root.Q<InspectorView>("inspector-view");        
        hierarchyView = root.Q<HierarchyView>("hierarchy-view");     
       
        arrayAddBtn = root.Q<Button>("AddBtn");  
        arrayAddBtn.tooltip = "Chapter Add";
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;
        dangerBtn = root.Q<Button>("ClearBtn");
        dangerBtn.tooltip = "All Nodes Delete";
        dangerBtn.clickable.clicked += OnClearNodes;

        chatView.OnNodeSelected += OnSelectionNodeChanged;

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("Add human");
        }
    }

    private void OnClearNodes()
    {
        if (chatContainer != null)
        {
            Debug.Log("Delete all nodes");
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
                // The current chat is an assistant's chat
                chatContainer.nowHumanName = "Assistant";


                hierarchyView.UpdateHierarchy(chatContainer, chatView);      
                chatView.LoadChatData(chatContainer);    
                chatView.PopulateView();          

                //Debug.Log($"Human count : {chatContainer.HumanAndChatDictionary.Count}");
            }
        }
    }
}
