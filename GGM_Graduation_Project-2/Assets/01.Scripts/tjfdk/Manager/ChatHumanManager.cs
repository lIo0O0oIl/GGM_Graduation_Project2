using ChatVisual;
using System.Collections.Generic;
using UnityEngine;

public class ChatHumanManager : MonoBehaviour
{
    public ChatContainer chatContainer;

    public float nextChatTime = 1f;         // when load next chat time
    public bool is_ChatStart = false;

    private List<Node> currentNodes = new List<Node>();
    public MemberProfile currentMember, chapterMember;
    public Node currentNode;
    public List<string> checkEvidence = new List<string>();

    //public Coroutine chatting;
    public bool isChattingRunning = false;



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
                    chatTree.nodeList[j].is_readThis = false;

                    if (chatTree.nodeList[j] is ChatNode chatNode)
                    {
                        for (int k = 0; k < chatNode.childList.Count; k++)
                        {
                            if (chatNode.childList[k] == null)
                            {
                                chatNode.childList.RemoveAt(k);
                            }
                        }
                    }

                    if (chatTree.nodeList[j] is ConditionNode conditionNode)
                        conditionNode.is_Unlock = false;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GoChat();
    }

    public void GoChat()
    {
        if (is_ChatStart)
        {
            if (currentMember.name == chapterMember.name)
                NextChat();
        }
    }

    public void NextChat()
    {
        Debug.Log("실행은되는중.ㄴ");

        // node list
        var children = chatContainer.GetChatTree().GetChild(currentNode);

        foreach (Node node in children)
        {
            // when node is ChatNode
            if (node is ChatNode chatNode)
            {
                if (chatNode.is_readThis == false)
                {
                    // load next
                    currentNode = children[0];
                    // check
                    children[0].is_readThis = true;

                    // chat event
                    GameManager.Instance.chatSystem.SettingChat
                        (currentMember, chatNode.state, chatNode, chatNode.face, chatNode.textEvent);

                    // Input chat
                    GameManager.Instance.chatSystem.InputChat
                        (currentMember.name, chatNode.state, chatNode.type, chatNode.face, chatNode.chatText, true);

                    // start chatting
                    IsChat(true);

                    currentMember.currentNode = chatNode;
                }
            }
            else if (node is AskNode askNode)
            {
                if (askNode.is_readThis == false && askNode.is_UseThis == false)
                {
                    // load next
                    currentNode = askNode.parent;
                    // check
                    askNode.is_readThis = true;

                    // save member's currentNode
                    currentMember.currentNode = askNode;
                    currentMember.currentAskNode = askNode;

                    // Input chat
                    GameManager.Instance.chatSystem.InputQuestion(currentMember.name, false, askNode);

                    // save question list
                    currentMember.questions.Add(askNode);

                    // stop chatting 
                    IsChat(false);

                    //is_ChatStop = true;
                }
            }
            else if (node is ConditionNode conditionNode)
            {
                // when all question conditon
                if (conditionNode.is_AllQuestion)
                {
                    // when exist ask
                    if (conditionNode.asks.Count > 0)
                    {
                        // when all question is use
                        if (conditionNode.Checkk())
                        {
                            // load next
                            currentNode = conditionNode;
                            // check
                            conditionNode.is_UseThis = true;
                        }
                        else
                        {
                            // when default question
                            if (conditionNode.asks[0].parent is ChatNode)
                            {
                                // load next
                                currentNode = (conditionNode.asks[0].parent as ChatNode);
                            }
                            // when lock question
                            else if (conditionNode.asks[0].parent is ConditionNode)
                            {
                                ConditionNode parent = conditionNode.asks[0].parent as ConditionNode;

                                // load next
                                currentNode = parent.parentList[0];
                            }
                        }
                    }
                }
                // when lock condition
                else if (conditionNode.is_SpecificFile)
                {
                    // when lock is unlock
                    if (checkEvidence.Contains(conditionNode.fileName))
                    {
                        // load next
                        currentNode = conditionNode;
                    }
                    else
                    {
                        // start chatting
                        IsChat(true);
                    }
                }
                // when lock question condition
                else if (conditionNode.is_LockQuestion)
                {
                    // get ask
                    AskNode ask = conditionNode.childList[0] as AskNode;

                    if (ask.is_readThis == false && ask.is_UseThis == false)
                    {
                        // load next
                        conditionNode.childList[0].is_readThis = true;

                        // save question list
                        currentMember.questions.Add(ask);

                        // Input Question
                        GameManager.Instance.chatSystem.InputQuestion
                            (currentMember.name, !conditionNode.is_Unlock, ask);

                        // stop chatting
                        IsChat(false);
                    }

                    // load next
                    currentNode = conditionNode.parentList[0];
                }
            }
        }
    }

    public void StartChat(string name)      // HG
    {
        // change current member
        currentMember = GameManager.Instance.chatSystem.FindMember(name);

        // change member name ui
        GameManager.Instance.chatSystem.ChangeMemberName
            (GameManager.Instance.chatSystem.FindMember(currentMember.name).name);

        // save current name
        chatContainer.currentName = name;
        // get current nodes
        currentNodes = chatContainer.GetChatTree().nodeList;

        if (currentNodes[0] is RootNode rootNode)
        {
            // when current member's currentNode isn't null
            // load next
            if (currentMember.currentNode != null)
            {
                currentNode = currentMember.currentNode;
            }
            else
                currentNode = rootNode;
        }

        IsChat(true);
    }

    public void IsChat(bool isChat)
    {
        if (isChat)
        {
            is_ChatStart = true;

            // when difference chapter member and current memer 
            if (chapterMember.name != currentMember.name)
                GameManager.Instance.chatSystem.MemberList(true);
            else
                GameManager.Instance.chatSystem.MemberList(false);
        }
        else
        {
            is_ChatStart = false;

            GameManager.Instance.chatSystem.MemberList(true);
        }
    }
}