using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
//    [CreateAssetMenu(menuName ="SO/ChatTree")]
//    public class ChatTree : ScriptableObject
//    {
//        public RootNode rootNode;

//        public string humanName;
//        public List<Node> nodeList = new List<Node>();

//        //private bool is_ConditionNowOk = false;
//        ///private int nowChatIndex = 1;       // index for Sort

//#if UNITY_EDITOR

//        public DefaultA CreateNode(Type type)
//        {
//            var node = ScriptableObject.CreateInstance(type) as Node;
//            node.name = type.Name;
//            node.guid = GUID.Generate().ToString();

//            Undo.RecordObject(this, "CE(CreateNode)");
//            nodeList.Add(node);

//            AssetDatabase.AddObjectToAsset(node, this);         // Add as Asset Child

//            Undo.RegisterCreatedObjectUndo(node, "CE(CreateNode)");
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();

//            return node;
//        }

//        public void DeleteNode(Node node)
//        {
//            Undo.RecordObject(this, "CE(DeleteNode)");
//            nodeList.Remove(node);
//            AssetDatabase.RemoveObjectFromAsset(node);
//            Undo.DestroyObjectImmediate(node);      // Delete directly from memory
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//        }

//        public void AddChild(Node parent, Node child)
//        {
//            //Debug.Log($"Connect! parent : {parent}, child : {child}");

//            AddParent(parent, child);

//            var rootNode = parent as RootNode;
//            if (rootNode != null)
//            {
//                rootNode.child = child;
//                EditorUtility.SetDirty(rootNode);
//                return;
//            }

//            var chatNode = parent as ChatNode;
//            if (chatNode != null)
//            {
//                chatNode.childList.Add(child);
//                EditorUtility.SetDirty(chatNode);
//                return;
//            }

//            var askNode = parent as AskNode;
//            if (askNode != null)
//            {
//                askNode.child = child;
//                EditorUtility.SetDirty(askNode);
//                return;
//            }

//            var conditionNode = parent as ConditionNode;
//            if (conditionNode != null)
//            {
//                conditionNode.childList.Add(child);
//                EditorUtility.SetDirty(conditionNode);
//            }
//        }

//        public void AddParent(Node parent, Node child)
//        {
//            var chatNode = child as ChatNode;
//            if (chatNode != null)
//            {
//                chatNode.parent = parent;
//                return;
//            }

//            var askNode = child as AskNode;
//            if (askNode != null)
//            {
//                askNode.parent = parent;
//                return;
//            }

//            var conditionNode = child as ConditionNode;
//            if (conditionNode != null)
//            {
//                conditionNode.parentList.Add(parent);
//            }
//        }

//        public void RemoveChild(Node parent, Node child)
//        {
//            RemoveParent(parent, child);

//            var rootNode = parent as RootNode;
//            if (rootNode != null)
//            {
//                rootNode.child = null;
//                EditorUtility.SetDirty(rootNode);
//                return;
//            }

//            var chatNode = parent as ChatNode;
//            if (chatNode != null)
//            {
//                chatNode.childList.Remove(child);
//                EditorUtility.SetDirty(chatNode);
//                return;
//            }

//            var askNode = parent as AskNode;
//            if (askNode != null)
//            {
//                askNode.child = null;
//                EditorUtility.SetDirty(askNode);
//                return;
//            }

//            var conditionNode = parent as ConditionNode;
//            if (conditionNode != null)
//            {
//                conditionNode.childList.Remove(child);
//                EditorUtility.SetDirty(conditionNode);
//            }
//        }

//        public void RemoveParent(Node parent, Node child)
//        {
//            var chatNode = child as ChatNode;
//            if (chatNode != null)
//            {
//                chatNode.parent = null;
//                EditorUtility.SetDirty(chatNode);
//                return;
//            }

//            var askNode = child as AskNode;
//            if (askNode != null)
//            {
//                askNode.parent = null;
//                EditorUtility.SetDirty(askNode);
//                return;
//            }

//            var conditionNode = child as ConditionNode;
//            if (conditionNode != null)
//            {
//                conditionNode.parentList.Remove(child);
//                EditorUtility.SetDirty(conditionNode);
//            }
//        }
//#endif

//        public List<Node> GetChild(Node parent)
//        {
//            List<Node> children = new List<Node>();

//            var rootNode = parent as RootNode;
//            if (rootNode != null && rootNode.child != null)
//            {
//                children.Add(rootNode.child);
//                return children;
//            }

//            var askNode = parent as AskNode;
//            if (askNode != null && askNode.child != null)
//            {
//                children.Add(askNode.child);
//                return children;
//            }

//            var chatNode = parent as ChatNode;
//            if (chatNode != null && chatNode.childList.Count != 0)
//            {
//                children = chatNode.childList;
//                return children;
//            }

//            var conditionNode = parent as ConditionNode;
//            if (conditionNode != null && conditionNode.childList.Count != 0)
//            {
//                children = conditionNode.childList;
//            }

//            return children;
//        }
        
//        public void SortChildAndIndex(Node startNode, int startIndex)    
//        {
//            /*Node nowNode = startNode;         // RootNode
//            Queue<Node> askChatNode = new Queue<Node>();
//            List<Node> children = new List<Node>();
//            nowChatIndex = startIndex;

//            Debug.Log("nono");
//            children = GetChild(nowNode);

//            // DFS BFS 
//            while (children.Count > 0)
//            {
//                children = GetChild(nowNode);
//                Debug.Log(children.Count);

//                if (children.Count == 1 && children[0] is ChatNode)            // When a child is a ChatNode
//                {
//                    if (is_ConditionNowOk)
//                    {
//                        if (children[0] is ConditionNode condition)
//                        {
//                            *//*children[0].index = nowChatIndex;
//                            children[0].indexLabel.text = nowChatIndex.ToString();
//                            nowChatIndex++;
//                            nowNode = children[0];
//                            continue;*//*
//                            Debug.Log(condition);
//                        }
//                    }
//                    children[0].index = nowChatIndex;
//                    children[0].indexLabel.text = nowChatIndex.ToString();
//                    nowChatIndex++;
//                    nowNode = children[0];
//                    Debug.Log("ho");
//                }
//                else        // When child is not a ChatNode
//                {
//                    Debug.Log(children.Count);
//                    for (int i = 0; i < children.Count; i++)
//                    {
//                        children[i].index = nowChatIndex;
//                        children[i].indexLabel.text = nowChatIndex.ToString();
//                        nowChatIndex++;
//                        askChatNode.Enqueue(children[i]);
//                    }
//                    while (askChatNode.Count > 0)
//                    {
//                        if (askChatNode.Count == 0) is_ConditionNowOk = true;
//                        SortChildAndIndex(askChatNode.Peek(), nowChatIndex);
//                        askChatNode.Dequeue();
//                    }
//                    break;
//                }
//            }*/
//        }
//    }
}