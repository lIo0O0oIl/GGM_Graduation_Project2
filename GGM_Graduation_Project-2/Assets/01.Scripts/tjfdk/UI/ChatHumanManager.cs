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
    public ChatNode nowQuestionParent;
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
                chatTree.nodeList[j].is_UseThis = false;
                chatTree.nodeList[j].test_isRead = false;
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
        Debug.Log("코루틴 작동중");
        MemberProfile member = GameManager.Instance.chatSystem.FindMember(nowHumanName);

        while (true)
        {
            // node list
            var children = chatContainer.GetChatTree().GetChild(currentNode);

            foreach (Node node in children)
            {
                if (node is ChatNode chatNode)
                {
                    // Input chat
                    GameManager.Instance.chatSystem.InputChat(nowHumanName, chatNode.state,
                        chatNode.type, chatNode.face, chatNode.chatText, chatNode.textEvent);

                    // chat event
                    GameManager.Instance.chatSystem.SettingChat(member, chatNode, chatNode.face, chatNode.textEvent);

                    // load next
                    currentNode = children[0];
                }
                else if (node is AskNode askNode)
                {
                    if (askNode.test_isRead == false && askNode.is_UseThis == false)
                    {
                        nowQuestionParent = askNode.parent as ChatNode;

                        // 중복검사 하고 스택에 parent 넣기
                        if (member.memQuestionParent.Count == 0)
                        {
                            member.memQuestionParent.Push(nowQuestionParent);
                            Debug.Log("stack에 넣음 " + member.memQuestionParent.Count);
                        }
                        else
                        {
                            if (member.memQuestionParent.Contains(nowQuestionParent) == false)
                            {
                                member.memQuestionParent.Push(nowQuestionParent);
                                Debug.Log("stack에 넣음 " + member.memQuestionParent.Count);
                            }
                        }

                        GameManager.Instance.chatSystem.InputQuestion(nowHumanName, true, askNode.askText, askNode.textEvent,
                            askNode.LoadNextDialog, () => 
                            { 
                                    //Debug.Log(member.name + " " + askNode.askText);
                                currentNode = askNode; 
                                askNode.is_UseThis = true;
                                GameManager.Instance.chatSystem.RemoveQuestion();
                                foreach (AskNode ask in member.questions)
                                {
                                    ask.test_isRead = false;
                                }
                            });
                        GameManager.Instance.chatSystem.SettingChat(member, askNode, member.currentFace, askNode.textEvent);
                        member.questions.Add(askNode);

                        //GameManager.Instance.chatSystem.RemoveQuestion();
                        //if (member.memCurrentNode is ChatNode c)
                        //    Debug.Log(c + " 현재 인물의 최근 대화");
                        //else
                        //    Debug.Log("chatnode 자료형이 아님");
                        member.memCurrentNode = askNode.parent;
                        currentNode = askNode.parent;

                        askNode.test_isRead = true;
                    }
                }
                else if (node is ConditionNode conditionNode)
                {
                    if (conditionNode.is_AllQuestion)
                    {
                        nowQuestionParent = member.memQuestionParent.Peek();
                        if (nowQuestionParent != null)
                        {
                            var questions = chatContainer.GetChatTree().GetChild(nowQuestionParent);

                            if (conditionNode.Checkk())
                            {
                                member.memQuestionParent.Pop();
                                conditionNode.is_UseThis = true;
                                currentNode = conditionNode;
                            }
                            else
                            {
                                ////Debug.Log("오시나용...");
                                currentNode = nowQuestionParent;
                                StartChatting();
                            }
                        }
                        else
                            Debug.LogError("not exist question, but exist question condition");
                    }
                    else if (conditionNode.is_SpecificFile)
                    {
                        if (conditionNode.is_UseThis == false)
                        {
                            //Debug.Log("file trigger off");
                            nowCondition = conditionNode;
                            StopChatting();
                        }
                        else
                        {
                            //Debug.Log("file trigger on");
                            currentNode = conditionNode;
                        }
                    }
                    else if (conditionNode.is_LockQuestion)
                    {
                        AskNode ask = conditionNode.childList[0] as AskNode;

                        if (conditionNode.childList[0].test_isRead == false && conditionNode.childList[0].is_UseThis == false)
                        {
                            // 중복검사 하고 스택에 parent 넣기
                            if (member.memQuestionParent.Count == 0)
                            {
                                member.memQuestionParent.Push(nowQuestionParent);
                            }
                            else
                            {
                                if (member.memQuestionParent.Contains(nowQuestionParent) == false)
                                    member.memQuestionParent.Push(nowQuestionParent);
                            }

                            GameManager.Instance.chatSystem.InputQuestion(nowHumanName, false, ask.askText, ask.textEvent,
                                ask.LoadNextDialog, () =>
                                {
                                    currentNode = ask;
                                    conditionNode.childList[0].is_UseThis = true;
                                    GameManager.Instance.chatSystem.RemoveQuestion();
                                    foreach (AskNode ask in member.questions)
                                    {
                                        ask.test_isRead = false;
                                    }
                                });
                            GameManager.Instance.chatSystem.SettingChat(member, ask, member.currentFace, ask.textEvent);
                            member.questions.Add(ask);
                            conditionNode.childList[0].test_isRead = true;
                        }
                    }
                }

            }

            //if (children.Count == 1)            
            //{
            //    if (children[0] is ChatNode chatNode) // When a child is a ChatNode
            //    {
            //        if (chatNode.test_isRead == false) // 안 한 거라면
            //        {
            //             인풋
            //            GameManager.Instance.chatSystem.InputChat(nowHumanName, chatNode.state,
            //                chatNode.type, chatNode.face, chatNode.chatText, chatNode.textEvent);
            //             event
            //            GameManager.Instance.chatSystem.SettingChat(member, chatNode, chatNode.face, chatNode.textEvent);
            //             chat 다음 꺼 ㄱ
            //            currentNode = children[0];
            //             읽음 표시
            //            chatNode.test_isRead = true;
            //        }
            //    }
            //    else if (children[0] is ConditionNode conditionNode) // When a child is a ConditionNode
            //    {
            //         When a file Trigger condition
            //        if (conditionNode.is_SpecificFile)
            //        {
            //             아직이라면 trigger 올 때까지 정지
            //            if (conditionNode.is_UseThis == false)
            //            {
            //                Debug.Log("file trigger off");
            //                nowCondition = conditionNode;
            //                StopChatting();
            //            }
            //            else
            //            {
            //                 왔다면 conditino 아래 챗 ㄱ
            //                Debug.Log("file trigger on");
            //                currentNode = conditionNode;
            //            }
            //        }
            //         When a Check all read question condition
            //        else if (conditionNode.is_AllQuestion)
            //        {
            //            Debug.Log("모든 질문이 끝났는지 검사하는 곳에 옴");
            //            nowQuestionParent = member.memQuestionParent.Pop();
            //            Debug.Log(nowQuestionParent.chatText + " 질문을 벗어나 부모로 돌아옴!");
            //            if (nowQuestionParent != null)
            //            {
            //                bool all_Use = true;
            //                var questions = chatContainer.GetChatTree().GetChild(nowQuestionParent);
            //                foreach (AskNode ask in questions)
            //                {
            //                    if (ask.is_UseThis == false)
            //                        all_Use = false;
            //                }

            //                if (all_Use == false)
            //                    currentNode = nowQuestionParent;
            //                else
            //                {
            //                    Debug.Log("어디가 문제일까아아ㅏㅏㅏㅏㅏㅏㅏ");
            //                    conditionNode.is_UseThis = true;
            //                    currentNode = conditionNode;
            //                }
            //            }
            //            else
            //                Debug.LogError("not exist question, but exist question condition");
            //        }
            //         When a lock question condition
            //        else if (conditionNode.is_LockQuestion)
            //        {
            //             lock question condition
            //        }
            //        else
            //            Debug.Log("condition node type not select");
            //    }
            //    else if (children[0] is AskNode askNode) // When child is a AskNode
            //    {
            //        GameManager.Instance.chatSystem.OpenOtherQuestion(true);
            //        Debug.Log(member.memQuestionParent.Count + " 몇 중 질문?");
            //        if (askNode.test_isRead == false)
            //        {
            //            Debug.Log(askNode.askText);

            //            nowQuestionParent = askNode.parent as ChatNode;
            //            bool is_Lock = askNode.parent is ConditionNode ? false : true;

            //             input question
            //            GameManager.Instance.chatSystem.InputQuestion(nowHumanName, is_Lock,
            //                askNode.askText, askNode.textEvent, askNode.LoadNextDialog, () => { currentNode = askNode; askNode.is_UseThis = true; });
            //             record question
            //            member.questions.Add(askNode);
            //             event
            //            GameManager.Instance.chatSystem.SettingChat(member, askNode, member.currentFace, askNode.textEvent);

            //            member.memCurrentNode = askNode.child;
            //            GameManager.Instance.chatSystem.RemoveQuestion();
            //            askNode.test_isRead = true;
            //            currentNode = askNode.parent;
            //        }
            //    }
            //}
            //else        // When child is not a ChatNode
            //{
            //     여기까지 옴, 질문 켜주는 함수 OpenOtherQuestion 호출 위치르 ㄹ 바꿔야함!
            //    for (int i = 0; i < children.Count; i++)
            //    {
            //        if (children[i] is AskNode askNode) // When child is a AskNode
            //        {
            //            GameManager.Instance.chatSystem.OpenOtherQuestion(true);
            //            if (askNode.test_isRead == false)
            //            {
            //                Debug.Log(askNode.askText);

            //                 질문의 부모를 순서대로 겹치지 않게 넣기
            //                nowQuestionParent = askNode.parent as ChatNode;
            //                foreach (ChatNode parent in member.memQuestionParent)
            //                {
            //                    if (parent != nowQuestionParent)
            //                    {
            //                        Debug.Log(nowQuestionParent.chatText + " 를 저장함");
            //                        member.memQuestionParent.Push(nowQuestionParent);
            //                    }
            //                }
            //                bool is_Lock = askNode.parent is ConditionNode ? false : true;

            //                 input question
            //                GameManager.Instance.chatSystem.InputQuestion(nowHumanName, is_Lock,
            //                    askNode.askText, askNode.textEvent, askNode.LoadNextDialog, () => 
            //                    { 
            //                        currentNode = askNode; 
            //                        askNode.is_UseThis = true;
            //                        foreach (AskNode q in member.questions)
            //                            q.test_isRead = false;
            //                        GameManager.Instance.chatSystem.RemoveQuestion();

            //                        nowQuestionParent = askNode.parent as ChatNode;
            //                        if (member.memQuestionParent.Count == 0)
            //                        {
            //                            Debug.Log(nowQuestionParent.chatText + " 를 저장함");
            //                            member.memQuestionParent.Push(nowQuestionParent);
            //                        }
            //                        else
            //                        {
            //                            foreach (ChatNode parent in member.memQuestionParent)
            //                            {
            //                                if (parent != nowQuestionParent)
            //                                {
            //                                    Debug.Log(nowQuestionParent.chatText + " 를 저장함");
            //                                    member.memQuestionParent.Push(nowQuestionParent);
            //                                }
            //                            }
            //                        }
            //                        Debug.Log(member.memQuestionParent.Count + " 몇 중 질문?");
            //                    });
            //                 record question
            //                member.questions.Add(askNode);
            //                 event
            //                GameManager.Instance.chatSystem.SettingChat(member, askNode, member.currentFace, askNode.textEvent);

            //                askNode.test_isRead = true;
            //                currentNode = askNode.parent;
            //            }
            //        }
            //        if (children[i] is ConditionNode conditionNode) // When child is a ConditionNode
            //        {
            //            if (conditionNode.is_AllQuestion)
            //            {
            //                Debug.Log("모든 질문이 끝났는지 검사하는 곳에 옴");
            //                nowQuestionParent = member.memQuestionParent.Pop();
            //                if (nowQuestionParent != null)
            //                {
            //                    bool all_Use = true;
            //                    var questions = chatContainer.GetChatTree().GetChild(nowQuestionParent);
            //                    foreach (AskNode ask in questions)
            //                    {
            //                        if (ask.is_UseThis == false)
            //                            all_Use = false;
            //                    }

            //                    if (all_Use == false)
            //                    {
            //                        currentNode = nowQuestionParent;
            //                        GameManager.Instance.chatSystem.OpenOtherQuestion(true);
            //                    }
            //                    else
            //                    {
            //                        Debug.Log("어디가 문제일까아아ㅏㅏㅏㅏㅏㅏㅏ");
            //                        conditionNode.is_UseThis = true;
            //                        currentNode = conditionNode;
            //                    }
            //                }
            //                else
            //                    Debug.LogError("not exist question, but exist question condition");
            //            }
            //            else if (conditionNode.is_SpecificFile)
            //            {
            //                if (conditionNode.checkClass.Check())
            //                {
            //                    Debug.Log(conditionNode.childList.Count + "얘가 불러와짐!!");
            //                    children = chatContainer.GetChatTree().GetChild(conditionNode);
            //                    conditionNode.is_UseThis = true;
            //                }
            //                else
            //                {

            //                }

            //                 아직이라면 trigger 올 때까지 정지
            //                if (conditionNode.is_UseThis == false)
            //                {
            //                    Debug.Log("file trigger off");
            //                    nowCondition = conditionNode;
            //                    StopChatting();
            //                }
            //                else
            //                {
            //                     왔다면 conditino 아래 챗 ㄱ
            //                    Debug.Log("file trigger on");
            //                    currentNode = conditionNode;
            //                }
            //            }
            //            else if (conditionNode.is_LockQuestion)
            //            {
            //                 lock question condition
            //            }
            //            else
            //                Debug.Log("tlqkftlqlfktl");
            //        }
            //    }
            //}

            yield return new WaitForSeconds(1f);
        }
    }

    public void ChatStart(string name)      // HG
    {
        Debug.Log("대화 시작");
        nowHumanName = name;
        MemberProfile member = GameManager.Instance.chatSystem.FindMember(nowHumanName);
        chatContainer.nowName = name;
        nowNodes = chatContainer.GetChatTree().nodeList;
        if (nowNodes[0] is RootNode rootNode)
        {
            if (member.memCurrentNode != null)
            {
                currentNode = member.memCurrentNode;
                ChatNode test = currentNode as ChatNode;
                Debug.Log(test.chatText + " 시작 지점");
            }
            else
                currentNode = rootNode;

            //nowIndex = rootNode.nowIndex;
        }

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
