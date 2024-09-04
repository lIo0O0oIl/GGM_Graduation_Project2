using ChatVisual;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChatHumanManager : MonoBehaviour
{
    //public ChatContainer chatContainer;

    public float nextChatTime = 1f;         // when load next chat time
    public bool is_ChatStart = false;

    //private List<NormalCHat> currentNodes = new List<NormalCHat>();
    //public string currentName;
    public MemberProfile currentMember, chapterMember;
    public NormalChat currentNode;
    public List<string> checkEvidence = new List<string>();

    //public Coroutine chatting;
    public bool isChattingRunning = false;



    private void Start()
    {
        // so 초기화 해줘야함...

        //// 값 초기화
        //for (int i = 0; i < chatContainer.chatTrees.Count; i++)
        //{
        //    ChatTree chatTree = chatContainer.chatTrees[i];
        //    for (int j = 0; j < chatTree.nodeList.Count; j++)
        //    {
        //        if (chatTree.nodeList[j] != null)
        //        {
        //            chatTree.nodeList[j].is_UseThis = false;
        //            chatTree.nodeList[j].is_readThis = false;
        //            if (chatTree.nodeList[j] is ChatNode chatNode)
        //            {
        //                for (int k = 0; k < chatNode.childList.Count; k++)
        //                {
        //                    if (chatNode.childList[k] == null)
        //                    {
        //                        chatNode.childList.RemoveAt(k);
        //                    }
        //                }
        //            }
        //            if (chatTree.nodeList[j] is ConditionNode conditionNode)
        //                conditionNode.is_Unlock = false;
        //        }
        //    }
        //}
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
            //if (currentMember.name == chapterMember.name)
                NextChat();
        }
    }

    public void NextChat()
    {
        // 해당 턴의 채팅 받아오기
        NormalChat chat = currentMember.excelChat[currentMember.currentIdx];

        // 만약 텍스트라면
        if (chat is Chat text)
            Chat(text);
        // 만약 질문이라면
        else if (chat is Question ask)
            Question(ask);
    }

    public void Chat(Chat text)
    {
        // 만약 읽지도 출력하지도 않은 글이라면 (text는 둘 다 같은 뜻으로 사용함)
        if (text.isRead == false && text.isUse == false)
        {
            // 만약 채팅에 잠금이 걸려있다면 (글이 존재한다면)
            //if (text.fileName != "" || text.fileName != null)
            if (!string.IsNullOrEmpty(text.fileName))
            {
                // file name에 파일명이 하나가 아님 파일이름1/파일이름2 라서 / 기준으로 구분해야할듯?
                if (GameManager.Instance.fileManager.FindFile(text.fileName) != null)
                {
                    // 만약 잠금 해제 조건이 충족했다면
                    if (GameManager.Instance.fileManager.FindFile(text.fileName).isRead == false)
                        return;
                }
                else
                {
                    Debug.LogError(text.fileName + " 파일이 존재하지 않음");
                    return;
                }
            }

            // 해당 채팅 체크
            text.isRead = true;
            text.isUse = true;

            // 채팅 출력
            GameManager.Instance.chatSystem.InputChat
                (currentMember.name, text.who, text.type, text.text);

            // 채팅 세팅
            GameManager.Instance.chatSystem.SettingChat
                (currentMember, text.who, text.face, text.fildLoad, text.evt);

            // 채팅 진행
            IsChat(true);

            // 다음 채팅으로
            currentMember.currentIdx++;
        }
    }

    public void Question(Question ask)
    {
        if (ask.isRead == false && ask.isUse == false)
        {
            // 읽음으로 확인
            ask.isRead = true;

            // 질문 출력
            GameManager.Instance.chatSystem.InputQuestion
                (currentMember.name, ask.type == EAskType.Lock, ask);

            // 질문 추가
            currentMember.questions.Add(ask);

            // 대화 일시정지
            IsChat(false);

            // 다음 채팅 & 채팅으로 이동
            currentMember.currentIdx++;
            currentMember.currentAskIdx++;

            if (currentMember.excelChat[currentMember.currentIdx] is Question)
                NextChat();
            //고쳐야함...
        }
    }

    public void StartChat(string name)      // HG
    {
        // 사람 찾고
        currentMember = GameManager.Instance.chatSystem.FindMember(name);

        // 사람 변경
        GameManager.Instance.chatSystem.ChangeMemberName(name);

        // 현재 노드 설정... (안 쓰는듯함)
        if (currentMember.currentIdx != 0)
            currentNode = currentMember.excelChat[currentMember.currentIdx];
        else
            currentNode = currentMember.excelChat[0];

        IsChat(true);

        //// change current member
        //currentMember = GameManager.Instance.chatSystem.FindMember(name);
        //// change member name ui
        //GameManager.Instance.chatSystem.ChangeMemberName
        //    (GameManager.Instance.chatSystem.FindMember(currentMember.name).name
        //// save current name
        //chatContainer.currentName = name;
        //// get current nodes
        //currentNodes = chatContainer.GetChatTree().nodeList;
        //if (currentNodes[0] is RootNode rootNode)
        //{
        //    // when current member's currentNode isn't null
        //    // load next
        //    if (currentMember.currentNode != null)
        //    {
        //        currentNode = currentMember.currentNode;
        //    }
        //    else
        //        currentNode = rootNode;
        //}
        //IsChat(true);
    }

    public void IsChat(bool isChat)
    {
        if (isChat)
        {
            is_ChatStart = true;

            //// when difference chapter member and current memer 
            //if (chapterMember.name != currentMember.name)
            GameManager.Instance.chatSystem.MemberList(true);
            //else
            //GameManager.Instance.chatSystem.MemberList(false);
        }
        else
        {
            is_ChatStart = false;

            GameManager.Instance.chatSystem.MemberList(true);
        }
    }
}
