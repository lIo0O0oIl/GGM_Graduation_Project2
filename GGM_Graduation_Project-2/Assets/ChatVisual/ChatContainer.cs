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
        public List<Node> nowChatNodeList = new List<Node>();       // Now drawn

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

        /*public void AddChild(Node parent, Node child)
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
                chatNode.child.Add(child);
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

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = child;
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
            var rootNode = parent as RootNode;      //?????堉온??耀붾굝?????????용섯? ????룸ħ瑗????????
            if (rootNode != null)
            {
                rootNode.child = null;
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.child.Remove(child);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = null;
                return;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = null;
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

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null && lockAskNode.child != null)
            {
                children.Add(lockAskNode.child);
                return children;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null && chatNode.child.Count != 0)
            {
                children = chatNode.child;
            }

            return children;
        }

        public void SortChildAndIndex()        // ??饔낅떽?????怨뚮옩鴉딅퀫?????饔낅떽????鶯ㅺ동??????嚥▲굧?????耀붾굝??????????壤???饔낅떽?????怨뚮옩鴉딅퀫???????獄쏅챶留??????????????癰궽블뀯???耀붾굝??????????壤???饔낅떽?????怨뚮옩鴉딅퀫??????????????????????????쑩??? 
        {
            nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();    // ?耀붾굝??????????壤? ????耀붾굝??????????壤???? ??
            Node nowNode = nodes[0];

            // DFS ?????????ル뒌????? BFS ???耀붾굝??????????壤???耀붾굝?????????붾눀????????밸븶筌믩끃??? ????μ떜媛?걫?????耀붾굝??????????壤???????????????????욱뒅??DFS ???????????욱뒅????饔낅떽????????耀붾굝???????????欲꼲????
            while (nowNode != null)
            {
                var children = GetChild(nowNode);
                if (children.Count == 1)        // ?????醫딇떍????????癲ル슢??????饔낅떽?????怨뚮옩鴉딅퀫??????
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
            }
        }

        private void AskChatSort(ChatNode chatNode)      // ?耀붾굝??????????壤??????癲??됀????????????耀붾굝??????????壤??????????μ떜媛?걫?????饔낅떽????鶯ㅺ동?????????썹땟洹욌뙀???饔낅떽????鶯ㅺ동??????????????堉온?????汝뷴젆?琉????⑤슦????????????????쑩???域뱄퐘逾???ㅻ쿋?????
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
    }*/
    }
}