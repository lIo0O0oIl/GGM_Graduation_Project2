using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class ChatEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset treeAsset = null;

    private HierarchyView hierarchyView;

    private IMGUIContainer blackBoardView;      // 코드기반 GUI, 아래꺼

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

        hierarchyView = root.Q<HierarchyView>("hierarchy-view");        // 하이어라키창 이름으로 가져오기.
        blackBoardView = root.Q<IMGUIContainer>("inspector");       // 아래꺼 가져오기 아직 blackboard 임.

    }
}
