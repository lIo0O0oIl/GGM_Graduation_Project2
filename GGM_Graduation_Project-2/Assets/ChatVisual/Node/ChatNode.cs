using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ChatNode : Node
    {
        public List<Node> child = new List<Node>();

        public EChatState state;     // 留먰븯??寃껋쓽 ???
        public EChatType type;
        public string text;        // 留??섎뒗 寃?
        public EFace face;       // 留????뚯쓽 ?쒖젙
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }
}
