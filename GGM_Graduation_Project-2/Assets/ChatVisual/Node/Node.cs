using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public abstract class Node
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        public int index;
        public Label indexLabel;

        //public virtual Node Clone()     // 노드 복사하는 함수
        //{
        //    return Instantiate(this);
        //}

        protected abstract void OnStart();
    }
}
