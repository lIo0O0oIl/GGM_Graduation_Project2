using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    // Only players can ask
    public class AskNode : Node
    {
        public Node parent;
        public Node child;

        public string ask;
        public List<EChatEvent> textEvent = new List<EChatEvent>();
        public string LoadNextDialog;
    }
}
