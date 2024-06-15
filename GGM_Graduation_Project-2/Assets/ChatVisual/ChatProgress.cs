using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ChatProgress : MonoBehaviour
    {
        public ChatContainer chatContainer;

        public float changeHumanTime = 1f;       // A time when humans change

        private List<Node> nowNodes = new List<Node>();
        private string nowHumanName;        // Name of the human you're talking to
        private int currentIndex = 0;

        public void InitChatProgress(string _nowHumanName)
        {
            nowHumanName = _nowHumanName;
            nowNodes = chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName];
        }

        private void Update()
        {
            
        }

        // Change human
        public void ChangeHuman(string _changeHumanName)
        {
            nowNodes = chatContainer.HumanAndChatDictionary[_changeHumanName];
            if (nowNodes[0] is RootNode rootNode)
            {
                currentIndex = rootNode.nowIndex;       // Change index
            }
        }

        // Proceed with chat
        public void NextChat()
        {
            var children = chatContainer.GetChild(nowNodes[currentIndex]);

            if (children.Count == 1 && children[0] is ChatNode)            // When a child is a ChatNode
            {
                // Outputting Metabolism

                // Emotion Changes

                // Handing Chat Event (Camera, vibration, fileLoad)

                currentIndex++;
            }
            else        // When child is not a ChatNode
            {
                // // 嶺뚯쉶?꾣룇??????브퀗?쀦뤃?????㏓낵???깅굵 ?????㏓낵???깅굵 ??                for (int i = 0; i < children.Count; i++)
                {
                    /*if (children[i] is AskNode askNode)
                    {
                        // Handling questions
                        currentIndex++;
                    }
                    else if (children[i] is ConditionNode conditionNode)
                    {
                        if (conditionNode.is_UseThis)
                        {
                            // ?브퀗?쀦뤃?筌뤾퍓援?????브퀗?쀦뤃???熬곣뫁???蹂ㅽ깴??寃밸듆 ???깅쾳 ?筌뤾퍓援???怨멥돘.
                            if (conditionNode.checkClass.Check())
                            {
                                currentIndex++;
                                NextChat();
                            }
                        }
                    }*/
                }
            }
        }
    }
}
