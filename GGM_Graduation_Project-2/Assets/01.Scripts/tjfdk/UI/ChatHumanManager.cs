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
    private float currentTime = 0f;

    private List<Node> nowNodes = new List<Node>();
    private string nowHumanName;        // Name of the human you're talking to
    private int nowIndex = 0;
    private bool is_ChatStart = false;

    public void AddHuman(string who)     // HG
    {
        chatSystem.AddMember(who);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (is_ChatStart && currentTime >= changeHumanTime)
        {
            currentTime = 0f;

            var children = chatContainer.GetChild(nowNodes[nowIndex]);

            if (children.Count == 1)            // When a child is a ChatNode
            {
                if (children[0] is ChatNode chatNode)
                {
                    Debug.Log(chatNode.chatText);
                // Outputting Metabolism

                // Emotion Changes

                // Handing Chat Event (Camera, vibration, fileLoad)
                //UI_Reader.Instance.chatSystem.InputChat()

                nowIndex++;
                }
            }
            else        // When child is not a ChatNode
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] is AskNode askNode)
                    {
                        // Handling questions
                        Debug.Log(askNode.askText);
                        nowIndex++;
                    }
                    else if (children[i] is ConditionNode conditionNode)
                    {
                        if (conditionNode.checkClass.Check())
                        {
                            children = chatContainer.GetChild(nowNodes[nowIndex]);
                            conditionNode.is_UseThis = true;
                            continue;
                        }
                    }
                }
            }

        }
    }

    public void ChatStart(string name)      // HG
    {
        Debug.Log("대화 시작");
        nowHumanName = name;
        nowNodes = chatContainer.HumanAndChatDictionary[nowHumanName];
        if (nowNodes[0] is RootNode rootNode)
        {
            nowIndex = rootNode.nowIndex;
        }
        is_ChatStart = true;
    }
}
