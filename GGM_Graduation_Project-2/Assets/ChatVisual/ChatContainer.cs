using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();         // Nodes connected to each other

        public int nowChaptersIndex;        // 癲?甕겹끂???嶺뚮ㅎ????
        public int nowChatIndex;            // ???뺣즸???嶺뚮ㅎ????

        [Space(30)]

        [SerializeField]        // ??숆강筌?쑜??癲ル슣?????????癲?甕겹끂??嶺? ?怨뚮옩?????????덉툗 ?? 癲ル슔?蹂?덫???怨뚮옖甕곕?苡??肉??壤굿??苑묊튊踰욍볥늉甕?
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } set { nowChapter = value; } }

        [SerializeField]
        private List<Chapter> mainChapter = new List<Chapter>();     // 癲?甕겹끂???
        public List<Chapter> MainChapter { get { return mainChapter; } set { mainChapter = value; } }

        private Node askParentNode;

        public void ChangeNowChapter(int index)
        {
            if (mainChapter.Count <= index)
            {
                Debug.Log("????궈?癲ル슢???????⑥궢猷??");
                mainChapter.Add(new Chapter());
            }
            nowChaptersIndex = index;
            nowChapter = mainChapter[index];        // 癲ル슔?蹂?덫???怨뚮옖甕곕?苡?
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            AssetDatabase.SaveAssets();

            nodes.Add(node);

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
            var rootNode = parent as RootNode;      //??딅텑?癲ル슢?꾬쭗? ??룸Ŧ爾??????
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

        public void SortChildAndIndex()        // ?嶺뚮ㅎ?볠뤃???嶺뚮㉡?ｈ????寃뗏?癲ル슣??袁ｋ즵 ?嶺뚮ㅎ?볠뤃???ш끽維??癲?????????瑜곸떵?癲ル슣??袁ｋ즵 ?嶺뚮ㅎ?볠뤃?????筌???⑥???壤굿??苑묊튊? 
        {
            nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();    // 癲ル슣??袁ｋ즵, ???癲ル슣??袁ｋ즵 ??? ??
            Node nowNode = nodes[0];

            // DFS ??傭???좊읈???? BFS ??癲ル슣??袁ｋ즵??癲ル슢?꾤땟?嶺??袁⑸즵?? ???源낆쓱??癲ル슣??袁ｋ즵???????????????⑤똾留?DFS ??傭????⑤똾留??嶺뚮ㅎ????癲ル슢???????⑥궢猷??
            while (nowNode != null)
            {
                var children = GetChild(nowNode);
                if (children.Count == 1)        // ??숆강筌?쑜?????뺣즸??嶺뚮ㅎ?볠뤃?????
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

        private void AskChatSort(ChatNode chatNode)      // 癲ル슣??袁ｋ즵??????뫢??????????癲ル슣??袁ｋ즵?????筌???源낃도 ?嶺뚮㉡?ｈ?????꾨탿 ?嶺뚮㉡?ｈ???嚥??????딅텑?癲??濡ろ뜏?蹂〓쇀??壤굿??苑묊튊踰우눊????
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
    }
}
