using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class BlackBoard
    {
        public Vector3 moveToPosition;
        public Vector2 lastSpotPosition;
        //public LayerMask whatIsEnemy;
        //public GameObject testGame;
    }

    public abstract class Node : MonoBehaviour
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public BlackBoard blackBoard;
        [TextArea] public string description;

        public virtual Node Clone()     // 노드 복사하는 함수
        {
            return Instantiate(this);
        }

        protected abstract void OnStart();
    }
}
