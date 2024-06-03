using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();         // ?몃뱶 由ъ뒪??

        public int nowChaptersIndex;        // 梨뺥꽣 ?몃뜳??
        public int nowChatIndex;            // 爾뉙똿 ?몃뜳??

        [Space(30)]

        [SerializeField]        // 洹몃깷 吏湲??대뼡 梨뺥꽣?몄? 蹂쇰젮怨??덈뒗 寃? 李몄“ 蹂듭궗濡??ｌ뼱以ъ쓬.
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } set { nowChapter = value; } }

        [SerializeField]
        private List<Chapter> mainChapter = new List<Chapter>();     // 梨뺥꽣??
        public List<Chapter> MainChapter { get { return mainChapter; } set { mainChapter = value; } }

        private Node askParentNode;

        public void ChangeNowChapter(int index)
        {
            if (mainChapter.Count <= index)
            {
                Debug.Log("?덈줈 留뚮뱾?댁＜湲?");
                mainChapter.Add(new Chapter());
            }
            nowChaptersIndex = index;
            nowChapter = mainChapter[index];        // 李몄“ 蹂듭궗
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);        // 由ъ뒪?몄뿉 異붽?

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.SaveAssets();
            SortChildAndIndex();
        }
#endif

        public void AddChild(Node parent, Node child)
        {
            Debug.Log($"???곌껐, parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;      //遺紐④? 猷⑦듃?대㈃
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
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            var rootNode = parent as RootNode;      //遺紐④? 猷⑦듃?대㈃
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
        }

        public List<Node> GetChildren(Node parent)
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

        public void SortChildAndIndex()        // ?몃뱶瑜??뺣젹?섍퀬 吏덈Ц ?몃뱶 ?꾨옒 梨쀭똿?대씪硫?吏덈Ц ?몃뱶???먯떇?쇰줈 ?ｌ뼱以? 
        {
            nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();    // 吏덈Ц, ?좉?吏덈Ц ?댁? 怨?
            Node nowNode = nodes[0];

            // DFS 濡?彛?媛?ㅺ? BFS 濡?吏덈Ц??紐⑤몢 諛쏆? ?ㅼ쓬??吏덈Ц????듬뱾????댁꽌 DFS 濡?彛??댁꽌 ?몃뜳??留뚮뱾?댁＜湲?
            while (nowNode != null)
            {
                var children = GetChildren(nowNode);
                if (children.Count == 1)        // 洹몃깷 爾뉙똿?몃뱶?대㈃
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
                var children = GetChildren(askQueue.Peek());
                if (children.Count > 0) AskChatSort(children[0] as ChatNode);
                askQueue.Dequeue();
            }
        }

        private void AskChatSort(ChatNode chatNode)      // 吏덈Ц???덇퀬 ?????대떦 吏덈Ц???먯떇?ㅼ쓣 ?뺣젹?섍린 ?뺣젹?섎㈃??遺紐?寃껊룄 ?ｌ뼱以섏빞??
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
            var next = GetChildren(chatNode);
            if (next.Count > 0) AskChatSort(next[0] as ChatNode);
        }
    }
}
