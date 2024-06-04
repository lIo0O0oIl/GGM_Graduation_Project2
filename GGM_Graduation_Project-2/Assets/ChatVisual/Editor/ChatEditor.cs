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
    private VisualTreeAsset treeAsset = null;           // UI ???뵬 ?節뚮선雅뚯눊由?

    private ChatView chatView;        // 筌??샒????쇰선揶쎛 ??덈뮉 ??
    private InspectorView inspectorView;        // ?紐꾨뮞??브숲 ??野껉퍔??
    private HierarchyView hierarchyView;      // ?꾨뗀諭뜻묾怨뺤뺘 GUI, ?袁⑸퓠??筌띾?釉??野? ??륁뵠?????
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
            chatView.SaveChatSystem();      // 筌≪럩??????筌왖疫뀀뜃?댐쭪? ??? 野????館鍮먧틠?⑤┛
        }
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;     // ?룐뫂?껅에???쇱젟??곸㉡.

        // UXML 筌띾슢諭??곻폒疫?
        VisualElement template = treeAsset.Instantiate();
        template.style.flexGrow = 1;        // ?遺용꺖 ??됰퓠 ?節뚮???????곗춳???뚣끉彛?野??硫?
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
        root.styleSheets.Add(styleSheet);       // ???????깆뱽 揶쎛?紐?餓?

        chatView = root.Q<ChatView>("chat-view");
        inspectorView = root.Q<InspectorView>("inspector-view");        // ?紐꾨뮞??됯숲 ??已??곗쨮 揶쎛?紐꾩궎疫?
        hierarchyView = root.Q<HierarchyView>("hierarchy-view");       // ?袁⑥삋??揶쎛?紐꾩궎疫?hierarchy ??
       
        arrayAddBtn = root.Q<Button>("AddBtn");     // 甕곌쑵??揶쎛?紐꾩궎疫?
        arrayAddBtn.tooltip = "Chapter Add";
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;
        dangerBtn = root.Q<Button>("ClearBtn");
        dangerBtn.tooltip = "All Nodes Delete";
        dangerBtn.clickable.clicked += OnClearNodes;

        chatView.OnNodeSelected += OnSelectionNodeChanged;      // ?紐껊굡???袁ⓥ뀲 野껉퍔?????わ쭪?筌?????源???紐꾪뀱

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("獄쏄퀣肉??곕떽???곻폒疫?");
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
            Debug.Log("Number of nodes to create : " + chatContainer.nodes.Count);
        }
    }

    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        chatView.SaveChatSystem();
        inspectorView.UpdateInspector(nodeView);        // ???紐껊굡???????블??紐꾨뮞??됯숲?????젻餓?
    }

    private void OnSelectionChange()        // ?癒?탵?怨? ???怨밴묶?癒?퐣 ?얜똻堉드첎????醫뤾문??됱뱽 ??
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))      // ??륁뵠?????筌≪럩肉??揶쎛?紐꾩궎疫?
            {
                //Debug.Log(chatContainer.nodes.Count + "揶쏆뮇???紐껊굡揶쎛 鈺곕똻???");

                chatContainer.ChangeNowChapter(0);      // 揶쎛??筌ｌ꼷??? 0甕곕뜆??

                hierarchyView.UpdateHierarchy(chatContainer, chatView);      // ??륁뵠?????筌띾슢諭??곻폒疫?
                
                chatView.LoadChatSystem(chatContainer);     // ?怨쀬뵠??嚥≪뮆諭???곻폒疫?
                chatView.PopulateView();           // ?怨쀬뵠?怨? 疫꿸퀡而??곗쨮 癰귣똻???野?筌띾슢諭??곻폒疫?

                Debug.Log($"{chatContainer.MainChapter.Count}筌띾슦寃???륁뵠?????? ??밴쉐??뤿선????");
            }
        }
    }
}
