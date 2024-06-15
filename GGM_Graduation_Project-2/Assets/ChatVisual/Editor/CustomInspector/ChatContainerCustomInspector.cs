using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    //[CustomEditor(typeof(ChatContainer))]
    public class ChatContainerCustomInspector : Editor
    {
        private bool is_Open, is_Open2;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ChatContainer chatContainer = (ChatContainer)target;


            is_Open2 = EditorGUILayout.BeginFoldoutHeaderGroup(is_Open2, "NodeList");
            if (is_Open2)
            {
                GUIStyle boxStyle = EditorStyles.helpBox;
                GUILayout.BeginVertical(boxStyle);

                foreach (var key in chatContainer.nodeList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{key.nodes.Count}");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            // Show  HumanAndChatDictionary
            is_Open = EditorGUILayout.BeginFoldoutHeaderGroup(is_Open, "HumanAndChatDictionary", menuAction: ShowHeaderContextMenu);
            
            if (is_Open)
            {
                GUIStyle boxStyle = EditorStyles.helpBox;
                GUILayout.BeginVertical(boxStyle);

                foreach (var key in chatContainer.HumanAndChatDictionary)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{key.Key}");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"{key.Value.Count}");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (GUILayout.Button("Initialization Dictionary"))
            {
                for (int i = 0; i < chatContainer.nameList.Count; i++)
                {
                    chatContainer.HumanAndChatDictionary.Add(chatContainer.nameList[i], chatContainer.nodeList[i].nodes);
                    for (int j = 0; j < chatContainer.nodeList[i].nodes.Count; j++)
                    {
                        Debug.Log(chatContainer.nodeList[i].nodes[j]);
                    }
                }
            }

            if (!chatContainer.HumanAndChatDictionary.ContainsKey("HG"))
            {
                if (GUILayout.Button("Start Dictionary (Add HG)"))
                {
                    chatContainer.HumanAndChatDictionary.Add("HG", new List<Node>());
                    chatContainer.nameList.Add("HG");
                    chatContainer.nodeList.Add(new Nodes());
                }
            }

            if (GUILayout.Button("Delete All Dictionary"))
            {
                chatContainer.HumanAndChatDictionary.Clear();
                chatContainer.nameList.Clear();
                chatContainer.nodeList.Clear();
            }

            EditorGUILayout.Space(15);
            if (GUILayout.Button("Save"))
            {
                for (int i = 0; i < chatContainer.nodeList.Count; i++)
                {
                    List<Node> nodeList = new List<Node>(chatContainer.nodeList[i].nodes);
                    EditorPrefs.SetInt($"NodeListCount{i}", nodeList.Count);
                    for (int j = 0; j < nodeList.Count; j++)
                    {
                        string data = "";
                        switch(nodeList[j])
                        {
                            case RootNode rootNode:
                                data = JsonUtility.ToJson(rootNode);
                                break;
                            case ChatNode chatNode:
                                data = JsonUtility.ToJson(chatNode);
                                break;
                            case AskNode askNode:
                                data = JsonUtility.ToJson(askNode);
                                break;
                            case ConditionNode conditionNode:
                                data = JsonUtility.ToJson(conditionNode);
                                break;
                        }
                        EditorPrefs.SetString($"NodeSave{i}-{j}", data);
                    }
                }
                EditorPrefs.SetInt("NodeListCount", chatContainer.nodeList.Count);

            }
            if (GUILayout.Button("Load"))
            {
                chatContainer.nodeList.Clear();
                int listCount = EditorPrefs.GetInt("NodeListCount");
                for (int i = 0; i < listCount; i++)
                {
                    chatContainer.nodeList.Add(new Nodes());
                    int nodeCount = EditorPrefs.GetInt($"NodeListCount{i}");
                    List<Node> nodeList = new List<Node>();
                    for (int j = 0; j < nodeCount; j++)
                    {
                        string data = EditorPrefs.GetString($"NodeSave{i}-{j}");
                        if (data.Contains("showName"))
                        {
                            nodeList.Add(JsonUtility.FromJson<RootNode>(data));
                        }
                        else if(data.Contains("chatText"))
                        {
                            nodeList.Add(JsonUtility.FromJson<ChatNode>(data));
                        }
                        else if (data.Contains("askText"))
                        {
                            nodeList.Add(JsonUtility.FromJson<AskNode>(data));
                        }
                        else if (data.Contains("is_AllQuestion"))
                        {
                            nodeList.Add(JsonUtility.FromJson<ConditionNode>(data));
                        }
                    }
                    chatContainer.nodeList[i].nodes = nodeList;
                }

                //JsonUtility.FromJsonOverwrite(data, chatContainer.oneNodesTest);
                //Debug.Log(chatContainer.oneNodesTest as RootNode);
            }
            if (GUILayout.Button("Delete Save Data"))
            {
                EditorPrefs.DeleteAll();
            }
        }
        private void ShowHeaderContextMenu(Rect position)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                Debug.Log("del");
            });
            menu.DropDown(position);
        }
    }
}
