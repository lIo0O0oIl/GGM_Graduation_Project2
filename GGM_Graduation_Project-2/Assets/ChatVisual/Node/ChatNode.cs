using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ChatNode : Node
    {
        public List<Node> child = new List<Node>();

        public EChatState state;     // 말하는 것의 타입
        public string text;        // 말 하는 것.
        public EFace face;       // 말 할 때의 표정
        public List<EChatEvent> textEvent = new List<EChatEvent>();

        protected override void OnStart()
        {
        }
    }
}
