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
        private Chapters[] chapters;     // 챕터들
        public Chapters[] Chapters { get { return chapters; } set { chapters = value; } }

#if UNITY_EDITOR
        //public Node CreateNode(Type type)
        //{
        //    var node = Activator.CreateInstance(type) as Node;
        //    node.guid = GUID.Generate().ToString();

        //    nodes.Add(node);

        //    AssetDatabase.SaveAssets();

        //    return node;
        //}
#endif
    }
}
