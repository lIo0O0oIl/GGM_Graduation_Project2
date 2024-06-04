using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class LockAskNode : Node
    {
        public Node child;

        public List<string> evidence = new List<string>();
        public string ask;        // 臾쇱쓣 ???덈뒗 ?좏깮吏
        public List<Chat> reply = new List<Chat>();     // 洹몄뿉 ?????듬뱾
        public bool is_UseThis;     // ?ъ슜?덈뒗吏
    }
}
