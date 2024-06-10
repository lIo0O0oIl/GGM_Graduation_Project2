using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class RootNode : Node
    {
        public Node child;

        public string showName;
        public List<string> loadFileNameList = new List<string>();

        public int nowIndex;        // Indexes to continue chatting
    }
}
