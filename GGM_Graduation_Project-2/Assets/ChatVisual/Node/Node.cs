using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public abstract class Node
    {
        [HideInInspector] public string guid;       // Unity Object Unique identifiers
        [HideInInspector] public Vector2 position;
        public int index;
        public Label indexLabel;
    }
}
