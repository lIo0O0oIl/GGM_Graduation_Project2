using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Codice.Client.Common;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private int enumValue;
        private int enumValue2;

        private List<EChatEvent> chatEventList = new List<EChatEvent>();
        private List<string> LoadFileList = new List<string>();     

        private bool is_Expand = false;
        private bool is_LoadList = false;

        public void UpdateInspector(NodeView node)   
        {
            Clear();        // Delete all existing children

            is_Expand = false;
            is_LoadList = false;

            var container = new IMGUIContainer();
            container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);

                switch (node.node)
                {
                    case RootNode:
                        {
                            RootNode rootNode = node.node as RootNode;
                            GUILayout.Space(15);

                            rootNode.showName = EditorGUILayout.TextField("ShowName", rootNode.showName, EditorStyles.textArea);
                            GUILayout.Space(10);

                            // LoadFileName
                            if (!is_LoadList)
                            {
                                LoadFileList = new List<string>(rootNode.loadFileNameList);
                                is_LoadList = true;
                            }
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Round List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < LoadFileList.Count; ++i)
                                {
                                    LoadFileList[i] = EditorGUILayout.TextArea(LoadFileList[i]);
                                }
                                if (GUILayout.Button("Add Round"))
                                {
                                    LoadFileList.Add("");
                                }
                            }
                            rootNode.loadFileNameList = new List<string>(LoadFileList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();

                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.IntField("NowIndex", rootNode.nowIndex);
                        }
                        break;
                    case ChatNode:
                        {
                            GUILayout.Space(10);
                            ChatNode chatNode = node.node as ChatNode;

                            enumValue = (int)chatNode.state;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EChatState))); 
                            chatNode.state = (EChatState)enumValue;
                            GUILayout.Space(5);

                            GUILayout.Label("Chat");
                            chatNode.text = EditorGUILayout.TextArea(chatNode.text, EditorStyles.textArea);
                            GUILayout.Space(10);

                            enumValue2 = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));     
                            chatNode.face = (EFace)enumValue2;
                            GUILayout.Space(10);

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

                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", chatNode.is_UseThis);
                        }
                        break;
                    case AskNode:
                        {
                            GUILayout.Space(10);
                            AskNode askNode = node.node as AskNode;

                            GUILayout.Label("Ask");
                            askNode.askText = EditorGUILayout.TextArea(askNode.askText, EditorStyles.textArea);

                            if (!is_LoadList)
                            {
                                chatEventList = new List<EChatEvent>(askNode.textEvent);
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
                            askNode.textEvent = new List<EChatEvent>(chatEventList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();

                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", askNode.is_UseThis);
                        }
                        break;
                    case ConditionNode:
                        {
                            ConditionNode conditionNode = node.node as ConditionNode;
                            GUILayout.Space(15);

                            if (conditionNode.is_SpecificFile == false)
                            {
                                conditionNode.is_AllQuestion = EditorGUILayout.Toggle("AllQuestion", conditionNode.is_AllQuestion);
                            }
                            if (conditionNode.is_AllQuestion == false)
                            {
                                conditionNode.is_SpecificFile = EditorGUILayout.Toggle("SpecificFile", conditionNode.is_SpecificFile);
                            }

                            if (conditionNode.is_AllQuestion)
                            {
                                conditionNode.InitConditionNode();
                                if (conditionNode.checkClass is AllQuestion allQuestion)
                                {
                                    ++EditorGUI.indentLevel;
                                    allQuestion.Init(conditionNode);
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label($"AskCount");
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label($"{allQuestion.asks.Count}");
                                    GUILayout.EndHorizontal();
                                    --EditorGUI.indentLevel;
                                }
                            }
                            if (conditionNode.is_SpecificFile)
                            {
                                conditionNode.InitConditionNode();
                                if (conditionNode.checkClass is SpecificFile specificFile)
                                {
                                    ++EditorGUI.indentLevel;
                                    specificFile.fileName = EditorGUILayout.TextField("FileName", specificFile.fileName);
                                    --EditorGUI.indentLevel;
                                }
                            }

                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", conditionNode.is_UseThis);
                        }
                        break;
                }

                //GUILayout.Space(5);
                //EditorGUILayout.Toggle("child", is_ChildExist);
                EditorGUI.EndDisabledGroup();
            };

            Add(container);
        }

        private void ShowHeaderContextMenu(Rect position)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                Debug.Log("Clear");
                chatEventList.Clear();
                LoadFileList.Clear();
            });
            menu.DropDown(position);
        } 
    }
}
