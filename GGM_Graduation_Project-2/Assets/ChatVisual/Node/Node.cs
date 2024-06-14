using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public abstract class Node
    {
        public string guid;       // Unity Object Unique identifiers
        public Vector2 position;

        public int index;
        public Label indexLabel;

        public bool is_UseThis;
    }
}
