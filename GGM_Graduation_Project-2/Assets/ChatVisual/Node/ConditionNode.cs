using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    // Situations in which a ConditionNode will be disarmed
    // 1. if all question nodes associated with you are unlocked
    // 2. when a specific file is opened

    public class ConditionNode : Node
    {
        public List<Node> parentList = new List<Node>();
        public Node child;
    }
}
