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

            if (GUILayout.Button("Dic Add"))
            {
                chatContainer.HumanAndChatDictionary.Add("Assistant", new List<Node>());
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
