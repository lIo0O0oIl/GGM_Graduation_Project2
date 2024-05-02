using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    [System.Serializable]
    public class Hierarchy     // 바로 보일 것
    {
        public Vector3 moveToPosition;
        public Vector2 lastSpotPosition;
        //public LayerMask whatIsEnemy;
        //public GameObject testGame;
    }

    public abstract class Node
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Hierarchy blackBoard;
        [TextArea] public string description;

  /*      public virtual Node Clone()     // 노드 복사하는 함수
        {
            return Instantiate(this);
        }*/

        protected abstract void OnStart();
    }
}
