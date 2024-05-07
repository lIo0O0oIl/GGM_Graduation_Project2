using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public Node rootNode;
        public Hierarchy hierarchy = new Hierarchy();        // 노드가 생성되는 곳.
        public List<Node> nodes = new List<Node>();         // 노드 리스트

        [SerializeField]
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } }

        [SerializeField]
        private Chapter[] chapters;     // 챕터들
        public Chapter[] Chapters { get { return chapters; } set { chapters = value; } }

        public void ChangeNowChapter(int index)
        {
            nowChapter = chapters[index];
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.SaveAssets();
        }
#endif

        public void AddChild(Node parent, Node child)
        {
            var rootNode = parent as RootNode;      //부모가 루트이면
            if (rootNode != null)
            {
                rootNode.child = child;
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.child = child;
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                return;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = child;
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();

            var rootNode = parent as RootNode;
            if (rootNode != null && rootNode.child != null)
            {
                children.Add(rootNode.child);
            }

            var askNode = parent as AskNode;
            if (askNode != null && askNode.child != null)
            {
                children.Add(askNode.child);
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null && lockAskNode.child != null)
            {
                children.Add(lockAskNode.child);
            }

            return children;
        }
    }
}
