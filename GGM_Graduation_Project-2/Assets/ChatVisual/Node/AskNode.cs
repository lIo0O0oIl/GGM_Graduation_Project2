using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class AskNode : Node
    {
        public string ask;  
        public List<Chat> reply = new List<Chat>();
    }
}
