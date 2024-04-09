using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    [Header("ChattinggContainer")]
    public GameObject chatContainer;        // 쳇팅들 담긴 곳임.
    public TMP_Text chattingHumanName;
    [HideInInspector]
    public List<GameObject> assistantChatList = new List<GameObject>();    // 조수와 나눈 대화는 저장해주기
    private Chapters[] chapters;      // 쳇팅 SO들을 넣어줌.
    public Chapters[] Chapters {  get { return chapters; } set { chapters = value; } }

    [Header("ChatDelay")]       // 쳇팅 딜레이 관련
    public float delayTime = 0.75f;
    private float currentTime = 0.0f;
    private bool is_Chatting = false;       // 챗팅을 하는 중이라면

    [Header("ChatCount")]       // 지금 쳇팅이 얼마나 진행되었는지
    [HideInInspector]
    public int nowLevel = 0;            // 현재 쳇팅의 레벨
    private int nowChatIndex = 0;            // 현재 쳇팅 인덱스

    [Header("Ask")]     // 물어보는 것 관련
    private int askLenght = 0;
    private bool is_Choosing;       // 선택지가 있어서 선택중일 때. 멈춰있는 시간을 말하는 것.
    private bool is_AskChat;
    private int nowAskLevel = 0;        // 지금 질문의 레벨   
    private int nowAskChatIndex = 0;        // 지금 대답의 인덱스
    private List<string> notUseAskList = new List<string>();

    private void Start()
    {
        Instance = this;
    }

    private void Update()       // 쳇팅 시스템
    {
        if (is_Chatting && !is_Choosing)
        {
            currentTime += Time.deltaTime;
            if (currentTime > delayTime || Input.GetMouseButtonDown(0))     // 왼쪽 버튼을 눌렀다면
            {
                if (is_AskChat) AskChapter();       // 질문에 대한 답을 출력함.
                else Chapter();

                currentTime = 0.0f;
            }
        }
    }

    public void StartChatting(int index)
    {
        nowChatIndex = 0;
        nowAskChatIndex = 0;
        nowLevel = index;

        // 쳇팅창 정보 설정해주기
        if (chattingHumanName.text != chapters[index].who)     // 다른 사람과 대화를 하는 것이라면
        {
            // 지금까지 있던 대화 다 지워주기
            for (int i = 0; i < chatContainer.transform.childCount; i++)
            {
                chatContainer.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (chattingHumanName.text != "조수")      // 용의자랑 대화한 내역이였다면 대화 내역을 파일에 png 로 저장하고 대화 내역은 모두 지운다. 그리고 조수의 대화다 켜주기
            {
                Debug.Log("용의자 대화 내역 사진식으로 저장해주기!");
                for (int i = 0; i < assistantChatList.Count; i++)
                {
                    if (assistantChatList[i].gameObject != null)
                    {
                        assistantChatList[i].gameObject.SetActive(true);
                    }
                }
            }

            chattingHumanName.text = chapters[index].who;      // 이름 넣어주기
        }

        int chatLenght = chapters[index].chat.Length;       // 쳇팅들의 길이
        askLenght = chapters[index].askAndReply.Length;      // 질문들의 개수

        is_Chatting = true;
    }

    private void Chapter()
    {
        if (is_Choosing == false && nowChatIndex < chapters[nowLevel].chat.Length)        // 선택중이 아니라면
        {
            bool state = false;       // 조수인지 플레이어(형사) 인지 형변환. 1이 플레이어임.
            switch (chapters[nowLevel].chat[nowChatIndex].state)
            {
                case ChatState.Other:
                    state = false;
                    break;
                case ChatState.Me:
                    state = true;
                    break;
                case ChatState.Ask:
                    for (int i = 0; i < askLenght; i++)
                    {
                        TextBox.Instance.InputText(true, chapters[nowLevel].chat[nowChatIndex].text, true);
                        notUseAskList.Add(chapters[nowLevel].chat[nowChatIndex].text);            // 질문들 추가
                        nowChatIndex++;
                    }
                    is_Choosing = true;
                    return;
                case ChatState.LoadNext:
                    Debug.LogError("아직 만들지 않는 LoadNext 예요.");
                    return;     // 아예 돌려
                default:
                    Debug.LogError($"{chapters[nowLevel].chat[nowChatIndex].state} 는(은) 없는 유형이예요!");
                    break;
            }
            TextBox.Instance.InputText(state, chapters[nowLevel].chat[nowChatIndex].text, false);
            nowChatIndex++;
        }
        else
        {
            is_Chatting = false;
            //Debug.LogError($"이게 왜 나와, 선택중? : {is_Choosing}, 지금 어디야? : {nowLevel}, 지금 채팅은? : {nowChatIndex}");
        }
    }

    public void answer(string str)     // 버튼을 클릭했을 때
    {
        TextBox.Instance.CurrentSpeechColorChange();
        for (int i = 0; i < notUseAskList.Count; i++)
        {
            if (notUseAskList[i] == str)
            {
                nowAskLevel = i;
                nowAskChatIndex = 0;
                notUseAskList.RemoveAt(i);

                is_AskChat = true;
                is_Choosing = false;
            }
        }
    }

    private void AskChapter()
    {
        if (nowAskChatIndex < chapters[nowLevel].askAndReply[nowAskLevel].reply.Length)
        {
            TextBox.Instance.InputText(false, chapters[nowLevel].askAndReply[nowAskLevel].reply[nowAskChatIndex]);
            nowAskChatIndex++;
        }
        else
        {
            is_AskChat = false;

            if (notUseAskList.Count == 0)       // 더 질문할 것이 없으면
            {
                is_Choosing = false;
                return;
            }

            for (int i = 0; i < notUseAskList.Count; i++)
            {
                TextBox.Instance.InputText(true, notUseAskList[i], true);
            }
            is_Choosing = true;
        }

    }

    public void ChangeDelaySpeed(float _value)
    {
        delayTime = _value;
    }       // 쳇팅 딜레이 시간 변경

    private void UpLoadFile(string round)
    {
        InvisibleFileManager.Instance.ShowRoundFile(round);
    }       // 파일을 업로드 해주는 함수
}
