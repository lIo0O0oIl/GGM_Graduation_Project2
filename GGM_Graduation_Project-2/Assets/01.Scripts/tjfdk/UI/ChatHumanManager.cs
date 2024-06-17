using ChatVisual;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public ConditionNode nowCondition;
    private bool is_ChatStart = false;

    public Coroutine chatting;
    public bool isChattingRunning = false;

    private void Start()
    {
        for (int i = 0; i < chatContainer.chatTrees.Count; i++)
        {
            ChatTree chatTree = chatContainer.chatTrees[i];
            for (int j = 0; j < chatTree.nodeList.Count; j++)
            {
                chatTree.nodeList[i].is_UseThis = false;
            }
        }
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
            Debug.Log("코루틴 도는 중");
            // node list
            var children = chatContainer.GetChatTree().GetChild(currentNode);

            if (children.Count == 1)            // When a child is a ChatNode
            {
                if (children[0] is ChatNode chatNode)
                {
                    //if (chatNode.is_UseThis == false)
                    //{
                        GameManager.Instance.chatSystem.InputChat(nowHumanName, chatNode.state,
                            chatNode.type, chatNode.face, chatNode.chatText, chatNode.textEvent);
                        // event
                        GameManager.Instance.chatSystem.SettingChat(member, chatNode, chatNode.face, chatNode.textEvent);

                        currentNode = children[0];
                        //chatNode.is_UseThis = true;
                    //}
                }
                else if (children[0] is ConditionNode conditionNode)
                {
                    if (conditionNode.is_UseThis)
                    {
                        Debug.Log("true로 변경됨");
                        currentNode = conditionNode;
                    }
                    else
                    {
                        nowCondition = conditionNode;
                        StopChatting();
                    }
                }

            }
            else        // When child is not a ChatNode
            {
                Debug.Log("질문 초반 진입");
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].is_UseThis == false)
                    {
                        if (children[i] is AskNode askNode) // When child is a AskNode
                        {
                            Debug.Log(askNode.askText);

                            bool is_Lock = askNode.parent is ConditionNode ? false : true;

                            // input question
                            GameManager.Instance.chatSystem.InputQuestion(nowHumanName, is_Lock,
                                askNode.askText, askNode.textEvent, askNode.LoadNextDialog, () => { currentNode = askNode; });
                            // record question
                            member.questions.Add(askNode);
                            // event
                            GameManager.Instance.chatSystem.SettingChat(member, askNode, member.currentFace, askNode.textEvent);

                            askNode.is_UseThis = true;
                            //currentNode = askNode.parent;
                        }
                        else if (children[i] is ConditionNode conditionNode) // When child is a ConditionNode
                        {
                            if (conditionNode.checkClass.Check())
                            {
                                children = chatContainer.GetChatTree().GetChild(conditionNode);
                                conditionNode.is_UseThis = true;
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1f);
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

        StartChatting();
    }

    public void StartChatting()
    {
        Debug.Log("코루틴 시작");
        if (isChattingRunning == false)
        {
            chatting = StartCoroutine(ReadChat());
        }

        isChattingRunning = true;
    }

    public void StopChatting()
    {
        Debug.Log("코루틴 중단");
        if (isChattingRunning)
        {
            if (chatting != null)
                StopCoroutine(chatting);
        }

        isChattingRunning = false;
    }
}
