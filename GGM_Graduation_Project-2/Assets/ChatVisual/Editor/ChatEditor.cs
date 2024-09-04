using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.Callbacks;

public class ChatEditor : EditorWindow
{
    //[SerializeField]
    //private VisualTreeAsset treeAsset = null;           // UI

    //private ChatView chatView;   
    //private InspectorView inspectorView;        
    //private HierarchyView hierarchyView;  
    //private Button arrayAddBtn;
    //private Button sortBtn;

    //private ChatContainer chatContainer;

    //[MenuItem("ChatSystem/ChatEditor")]
    //public static void OpenWindow()
    //{
    //    GetWindow<ChatEditor>("ChatEditor");
    //}

    //[OnOpenAsset]
    //public static bool OnOpenAsset(int instanceId, int line)
    //{
    //    if (Selection.activeObject is ChatTree)
    //    {
    //        OpenWindow();
    //        return true;
    //    }
    //    return false;
    //}

    //public void CreateGUI()
    //{
    //    VisualElement root = rootVisualElement; 

    //    VisualElement template = treeAsset.Instantiate();
    //    template.style.flexGrow = 1;      
    //    root.Add(template);

    //    var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
    //    root.styleSheets.Add(styleSheet);     

    //    chatView = root.Q<ChatView>("chat-view");
    //    chatView.InitChatView(root.Q<Label>("ChatViewLabel"));
    //    inspectorView = root.Q<InspectorView>("inspector-view");        
    //    hierarchyView = root.Q<HierarchyView>("hierarchy-view");

    //    arrayAddBtn = root.Q<Button>("AddBtn");  
    //    arrayAddBtn.tooltip = "Add Human";
    //    arrayAddBtn.clickable.clicked += OnArrayAddBtn;
    //    sortBtn = root.Q<Button>("SortBtn");
    //    sortBtn.tooltip = "Delete All Nodes";
    //    sortBtn.clickable.clicked += FindChatContainer;

    //    chatView.OnNodeSelected += OnSelectionNodeChanged;

    //    OnSelectionChange();
    //}

    //private void OnArrayAddBtn()
    //{
    //    if (chatContainer != null)
    //    {
    //        Debug.Log("Add human");

    //        ChatTree newChatTree = ScriptableObject.CreateInstance<ChatTree>();
    //        string assetPath = "Assets/06.SO/HumanChat/NewHuman.asset";

    //        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);       // Create new names for duplicate filenames
    //        AssetDatabase.CreateAsset(newChatTree, assetPath);
    //        chatContainer.chatTrees.Add(newChatTree);

    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh();

    //        hierarchyView.UpdateHierarchy();
    //    }
    //}
    
    //private void FindChatContainer()
    //{
    //    chatContainer = FindObjectOfType<ChatContainer>();
    //    InitEditor();
    //}

    //private void OnSelectionNodeChanged(NodeView nodeView)
    //{
    //    inspectorView.UpdateInspector(nodeView); 
    //}

    //private void OnSelectionChange()
    //{
    //    if (Selection.activeGameObject != null)
    //    {
    //        if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))
    //        {
    //            // The current chat is an assistant's(HG) chat
    //            //chatContainer.nowHumanName = "HG";

    //            InitEditor();
    //        }
    //    }
    //}

    //private void InitEditor()
    //{
    //    hierarchyView.InitHierarchy(chatContainer, chatView, inspectorView);
    //    hierarchyView.UpdateHierarchy();

    //    if (chatContainer.chatTrees.Count <= 0)
    //    {
    //        OnArrayAddBtn();
    //    }

    //    chatView.PopulateView(chatContainer.chatTrees[0]);
    //}
}
