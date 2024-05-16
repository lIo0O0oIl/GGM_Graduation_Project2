using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditorInternal;
using Codice.Client.Common;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private int enumValue;
        private int enumValue2;

        private List<EChatEvent> chatEventList = new List<EChatEvent>();     // 쳇팅들
        private List<string> evidenceList = new List<string>();     // 증거들

        private bool is_Expand = false;

        //public InspectorView()
        //{

        //}

        public void UpdateSelection(NodeView node)      // 누른 노드가 다른거면
        {
            Clear();        // 엘리먼트 모두 없애고

            is_Expand = false;

            var container = new IMGUIContainer();
            container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);
                GUILayout.Label("Description");
                node.node.description = EditorGUILayout.TextArea(node.node.description, EditorStyles.textArea);
                GUILayout.Space(15);

                bool is_ChildExist = false;
                switch (node.node)
                {
                    case RootNode:
                        RootNode rootNode = node.node as RootNode;
                        if (rootNode.child != null) is_ChildExist = true;
                        break;
                    case ChatNode:
                        {
                            ChatNode chatNode = node.node as ChatNode;
                            if (chatNode.child.Count != 0) is_ChildExist = true;        // 자식이 하나라도 있으면 

                            enumValue = (int)chatNode.state;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EChatState)));     // 말하는 것 타입
                            chatNode.state = (EChatState)enumValue;
                            
                            GUILayout.Label("Chat");
                            chatNode.text = EditorGUILayout.TextArea(chatNode.text, EditorStyles.textArea);     // 텍스트
                            
                            enumValue = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));        // 말할 때의 표정
                            chatNode.face = (EFace)enumValue2;

                            // 쳇팅 이벤트 추가
                            chatEventList = chatNode.textEvent;
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Chat Event List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < chatEventList.Count; ++i)
                                {
                                    chatEventList[i] = (EChatEvent)EditorGUILayout.EnumPopup(chatEventList[i]);
                                }
                                if (GUILayout.Button("Add Evidence"))
                                {
                                    chatEventList.Add(EChatEvent.None);
                                }
                            }
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();

                        }
                        break;
                    case AskNode:
                        {
                            AskNode askNode = node.node as AskNode;
                            if (askNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Ask");
                            askNode.ask = EditorGUILayout.TextArea(askNode.ask, EditorStyles.textArea);     // 질문
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", askNode.is_UseThis);
                        }
                        break;
                    case LockAskNode:
                        {
                            LockAskNode lockAskNode = node.node as LockAskNode;
                            if (lockAskNode.child != null) is_ChildExist = true;

                            // 증거
                            evidenceList = lockAskNode.evidence;
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Evidence List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < evidenceList.Count; ++i)
                                {
                                    evidenceList[i] = GUILayout.TextField(evidenceList[i]);
                                }
                                if (GUILayout.Button("Add Evidence"))
                                {
                                    evidenceList.Add(null);
                                }
                            }
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();

                            GUILayout.Label("Ask");
                            lockAskNode.ask = EditorGUILayout.TextArea(lockAskNode.ask, EditorStyles.textArea);     // 질문
                        }
                        break;
                }

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("child", is_ChildExist);
                EditorGUI.EndDisabledGroup();
            };

            Add(container);     // UI 컨테이너에 넣어줌, 실제로 보이게 해줌.
        }

        private void ShowHeaderContextMenu(Rect position)       // 메뉴 옆에 클리어 만들어주기
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                chatEventList.Clear();
                evidenceList.Clear();
            });
            menu.DropDown(position);
        }
    }
}
