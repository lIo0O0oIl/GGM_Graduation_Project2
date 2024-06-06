using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    [Serializable]
    public struct TestS
    {
        public EChatState state;
        public EChatType type;
        public string text;
        public EFace face;
        public List<EChatEvent> textEvent;
    }

    public class ChatContainer : MonoBehaviour
    {
        //public List<Node> nodes = new List<Node>();         // Nodes connected to each other
        public Dictionary<string, List<Node>> HumanAndChatDictionary = new Dictionary<string, List<Node>>();

        public string nowHumanName;
        public List<Node> nowChatNodeList = new List<Node>();       // Now drawn

        public List<TestS> testList = new List<TestS>();

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            AssetDatabase.SaveAssets();

            HumanAndChatDictionary[nowHumanName].Add(node);

            return node;
        }

        public void DeleteNode(Node node)
        {
            HumanAndChatDictionary[nowHumanName].Remove(node);
            AssetDatabase.SaveAssets();
            //SortChildAndIndex();
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
            var rootNode = parent as RootNode;      //?????뼿??饔낅떽????ш퀚?녿뼸? ???猷멤꼻????????
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

        public void SortChildAndIndex()        // ??轅붽틓????곌램伊볟ㅇ????轅붽틓???壤굿?????濡ろ뜐????饔낅떽?????????彛???轅붽틓????곌램伊볟ㅇ??????諛몃마??????????????蹂κ텥???饔낅떽?????????彛???轅붽틓????곌램伊볟ㅇ????????????????????????얜Ŋ??? 
        {
            nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();    // ?饔낅떽?????????彛? ????饔낅떽?????????彛???? ??
            Node nowNode = nodes[0];

            // DFS ????????ル늉????? BFS ???饔낅떽?????????彛???饔낅떽????ш낄?뉔뇡???????썹땟戮녹??? ???嚥싲갭큔?????饔낅떽?????????彛??????????????????닿튃癲?DFS ??????????닿튃癲???轅붽틓???????饔낅떽??????????鰲????
            while (nowNode != null)
            {
                var children = GetChild(nowNode);
                if (children.Count == 1)        // ?????좊틣????????嶺뚮㉡?????轅붽틓????곌램伊볟ㅇ?????
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

        private void AskChatSort(ChatNode chatNode)      // ?饔낅떽?????????彛??????力?肉????????????饔낅떽?????????彛?????????嚥싲갭큔?????轅붽틓???壤굿??????ш끽維귞댆???轅붽틓???壤굿?????????????뼿?????棺堉?뤃???怨뚰뇠?癰????????????얜Ŋ???洹ｂ뵛??⑤베?????
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