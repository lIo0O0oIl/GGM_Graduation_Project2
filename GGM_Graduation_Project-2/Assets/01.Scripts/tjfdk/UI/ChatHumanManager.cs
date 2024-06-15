using ChatVisual;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatHumanManager : UI_Reader
{
    private ChatContainer chatContainer;

    public float changeHumanTime = 1f;       // A time when humans change

    private List<Node> nowNodes = new List<Node>();
    private string nowHumanName;        // Name of the human you're talking to
    private int currentIndex = 0;
    private bool is_ChatStart = false;

    public void AddHuman(string who)     // HG
    {
        GameManager.Instance.chatSystem.AddMember(who);
    }

    private void Update()
    {
        if (is_ChatStart)
        {
            var children = chatContainer.GetChild(nowNodes[currentIndex]);

            if (children.Count == 1 && children[0] is ChatNode)            // When a child is a ChatNode
            {
                // Outputting Metabolism

                // Emotion Changes

                // Handing Chat Event (Camera, vibration, fileLoad)

                //UI_Reader.Instance.chatSystem.InputChat()

                currentIndex++;
            }
            else        // When child is not a ChatNode
            {
                // // 筌욌뜄揆??援?鈺곌퀗援????욧볼??됱뱽 ????욧볼??됱뱽 ??                for (int i = 0; i < children.Count; i++)
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
                            // 鈺곌퀗援?紐껊굡????鈺곌퀗援???袁⑥┷??볤탢??겹늺 ??쇱벉 ?紐껊굡 ??곸㉡.
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

    public void ChatStart(string name)      // HG
    {
        nowHumanName = name;
        //chatContainer.HumanAndChatDictionary[nowHumanName]
        is_ChatStart=true;
    }
}
