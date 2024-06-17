using ChatVisual;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatHumanManager : UI_Reader
{
    public ChatContainer chatContainer;

    public float changeHumanTime = 1f;       // A time when humans change
    private float currentTime = 0f;

    private List<Node> nowNodes = new List<Node>();
    public string nowHumanName;        // Name of the human you're talking to
    //private int nowIndex = 0;
    public Node currentNode;
    private bool is_ChatStart = false;

    public Coroutine chatting;
    public bool isChattingRunning = false;

    private void Start()
    {
        //chatting = StartCoroutine(ReadChat());
        //StopCoroutine(chatting);
    }

    public void AddHuman(string who)     // HG
    {
        GameManager.Instance.chatSystem.AddMember(who);
    }

    private void Update()
    {
        //currentTime += Time.deltaTime;
        //if (is_ChatStart && currentTime >= changeHumanTime)
        //{
        //    currentTime = 0f;

        //    // node list
        //    var children = chatContainer.GetChild(nowNodes[nowIndex]);

        //    if (children.Count == 1)            // When a child is a ChatNode
        //    {
        //        if (children[0] is ChatNode chatNode)
        //        {
        //            Debug.Log(chatNode.chatText);
        //            ChatNode chat = (ChatNode)children[0];

        //            // Outputting Metabolism
        //            GameManager.Instance.chatSystem.InputChat(nowHumanName, chat.state, 
        //                chat.type, chat.face, chat.chatText, chat.textEvent);
                    
        //            // Emotion Changes
        //                // working to InputChat
        //            // Handing Chat Event (Camera, vibration, fileLoad)
        //                // working to InputChat

        //            nowIndex++;
        //        }
        //    }
        //    else        // When child is not a ChatNode
        //    {
        //        for (int i = 0; i < children.Count; i++)
        //        {
        //            if (children[i] is AskNode askNode)
        //            {
        //                // Handling questions
        //                Debug.Log(askNode.askText);
        //                AskNode ask = (AskNode)children[i];
        //                bool is_Lock = ask.parent is ConditionNode ? true : false;
        //                //GameManager.Instance.chatSystem.InputQuestion(nowHumanName, is_Lock,
        //                //    ask.askText, , ask.textEvent, );
        //                nowIndex++;
        //            }
        //            else if (children[i] is ConditionNode conditionNode)
        //            {
        //                if (conditionNode.checkClass.Check())
        //                {
        //                    children = chatContainer.GetChild(nowNodes[nowIndex]);
        //                    conditionNode.is_UseThis = true;
        //                    continue;
        //                }
        //            }
        //        }
        //    }

        //}
    }

    public IEnumerator ReadChat()
    {
        MemberProfile member = GameManager.Instance.chatSystem.FindMember(nowHumanName);

        while (true) 
        {
            // node list
            var children = chatContainer.GetChatTree().GetChild(currentNode);

            if (children.Count == 1)            // When a child is a ChatNode
            {
                if (children[0] is ChatNode chatNode)
                {
                    if (chatNode.is_UseThis == false)
                    {
                        GameManager.Instance.chatSystem.InputChat(nowHumanName, chatNode.state,
                            chatNode.type, chatNode.face, chatNode.chatText, chatNode.textEvent);
                        // event
                        GameManager.Instance.chatSystem.SettingChat(member, chatNode, chatNode.face, chatNode.textEvent);

                        currentNode = children[0];
                        chatNode.is_UseThis = true;
                    }
                }
            }
            else        // When child is not a ChatNode
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].is_UseThis == false)
                    {
                        if (children[i] is AskNode askNode) // When child is a AskNode
                        {
                            //Debug.Log(askNode.askText);

                            bool is_Lock = askNode.parent is ConditionNode ? true : false;

                            // input question
                            GameManager.Instance.chatSystem.InputQuestion(nowHumanName, is_Lock,
                                askNode.askText, askNode.textEvent, () => { currentNode = askNode; });
                            // record question
                            member.questions.Add(askNode);
                            // event
                            GameManager.Instance.chatSystem.SettingChat(member, askNode, member.currentFace, askNode.textEvent);

                            askNode.is_UseThis = true;
                        }
                        else if (children[i] is ConditionNode conditionNode) // When child is a ConditionNode
                        {
                            if (conditionNode.checkClass.Check())
                            {
                                children = chatContainer.GetChatTree().GetChild(conditionNode);
                                conditionNode.is_UseThis = true;
                            
                                continue;
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    public void ChatStart(string name)      // HG
    {
        Debug.Log("대화 시작");
        nowHumanName = name;
        chatContainer.nowName = name;
        nowNodes = chatContainer.GetChatTree().nodeList;
        if (nowNodes[0] is RootNode rootNode)
        {
            currentNode = rootNode;
            //nowIndex = rootNode.nowIndex;
        }

        Debug.Log("대화 다시 호출");
        StartChatting();
    }

    public void StartChatting()
    {
        if (isChattingRunning == false)
        {
            chatting = StartCoroutine(ReadChat());
        }

        isChattingRunning = true;
    }

    public void StopChatting()
    {
        if (isChattingRunning)
        {
            if (chatting != null)
                StopCoroutine(chatting);
        }

        isChattingRunning = false;
    }
}
