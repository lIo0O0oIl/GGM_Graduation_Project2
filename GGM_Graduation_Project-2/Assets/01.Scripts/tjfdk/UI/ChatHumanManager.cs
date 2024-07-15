using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class ChatHumanManager : MonoBehaviour
{
    public ChatContainer chatContainer;

    public bool is_ChatStart = false;

    private List<Node> nowNodes = new List<Node>();
    public string nowHumanName;        // Name of the human you're talking to
    public MemberProfile nowHuman, chapterHuman;
    public Node currentNode;
    public List<string> checkEvidence = new List<string>();

    //public Coroutine chatting;
    public bool isChattingRunning = false;

    //public void SetChatSpeed(float value) => changeHumanTime = value;
    public void SetWheelSpeed(float value) => GameManager.Instance.chatSystem.wheelSpeed = value;

    private void Start()
    {
        for (int i = 0; i < chatContainer.chatTrees.Count; i++)
        {
            ChatTree chatTree = chatContainer.chatTrees[i];
            for (int j = 0; j < chatTree.nodeList.Count; j++)
            {
                if (chatTree.nodeList[j] != null)
                {
                    chatTree.nodeList[j].is_UseThis = false;
                    chatTree.nodeList[j].test_isRead = false;

                    if (chatTree.nodeList[j] is ConditionNode conditionNode)
                        conditionNode.is_Unlock = false;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            GoChat();
    }

    public void GoChat()
    {
        if (is_ChatStart)
        {
            if (nowHuman.name == chapterHuman.name)
                NextChat();
        }
    }

    public void NextChat()
    {
        // node list
        var children = chatContainer.GetChatTree().GetChild(currentNode);

        foreach (Node node in children)
        {
            if (node is ChatNode chatNode)
            {
                if (chatNode.test_isRead == false)
                {
                    // load next
                    currentNode = children[0];
                    children[0].test_isRead = true;

                    // chat event
                    GameManager.Instance.chatSystem.SettingChat(nowHuman, chatNode.state, chatNode, chatNode.face, chatNode.textEvent);

                    // Input chat
                    GameManager.Instance.chatSystem.InputChat(nowHumanName, chatNode.state,
                        chatNode.type, chatNode.face, chatNode.chatText, true);
                }
            }
            else if (node is AskNode askNode)
            {
                if (askNode.test_isRead == false && askNode.is_UseThis == false)
                {
                    currentNode = askNode.parent;
                    nowHuman.memCurrentNode = askNode.parent;

                    GameManager.Instance.chatSystem.InputQuestion(nowHumanName, false, askNode);
                    GameManager.Instance.chatHumanManager.StopChatting();
                    nowHuman.questions.Add(askNode);
                    askNode.test_isRead = true;
                }
            }
            else if (node is ConditionNode conditionNode)
            {
                // When allquestion condition
                if (conditionNode.is_AllQuestion)
                {
                    if (conditionNode.is_AllQuestion)
                    {
                        if (conditionNode.asks.Count > 0)
                        {
                            if (conditionNode.Checkk())
                            {
                                conditionNode.is_UseThis = true;
                                currentNode = conditionNode;
                            }
                            else
                            {
                                if (conditionNode.asks[0].parent is ChatNode)
                                {
                                    currentNode = (conditionNode.asks[0].parent as ChatNode);
                                }
                                else if (conditionNode.asks[0].parent is ConditionNode)
                                {
                                    ConditionNode parent = conditionNode.asks[0].parent as ConditionNode;
                                    currentNode = parent.parentList[0];
                                }
                            }
                        }

                    }

                    //if (conditionNode.asks.Count > 0)
                    //{
                    //    Debug.Log(conditionNode.Checkk());
                    //    // when all question is useThis true
                    //    if (conditionNode.Checkk())
                    //    {
                    //        conditionNode.is_UseThis = true;
                    //        currentNode = conditionNode;
                    //    }
                    //    else
                    //    {
                    //        // 둘 중 하나 이상함 지워야함
                    //        if ((conditionNode.asks[0].parent as ConditionNode).parentList[0] != null)
                    //            currentNode = (conditionNode.asks[0].parent as ConditionNode).parentList[0];
                    //        else
                    //            currentNode = conditionNode.asks[0].parent;
                    //    }
                    //}
                    //else
                    //    Debug.LogError("not exist question, but exist question condition");
                }
                else if (conditionNode.is_SpecificFile)
                {
                    if (checkEvidence.Contains(conditionNode.fileName))     // 이 증거를 봤던 것이라면
                    {
                        currentNode = conditionNode;
                    }
                    else
                    {
                        StartChatting();
                        GameManager.Instance.chatSystem.OnOffMemberListButton(true);
                    }
                }
                else if (conditionNode.is_LockQuestion)
                {
                    AskNode ask = conditionNode.childList[0] as AskNode;

                    if (conditionNode.childList[0].test_isRead == false && conditionNode.childList[0].is_UseThis == false)
                    {
                        if (conditionNode.is_Unlock)
                        {
                            GameManager.Instance.chatSystem.InputQuestion(nowHumanName, false, ask);
                        //StartChatting();
                        }
                        else
                        {
                            GameManager.Instance.chatSystem.InputQuestion(nowHumanName, true, ask);
                        }
                            GameManager.Instance.chatHumanManager.StopChatting();
                        nowHuman.questions.Add(ask);
                        conditionNode.childList[0].test_isRead = true;
                    }

                    currentNode = conditionNode.parentList[0];
                }
            }
        }
    }

    public void ChatResetAndStart(string name)      // HG
    {
        nowHumanName = name;
        nowHuman = GameManager.Instance.chatSystem.FindMember(nowHumanName);

        // top name changed
        GameManager.Instance.chatSystem.ChangeMemberName
            (GameManager.Instance.chatSystem.FindMember(nowHumanName).name);

        chatContainer.nowName = name;
        nowNodes = chatContainer.GetChatTree().nodeList;

        if (nowNodes[0] is RootNode rootNode)
        {
            if (nowHuman.memCurrentNode != null)          // return human
            {
                currentNode = nowHuman.memCurrentNode;
            }
            else
                currentNode = rootNode;
        }

        StartChatting();
    }

    public void StartChatting()
    {
        //if (chapterHuman.name != nowHuman.name)
        //    is_ChatStart = false;
        //else
        is_ChatStart = true;

        if (chapterHuman.name != nowHuman.name)
            GameManager.Instance.chatSystem.OnOffMemberListButton(true);
        else
            GameManager.Instance.chatSystem.OnOffMemberListButton(false);
    }

    public void StopChatting()
    {
        is_ChatStart = false;
        GameManager.Instance.chatSystem.OnOffMemberListButton(true);
    }
}
