using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public string nowName;
        public List<ChatTree> chatTrees = new List<ChatTree>();

        public ChatTree GetChatTree()
        {
            foreach (ChatTree chatTree in chatTrees)
            {
                Debug.Log($"{chatTree.name}, {nowName}, {chatTree.name == nowName}");
                if (chatTree.name == nowName)
                    return chatTree;
            }

            return null;
        }
    }
}
