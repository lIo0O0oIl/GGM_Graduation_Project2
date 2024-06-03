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
        private List<string> roundList = new List<string>();        // 보여질 라운드(파일모음이름)

        private bool is_Expand = false;
        private bool is_LoadList = false;

        public void UpdateInspector(NodeView node)      // 누른 노드가 다른거면
        {
            Clear();        // 엘리먼트 모두 없애고

            is_Expand = false;
            is_LoadList = false;

            var container = new IMGUIContainer();
            container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);

                bool is_ChildExist = false;
                switch (node.node)
                {
                    case RootNode:
                        {
                            RootNode rootNode = node.node as RootNode;
                            if (rootNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Description");
                            rootNode.description = EditorGUILayout.TextArea(rootNode.description, EditorStyles.textArea);
                            GUILayout.Space(15);

                            rootNode.showName = EditorGUILayout.TextField("ShowName", rootNode.showName, EditorStyles.textArea);     // 누군지
                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("SaveLocation");
                            GUILayout.Space(50);
                            rootNode.saveLocation = (ESaveLocation)EditorGUILayout.EnumPopup(rootNode.saveLocation);        // 이넘값 바꾸기
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);

                            // round 추가
                            if (!is_LoadList)
                            {
                                roundList = new List<string>(rootNode.round);
                                is_LoadList = true;
                            }
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Round List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < roundList.Count; ++i)
                                {
                                    roundList[i] = EditorGUILayout.TextArea(roundList[i]);
                                }
                                if (GUILayout.Button("Add Round"))
                                {
                                    roundList.Add("");
                                }
                            }
                            rootNode.round = new List<string>(roundList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                            GUILayout.Space(5);

                            // 이 챕터가 끝나면 다음 챕터로 이동하게 하는 것이 있냐
                            rootNode.is_nextChapter = EditorGUILayout.Toggle("is_nextChapter", rootNode.is_nextChapter);
                            if (rootNode.is_nextChapter)
                            {
                                rootNode.nextChapterIndex = EditorGUILayout.TextField("nextChapterIndex", rootNode.nextChapterIndex);     // 다음으로 넘어갈 쳇팅
                            }
                        }
                        break;
                    case ChatNode:
                        {
                            GUILayout.Space(15);
                            ChatNode chatNode = node.node as ChatNode;
                            if (chatNode.child.Count != 0) is_ChildExist = true;        // 자식이 하나라도 있으면 

                            enumValue = (int)chatNode.state;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EChatState)));     // 말하는 것 타입
                            chatNode.state = (EChatState)enumValue;
                            GUILayout.Space(5);

                            GUILayout.Label("Chat");
                            chatNode.text = EditorGUILayout.TextArea(chatNode.text, EditorStyles.textArea);     // 텍스트
                            GUILayout.Space(10);

                            enumValue2 = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));        // 말할 때의 표정
                            chatNode.face = (EFace)enumValue2;
                            GUILayout.Space(10);

                            // 쳇팅 이벤트 추가
                            if (!is_LoadList)
                            {
                                chatEventList = new List<EChatEvent>(chatNode.textEvent);
                                is_LoadList = true;
                            }
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Chat Event List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < chatEventList.Count; ++i)
                                {
                                    chatEventList[i] = (EChatEvent)EditorGUILayout.EnumPopup(chatEventList[i]);
                                }
                                if (GUILayout.Button("Add Chat Event"))
                                {
                                    chatEventList.Add(EChatEvent.Vibration);
                                }
                            }
                            chatNode.textEvent = new List<EChatEvent>(chatEventList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                    case AskNode:
                        {
                            GUILayout.Space(15);
                            AskNode askNode = node.node as AskNode;
                            if (askNode.child != null) is_ChildExist = true;
                            GUILayout.Space(10);

                            GUILayout.Label("Ask");
                            askNode.ask = EditorGUILayout.TextArea(askNode.ask, EditorStyles.textArea);     // 질문
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", askNode.is_UseThis);
                        }
                        break;
                    case LockAskNode:
                        {
                            GUILayout.Space(15);
                            LockAskNode lockAskNode = node.node as LockAskNode;
                            if (lockAskNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Ask");
                            lockAskNode.ask = EditorGUILayout.TextArea(lockAskNode.ask, EditorStyles.textArea);     // 질문
                            GUILayout.Space(10);

                            // 증거
                            if (!is_LoadList)
                            {
                                evidenceList = new List<string>(lockAskNode.evidence);
                                is_LoadList = true;
                            }
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
                            lockAskNode.evidence = new List<string>(evidenceList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                }

                GUILayout.Space(5);
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
                Debug.Log("dl");
                chatEventList.Clear();
                evidenceList.Clear();
                roundList.Clear();
            });
            menu.DropDown(position);
        }
    }
}
