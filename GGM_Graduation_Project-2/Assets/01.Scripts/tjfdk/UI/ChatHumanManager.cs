using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatHumanManager : UI_Reader
{
    public ChatContainer chatContainer;

    public float changeHumanTime = 1f;       // A time when humans change
    private float currentTime = 0f;

    private List<Node> nowNodes = new List<Node>();
    public string nowHumanName;        // Name of the human you're talking to
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

    }

    public IEnumerator ReadChat()
    {
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

                        // 以묐났寃???섍퀬 ?ㅽ깮??parent ?ｊ린
                        if (member.memQuestionParent.Count == 0)
                        {
                            member.memQuestionParent.Push(nowQuestionParent);
                            Debug.Log("stack???ｌ쓬 " + member.memQuestionParent.Count);
                        }
                        else
                        {
                            if (member.memQuestionParent.Contains(nowQuestionParent) == false)
                            {
                                member.memQuestionParent.Push(nowQuestionParent);
                                Debug.Log("stack???ｌ쓬 " + member.memQuestionParent.Count);
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
                        //    Debug.Log(c + " ?꾩옱 ?몃Ъ??理쒓렐 ???);
                        //else
                        //    Debug.Log("chatnode ?먮즺?뺤씠 ?꾨떂");
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
                                ////Debug.Log("?ㅼ떆?섏슜...");
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
                            // 以묐났寃???섍퀬 ?ㅽ깮??parent ?ｊ린
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

            yield return new WaitForSeconds(1f);
        }
    }

    public void ChatStart(string name)      // HG
    {
        Debug.Log("????쒖옉");
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
                Debug.Log(test.chatText + " ?쒖옉 吏??");
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
