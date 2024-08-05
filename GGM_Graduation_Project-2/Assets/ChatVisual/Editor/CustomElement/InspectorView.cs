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
        private int enumValue3;

        private List<EChatEvent> chatEventList = new List<EChatEvent>();
        private List<string> LoadFileList = new List<string>(); 

        private bool is_Expand = false;
        private bool is_LoadList = false;
        private bool is_LoadNextDialog = false;

        public void UpdateInspector(NodeView node)   
        {
            Clear();        // Delete all existing children

            is_Expand = false;
            is_LoadList = false;
            is_LoadNextDialog = false;

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
                           /* if (!is_LoadList)
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
                            GUILayout.EndVertical();*/
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
                            chatNode.chatText = EditorGUILayout.TextArea(chatNode.chatText, EditorStyles.textArea);
                            GUILayout.Space(10);

                            enumValue2 = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));     
                            chatNode.face = (EFace)enumValue2;
                            GUILayout.Space(10);

                            enumValue3 = (int)chatNode.type;
                            enumValue3 = GUILayout.Toolbar(enumValue3, System.Enum.GetNames(typeof(EChatType)));
                            chatNode.type = (EChatType)enumValue3;
                            GUILayout.Space(10);

                            if (!is_LoadList)
                            {
                                chatEventList = new List<EChatEvent>(chatNode.textEvent);
                                LoadFileList = new List<string>(chatNode.loadFileName);
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
                            GUILayout.Space(10);

                            int loadFileCount = 0;
                            for (int i = 0; i < chatEventList.Count; i++)
                            {
                                if (chatEventList[i] == EChatEvent.LoadFile)
                                {
                                     loadFileCount++;
                                }
                                if (chatEventList[i] == EChatEvent.LoadNextDialog)
                                {
                                    is_LoadNextDialog = true;
                                }
                            }
                            for (int i = 0; i < loadFileCount; i++)
                            {
                                if (i >= LoadFileList.Count)
                                {
                                    Debug.Log("LoadFileList 媛 ADD ??");
                                    LoadFileList.Add("");
                                }
                                LoadFileList[i] = EditorGUILayout.TextArea(LoadFileList[i], EditorStyles.textArea);
                            }
                            chatNode.loadFileName = new List<string>(LoadFileList);
                            GUILayout.Space(10);

                            if (is_LoadNextDialog)
                            {
                                chatNode.LoadNextDialog = EditorGUILayout.TextArea(chatNode.LoadNextDialog, EditorStyles.textArea);
                            }

                            chatNode.is_UseThis = EditorGUILayout.Toggle("is_UseThis", chatNode.is_UseThis);

                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.IntField("ParentExist", chatNode.parent != null ? 1 : 0);
                            EditorGUILayout.IntField("ChildListCount", chatNode.childList.Count);
                            EditorGUI.EndDisabledGroup();
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
                                    if (chatEventList[i] == EChatEvent.LoadNextDialog)
                                    {
                                        is_LoadNextDialog = true;
                                    }
                                }
                                if (GUILayout.Button("Add Chat Event"))
                                {
                                    chatEventList.Add(EChatEvent.Vibration);
                                }
                            }
                            askNode.textEvent = new List<EChatEvent>(chatEventList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                            GUILayout.Space(10);

                            enumValue = (int)askNode.askType;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EAskType)));
                            askNode.askType = (EAskType)enumValue;
                            GUILayout.Space(10);

                            if (is_LoadNextDialog)
                            {
                                askNode.LoadNextDialog = EditorGUILayout.TextArea(askNode.LoadNextDialog, EditorStyles.textArea);
                            }

                            askNode.is_UseThis = EditorGUILayout.Toggle("is_UseThis", askNode.is_UseThis);
                        }
                        break;
                    case ConditionNode:
                        {
                            ConditionNode conditionNode = node.node as ConditionNode;
                            GUILayout.Space(15);

                            if (!conditionNode.is_SpecificFile && !conditionNode.is_LockQuestion)
                            {
                                conditionNode.is_AllQuestion = EditorGUILayout.Toggle("AllQuestion", conditionNode.is_AllQuestion);
                            }
                            if (!conditionNode.is_AllQuestion && !conditionNode.is_LockQuestion)
                            {
                                conditionNode.is_SpecificFile = EditorGUILayout.Toggle("SpecificFile", conditionNode.is_SpecificFile);
                            }
                            if (!conditionNode.is_AllQuestion && !conditionNode.is_SpecificFile)
                            {
                                conditionNode.is_LockQuestion = EditorGUILayout.Toggle("is_LockQuestion", conditionNode.is_LockQuestion);
                            }

                            if (conditionNode.is_AllQuestion)
                            {
                                conditionNode.InitConditionNode();
                                if (conditionNode.checkClass is AllQuestion allQuestion)
                                {
                                    ++EditorGUI.indentLevel;                                 
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label($"AskCount");
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label($"{allQuestion.conditionNode.asks.Count}");
                                    GUILayout.EndHorizontal();
                                    
                                    if (GUILayout.Button("AutoInit"))
                                    {
                                        allQuestion.Init(conditionNode);
                                    }
                                    --EditorGUI.indentLevel;
                                }
                            }

                            if (conditionNode.is_SpecificFile)
                            {
                                conditionNode.InitConditionNode();
                                if (conditionNode.checkClass is SpecificFile specificFile)
                                {
                                    ++EditorGUI.indentLevel;
                                    specificFile.conditionNode.fileName = EditorGUILayout.TextField("FileName", specificFile.conditionNode.fileName);
                                    --EditorGUI.indentLevel;
                                }
                            }

                            if (conditionNode.is_LockQuestion)
                            {
                                ++EditorGUI.indentLevel;
                                conditionNode.fileName = EditorGUILayout.TextField("FileName", conditionNode.fileName);
                                --EditorGUI.indentLevel;
                            }

                            conditionNode.is_UseThis = EditorGUILayout.Toggle("is_UseThis", conditionNode.is_UseThis);
                        }
                        break;
                }

                //GUILayout.Space(5);
                //EditorGUILayout.Toggle("child", is_ChildExist);

                GUILayout.Space(5);
                if (GUILayout.Button("Open SO"))
                {
                    Selection.activeObject = node.node;
                }
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
