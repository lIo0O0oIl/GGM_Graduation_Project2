using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    [CreateAssetMenu(menuName = "SO/ChatTree")]
    public class ChatTree : ScriptableObject
    {
        public RootNode rootNode;

        public string humanName;
        public List<Node> nodeList = new List<Node>();

#if UNITY_EDITOR

        public Node CreateNode(Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CE(CreateNode)");
            nodeList.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);         // Add as Asset Child

            Undo.RegisterCreatedObjectUndo(node, "CE(CreateNode)");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "CE(DeleteNode)");
            nodeList.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);      // Delete directly from memory
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void AddChild(Node parent, Node child)
        {
            AddParent(parent, child);

            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.childList.Add(child);
                EditorUtility.SetDirty(chatNode);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                EditorUtility.SetDirty(askNode);
                return;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.childList.Add(child);
                EditorUtility.SetDirty(conditionNode);
            }
        }

        public void AddParent(Node parent, Node child)
        {
            var chatNode = child as ChatNode;
            if (chatNode != null)
            {
                chatNode.parent = parent;
                return;
            }

            var askNode = child as AskNode;
            if (askNode != null)
            {
                askNode.parent = parent;
                return;
            }

            var conditionNode = child as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.parentList.Add(parent);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            RemoveParent(parent, child);

            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.childList.Remove(child);
                EditorUtility.SetDirty(chatNode);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = null;
                EditorUtility.SetDirty(askNode);
                return;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.childList.Remove(child);
                EditorUtility.SetDirty(conditionNode);
            }
        }

        public void RemoveParent(Node parent, Node child)
        {
            var chatNode = child as ChatNode;
            if (chatNode != null)
            {
                chatNode.parent = null;
                EditorUtility.SetDirty(chatNode);
                return;
            }

            var askNode = child as AskNode;
            if (askNode != null)
            {
                askNode.parent = null;
                EditorUtility.SetDirty(askNode);
                return;
            }

            var conditionNode = child as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.parentList.Remove(child);
                EditorUtility.SetDirty(conditionNode);
            }
        }
#endif

        public List<Node> GetChild(Node parent)
        {
            List<Node> children = new List<Node>();

            var rootNode = parent as RootNode;
            if (rootNode != null && rootNode.child != null)
            {
                children.Add(rootNode.child);
                return children;
            }

            var askNode = parent as AskNode;
            if (askNode != null && askNode.child != null)
            {
                children.Add(askNode.child);
                return children;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null && chatNode.childList.Count != 0)
            {
                children = chatNode.childList;
                return children;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null && conditionNode.childList.Count != 0)
            {
                children = conditionNode.childList;
            }

            return children;
        }
    }
}