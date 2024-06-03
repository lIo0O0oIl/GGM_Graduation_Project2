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
    private VisualTreeAsset treeAsset = null;           // UI ?뚯씪 ?ｌ뼱二쇨린

    private ChatView chatView;        // 梨쀭똿???ㅼ뼱媛 ?덈뒗 怨?
    private InspectorView inspectorView;        // ?몄뒪?⑺꽣 ??寃껋엫.
    private HierarchyView hierarchyView;      // 肄붾뱶湲곕컲 GUI, ?꾩뿉爰?留먰븯??寃? ?섏씠?대씪??
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
            chatView.SaveChatSystem();      // 李쎌쓣 ????吏湲덇퉴吏 ?댁? 寃???ν빐二쇨린
        }
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;     // 猷⑦듃濡??ㅼ젙?댁쨲.

        // UXML 留뚮뱾?댁＜湲?
        VisualElement template = treeAsset.Instantiate();
        template.style.flexGrow = 1;        // ?붿냼 ?덉뿉 ?ｌ뿀?????쇰쭏??而ㅼ쭏 寃??멸?
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
        root.styleSheets.Add(styleSheet);       // ???ㅽ??쇱쓣 媛?몄?以?

        chatView = root.Q<ChatView>("chat-view");
        inspectorView = root.Q<InspectorView>("inspector-view");        // ?몄뒪?숉꽣 ?대쫫?쇰줈 媛?몄삤湲?
        hierarchyView = root.Q<HierarchyView>("hierarchy-view");       // ?꾨옒爰?媛?몄삤湲?hierarchy ??
       
        arrayAddBtn = root.Q<Button>("AddBtn");     // 踰꾪듉 媛?몄삤湲?
        arrayAddBtn.tooltip = "Chapter Add";
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;
        dangerBtn = root.Q<Button>("ClearBtn");
        dangerBtn.tooltip = "All Nodes Delete";
        dangerBtn.clickable.clicked += OnClearNodes;

        chatView.OnNodeSelected += OnSelectionNodeChanged;      // ?몃뱶瑜??꾨Ⅸ 寃껋씠 ?щ씪吏硫????대깽???몄텧

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("諛곗뿴 異붽??댁＜湲?");
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
            Debug.Log("?몃뱶??媛쒖닔??" + chatContainer.nodes.Count + "媛??낅땲??");
            Close();
        }
    }

    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        inspectorView.UpdateInspector(nodeView);        // ???몃뱶瑜??뚮??ㅺ퀬 ?몄뒪?숉꽣???뚮젮以?
    }

    private void OnSelectionChange()        // ?먮뵒?곕? ???곹깭?먯꽌 臾댁뼵媛瑜??좏깮?덉쓣 ??
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))      // ?섏씠?대씪??李쎌뿉??媛?몄삤湲?
            {
                //Debug.Log(chatContainer.nodes.Count + "媛쒖쓽 ?몃뱶媛 議댁옱??");

                chatContainer.ChangeNowChapter(0);      // 媛??泥섏쓬? 0踰덉㎏

                hierarchyView.UpdateHierarchy(chatContainer, chatView);      // ?섏씠?대씪??留뚮뱾?댁＜湲?
                
                chatView.LoadChatSystem(chatContainer);     // ?곗씠??濡쒕뱶 ?댁＜湲?
                chatView.PopulateView();           // ?곗씠?곕? 湲곕컲?쇰줈 蹂댁씠??寃?留뚮뱾?댁＜湲?

                Debug.Log($"{chatContainer.MainChapter.Count}留뚰겮 ?섏씠?대씪?ㅺ? ?앹꽦?섏뼱????");
            }
        }
    }
}
