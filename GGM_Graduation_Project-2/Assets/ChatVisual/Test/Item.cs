using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class Item
    {
        public Sprite sprite;
        private VisualElement root;

        public Item(VisualElement _root)
        {
            root = _root;
        }
    }
}
