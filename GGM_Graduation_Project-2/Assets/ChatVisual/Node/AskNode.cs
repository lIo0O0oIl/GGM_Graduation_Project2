using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    [Serializable]
    // Only players can ask
    public class AskNode : Node
    {
        public Node parent;
        public Node child;

        public string askText;
        public List<EChatEvent> textEvent = new List<EChatEvent>();
        public string LoadNextDialog;

        public EAskType askType;
    }
}
