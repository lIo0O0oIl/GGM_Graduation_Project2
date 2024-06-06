using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    [CustomEditor(typeof(ChatContainer))]
    public class ChatContainerCustomInspector : Editor
    {
        private bool is_Open;
        private List<bool> is_OpenList = new List<bool>();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ChatContainer chatContainer = (ChatContainer)target;

            // Show  HumanAndChatDictionary
            is_Open = EditorGUILayout.BeginFoldoutHeaderGroup(is_Open, "HumanAndChatDictionary", menuAction: ShowHeaderContextMenu);
            
            if (is_Open)
            {
                GUIStyle boxStyle = EditorStyles.helpBox;
                GUILayout.BeginVertical(boxStyle);

                is_OpenList.Add(false);
                foreach (var key in chatContainer.HumanAndChatDictionary)
                {
                    is_OpenList[0] = EditorGUILayout.BeginFoldoutHeaderGroup(is_OpenList[0], key.Key);
                    GUILayout.Label(key.Key);
                    foreach (var value in key.Value)
                    {
                        if (value is RootNode root)
                        {
                            GUILayout.Label(root.showName);
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup(); 
                }

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            GUILayout.Label(chatContainer.HumanAndChatDictionary.Count.ToString());

            if (GUILayout.Button("Dic Add"))
            {
                chatContainer.HumanAndChatDictionary.Add("Assistant", new List<Node>());
            }

            if (chatContainer.HumanAndChatDictionary.ContainsKey("Assistant"))
            {
                GUILayout.Label(chatContainer.HumanAndChatDictionary["Assistant"].Count.ToString());
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
