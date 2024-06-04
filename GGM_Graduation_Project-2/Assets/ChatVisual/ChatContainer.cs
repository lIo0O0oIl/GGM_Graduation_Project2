using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();         // ?紐껊굡 ?귐딅뮞??

        public int nowChaptersIndex;        // 筌?벤苑??紐껊쑔??
        public int nowChatIndex;            // ?얜돉???紐껊쑔??

        [Space(30)]

        [SerializeField]        // 域밸챶源?筌왖疫???堉?筌?벤苑?紐? 癰귥눖?????덈뮉 野? 筌〓챷??癰귣벊沅쀦에??節뚮선餓Ρ딆벉.
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } set { nowChapter = value; } }

        [SerializeField]
        private List<Chapter> mainChapter = new List<Chapter>();     // 筌?벤苑??
        public List<Chapter> MainChapter { get { return mainChapter; } set { mainChapter = value; } }

        private Node askParentNode;

        public void ChangeNowChapter(int index)
        {
            if (mainChapter.Count <= index)
            {
                Debug.Log("??덉쨮 筌띾슢諭??곻폒疫?");
                mainChapter.Add(new Chapter());
            }
            nowChaptersIndex = index;
            nowChapter = mainChapter[index];        // 筌〓챷??癰귣벊沅?
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);        // ?귐딅뮞?紐꾨퓠 ?곕떽?

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
            Debug.Log($"???怨뚭퍙, parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;      //?봔筌뤴몿? ?룐뫂?????
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
            var rootNode = parent as RootNode;      //?봔筌뤴몿? ?룐뫂?????
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

        public void SortChildAndIndex()        // ?紐껊굡???類ｌ졊??랁?筌욌뜄揆 ?紐껊굡 ?袁⑥삋 筌??샒???わ쭖?筌욌뜄揆 ?紐껊굡???癒?뻼??곗쨮 ?節뚮선餓? 
        {
            nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();    // 筌욌뜄揆, ?醫?筌욌뜄揆 ??? ??
            Node nowNode = nodes[0];

            // DFS 嚥?壤?揶쎛??? BFS 嚥?筌욌뜄揆??筌뤴뫀紐?獄쏆룇? ??쇱벉??筌욌뜄揆??????щ굶??????곴퐣 DFS 嚥?壤???곴퐣 ?紐껊쑔??筌띾슢諭??곻폒疫?
            while (nowNode != null)
            {
                var children = GetChildren(nowNode);
                if (children.Count == 1)        // 域밸챶源??얜돉??紐껊굡????
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

        private void AskChatSort(ChatNode chatNode)      // 筌욌뜄揆????뉙?????????筌욌뜄揆???癒?뻼??쇱뱽 ?類ｌ졊??띾┛ ?類ｌ졊??롢늺???봔筌?野껉퍓猷??節뚮선餓μ꼷鍮??
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
