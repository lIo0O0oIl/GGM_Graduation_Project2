using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class AskNode : Node
    {
        public Node child;

        public string ask;        // 물을 수 있는 선택지
        public List<Chat> reply = new List<Chat>();     // 그에 대한 대답들
        public bool is_UseThis;     // 사용했는지

        protected override void OnStart()
        {
        }
    }
}
