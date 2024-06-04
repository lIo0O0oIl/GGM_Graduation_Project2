using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ConditionNode : Node
    {
        public string condition;
        public bool conditionCheck = false;

        public Conditions conditionClass;
    }

    public abstract class Conditions
    {

    }

    public class AskAllUse : Conditions
    {
        public List<AskNode> askNodes = new List<AskNode>();
        
        public bool AllAskUseCheck()
        {
            foreach (var i in askNodes)
            {
                if (!i.is_UseThis) return false;
            }
            return true;
        }
    }
}
