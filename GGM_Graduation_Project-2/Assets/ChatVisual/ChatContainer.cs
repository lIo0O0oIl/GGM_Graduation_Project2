using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();         // 노드 리스트

        public int nowChaptersIndex;        // 챕터 인덱스
        public int nowChatIndex;            // 쳇팅 인덱스

        [Space(30)]

        [SerializeField]        // 그냥 지금 어떤 챕터인지 볼려고 있는 것. 참조 복사로 넣어줬음.
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } set { nowChapter = value; } }

        [SerializeField]
        private List<Chapter> mainChapter = new List<Chapter>();     // 챕터들
        public List<Chapter> MainChapter { get { return mainChapter; } set { mainChapter = value; } }

        public void ChangeNowChapter(int index)
        {
            if (mainChapter.Count <= index)
            {
                Debug.Log("새로 만들어주기");
                mainChapter.Add(new Chapter());
            }
            nowChaptersIndex = index;
            nowChapter = mainChapter[index];        // 참조 복사
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);        // 리스트에 추가

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.SaveAssets();
            SortIndex();
        }
#endif

        public void AddChild(Node parent, Node child)
        {
            Debug.Log($"선 연결, parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;      //부모가 루트이면
            if (rootNode != null)
            {
                rootNode.child = child;
                SortIndex();
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.child.Add(child);
                SortIndex();
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                SortIndex();
                return;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = child;
                SortIndex();
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            var rootNode = parent as RootNode;      //부모가 루트이면
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

        public void SortIndex()     // 인덱스를 정렬한다.
        {
            nowChatIndex = 1;

            bool is_firstChatEnd = false;
            List<Node> askNodes = new List<Node>();     // 질문 담은 곳
            int askIndex = 0;

            nodes.ForEach(n =>
            {
                var children = GetChildren(n);
                if (children.Count == 0) askIndex++;
                children.ForEach(c =>
                {
                    if (c is ChatNode == false)     // 챗팅노드가 아니면 첫번째챗팅은 끝남.
                    {
                        is_firstChatEnd = true;
                        askNodes.Add(c);

                        AskNode askNode = c as AskNode;
                        if (askNode != null)
                        {
                            askNode.reply.Clear();
                        }

                        LockAskNode lockAskNode = c as LockAskNode;
                        if (lockAskNode != null)
                        {
                            lockAskNode.reply.Clear();
                        }
                    }
                    else if (c is ChatNode && is_firstChatEnd)
                    {
                        // 질문 대답 쳇팅들 나옴.
                        AskNode askNode = askNodes[askIndex] as AskNode;
                        if (askNode != null)
                        {
                            ChatNode chatNode = c as ChatNode;
                            if (chatNode != null)
                            {
                                Chat chat = new Chat();
                                chat.text = chatNode.text;
                                chat.state = chatNode.state;
                                chat.face = chatNode.face;
                                chat.textEvent = chatNode.textEvent;
                                askNode.reply.Add(chat);
                            }
                        }

                        LockAskNode lockAskNode = askNodes[askIndex] as LockAskNode;
                        if (lockAskNode != null)
                        {
                            ChatNode chatNode = c as ChatNode;
                            if (chatNode != null)
                            {
                                Chat chat = new Chat();
                                chat.text = chatNode.text;
                                chat.state = chatNode.state;
                                chat.face = chatNode.face;
                                chat.textEvent = chatNode.textEvent;
                                lockAskNode.reply.Add(chat);
                            }
                        }
                    }

                    c.index = nowChatIndex;
                    c.indexLabel.text = nowChatIndex.ToString();
                    nowChatIndex++;
                });
            });
        }

        private void SortChild(Node node)
        {
            // DFS 로 쭉 가다가 BFS 로 질문들 모두 받은 다음에 질문에 대답들에 대해서 DFS 로 쭉 해서 인덱스 밀어주기
        }
    }
}
