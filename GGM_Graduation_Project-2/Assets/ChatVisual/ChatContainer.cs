using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public Dictionary<string, List<Node>> HumanAndChatDictionary = new Dictionary<string, List<Node>>();        // Nodes connected to each other

        public string nowHumanName;
        
        private bool is_ConditionNowOk = false;
        private int nowChatIndex = 1;       // index for Sort

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            HumanAndChatDictionary[nowHumanName].Add(node);

            return node;
        }

        public void DeleteNode(Node node)
        {
            HumanAndChatDictionary[nowHumanName].Remove(node);
        }
#endif

        public void ChangeNowChapter(string key)
        {
            nowHumanName = key;
        }

        public void AddChild(Node parent, Node child)
        {
            Debug.Log($"Connect! parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = child;
                SortChildAndIndex(HumanAndChatDictionary[nowHumanName][0], 1);
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.childList.Add(child);
                SortChildAndIndex(HumanAndChatDictionary[nowHumanName][0], 1);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                SortChildAndIndex(HumanAndChatDictionary[nowHumanName][0], 1);
                return;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.child = child;
                SortChildAndIndex(HumanAndChatDictionary[nowHumanName][0], 1);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = null;
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.childList.Remove(child);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = null;
                return;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.child = null;
            }
        }

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
            }

            return children;
        }
        
        public void SortChildAndIndex(Node startNode, int startIndex)    
        {
            Node nowNode = startNode;         // RootNode
            Queue<Node> askChatNode = new Queue<Node>();
            List<Node> children = new List<Node>();
            nowChatIndex = startIndex;

            Debug.Log("nono");
            children = GetChild(nowNode);

            // DFS BFS 
            while (children.Count > 0)
            {
                children = GetChild(nowNode);
                Debug.Log(children.Count);

                if (children.Count == 1 && children[0] is ChatNode)            // When a child is a ChatNode
                {
                    if (is_ConditionNowOk)
                    {
                        if (children[0] is ConditionNode condition)
                        {
                            /*children[0].index = nowChatIndex;
                            children[0].indexLabel.text = nowChatIndex.ToString();
                            nowChatIndex++;
                            nowNode = children[0];
                            continue;*/
                            Debug.Log(condition);
                        }
                    }
                    children[0].index = nowChatIndex;
                    children[0].indexLabel.text = nowChatIndex.ToString();
                    nowChatIndex++;
                    nowNode = children[0];
                    Debug.Log("ho");
                }
                else        // When child is not a ChatNode
                {
                    Debug.Log(children.Count);
                    for (int i = 0; i < children.Count; i++)
                    {
                        children[i].index = nowChatIndex;
                        children[i].indexLabel.text = nowChatIndex.ToString();
                        nowChatIndex++;
                        askChatNode.Enqueue(children[i]);
                    }
                    while (askChatNode.Count > 0)
                    {
                        if (askChatNode.Count == 0) is_ConditionNowOk = true;
                        SortChildAndIndex(askChatNode.Peek(), nowChatIndex);
                        askChatNode.Dequeue();
                    }
                    break;
                }
            }
        }
    }
}