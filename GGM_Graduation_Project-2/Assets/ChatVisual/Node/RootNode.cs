using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class RootNode : Node
    {
        public ChatTree parent;
        public Node child;

        private void OnValidate()
        {
            Debug.Log("change");
            if (!string.IsNullOrEmpty(showName))
            {
                parent.humanName = showName;
            }
        }

        public string showName;
        public List<string> loadFileNameList = new List<string>();

        public int nowIndex;        // Indexes to continue chatting
    }
}
