using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class LockAskNode : Node
    {
        public List<string> evidence = new List<string>();
        public string ask;
        public List<Chat> reply = new List<Chat>();
        public bool is_UseThis;
    }
}
