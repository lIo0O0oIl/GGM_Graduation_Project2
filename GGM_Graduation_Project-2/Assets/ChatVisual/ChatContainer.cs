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
                SortChildAndIndex();
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.childList.Add(child);
                SortChildAndIndex();
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                SortChildAndIndex();
                return;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.child = child;
                SortChildAndIndex();
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

        public void SortChildAndIndex()    
        {
            /*nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();   
            Node nowNode = nodes[0];

            // DFS BFS 
            while (nowNode != null)
            {
                var children = GetChild(nowNode);
                if (children.Count == 1)    
                {
                    children[0].index = nowChatIndex;
                    children[0].indexLabel.text = nowChatIndex.ToString();
                    nowChatIndex++;
                    nowNode = children[0];
                }
                else
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        askQueue.Enqueue(children[i]);

                        if (children[i] is AskNode askNode)
                        {
                            askNode.reply.Clear();
                        }

                        if (children[i] is LockAskNode lockAskNode)
                        {
                            lockAskNode.reply.Clear();
                        }

                        children[i].index = nowChatIndex;
                        children[i].indexLabel.text = nowChatIndex.ToString();
                        nowChatIndex++;
                    }
                    break;
                }
            }

            while (askQueue.Count > 0)
            {
                Debug.Log(askQueue.Peek());
                askParentNode = askQueue.Peek();
                var children = GetChild(askQueue.Peek());
                if (children.Count > 0) AskChatSort(children[0] as ChatNode);
                askQueue.Dequeue();
            }*/
        }
/*
        private void AskChatSort(ChatNode chatNode)  
        {
            if (chatNode == null) return;

            if (askParentNode is  AskNode askNode)
            {
                Chat chat = new Chat();
                chat.text = chatNode.text;
                chat.state = chatNode.state;
                chat.face = chatNode.face;
                chat.textEvent = chatNode.textEvent;
                askNode.reply.Add(chat);
            }

            if (askParentNode is LockAskNode lockAskNode)
            {
                Chat chat = new Chat();
                chat.text = chatNode.text;
                chat.state = chatNode.state;
                chat.face = chatNode.face;
                chat.textEvent = chatNode.textEvent;
                lockAskNode.reply.Add(chat);
            }

            chatNode.index = nowChatIndex;
            chatNode.indexLabel.text = nowChatIndex.ToString();
            nowChatIndex++;

            Debug.Log(chatNode.child.Count);
            var next = GetChild(chatNode);
            if (next.Count > 0) AskChatSort(next[0] as ChatNode);
        }
*/
    }
}