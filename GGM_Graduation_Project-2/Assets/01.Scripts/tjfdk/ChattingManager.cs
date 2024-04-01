using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Chapters
{
    public ChatSO chatSO;
    public AskAndReplySO[] askAndReplySO;
    public WhoSO whoSO;     // 누군데?
}

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    [Header("ChattinggRoom")]
    public GameObject chatContainer;        // 쳇팅들 담긴 곳임.
    public TMP_Text chattingHumanName;
    [HideInInspector]
    public List<GameObject> assistantChatList = new List<GameObject>();    // 조수와 나눈 대화는 저장해주기

    [Space(25)]

    public Chapters[] chats;      // 쳇팅 SO들을 넣어줌.

    private int nowChatIndex = 0;            // 쳇팅들
    [HideInInspector]
    public int nowLevel = 0;            // 현재 쳇팅의 레벨
    private bool is_choosing;       // 선택지가 있어서 선택중일 때
    private bool is_SelectCriminalTiming = false;

    private int studentChatCount = 0;       // 처음 학생과의 대화에서 선택카운트

    public float delayTime = 0.75f;
    private WaitForSeconds delay;       // 대화 딜레이 시간
    private WaitForSeconds delay2;       // 대화 딜레이 시간

    private void Start()
    {
        Instance = this;

        delay = new WaitForSeconds(delayTime);
        delay2 = new WaitForSeconds(delayTime * 3);

        chattingHumanName.text = chats[0].whoSO.humanName;
    }

    private void OnDisable()        // SO 초기화
    {
        foreach (var chats in chats)
        {
            foreach (var ask in chats.askAndReplySO)
            {
                ask.ask.is_used = false;        // 사용하지 않았음.
            }
        }
        chats[0].chatSO.is_Ask = true;      // 첫번째꺼에 질문이 있음. 하드코딩.
    }

    public void StartChatting(int index)
    {
        nowChatIndex = 0;
        nowLevel = index;
        if (index != 0) StopCoroutine(StartChattingCoroutine(index - 1));       // 전에꺼 꺼주기
        StartCoroutine(StartChattingCoroutine(index));          // 지금꺼 시작
    }

    private IEnumerator StartChattingCoroutine(int index)
    {
        // 쳇팅창 정보 설정해주기
        //Debug.Log($"{chattingHumanName.text}, {chats[index].whoSO.humanName}");
        if (chattingHumanName.text != chats[index].whoSO.humanName)     // 다른 사람과 대화를 하는 것이라면
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

            chattingHumanName.text = chats[index].whoSO.humanName;      // 이름 넣어주기
        }

        int chatLenght = chats[index].chatSO.chat.Length;       // 쳇팅들의 길이
        int askLenght = chats[index].askAndReplySO.Length == 0 ? 0 : 1;      // 질문들의 개수
        for (int i = 0; i < chatLenght + askLenght; i++)
        {
            Chapter();
            yield return delay;
        }
    }

    private void Chapter()
    {
        if (is_choosing == false && nowChatIndex < chats[nowLevel].chatSO.chat.Length)        // 선택중이 아니라면
        {
            bool state = chats[nowLevel].chatSO.chat[nowChatIndex].state == ChatState.Other ? false : true;       // 조수인지 플레이어(형사) 인지 형변환. 1이 플레이어임.
            TextBox.Instance.InputText(state, chats[nowLevel].chatSO.chat[nowChatIndex].text, false);
            nowChatIndex++;

            if (nowLevel == 4 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)      // 첫 학생과의 대화를 끝맺음 했다면.
            {
                StartCoroutine(EndOtherChat(5));
            }

            if (nowLevel == 5)    // 일진의 정보를 요청했다면
            {
                if (nowChatIndex == 1) UpLoadFile("채팅파일");
                if (nowChatIndex == 5) UpLoadFile("강지현물품");
            }

            if (nowLevel == 7 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                StartCoroutine(EndOtherChat(8));
            }

            if (nowLevel == 8 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length - 1)
            {
                Debug.Log("부장교사물품");
                UpLoadFile("부장교사물품");
            }

            if (nowLevel == 10 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                StartCoroutine(EndOtherChat(11));
            }       // 부장과 교사 끝남

            if (nowLevel == 13 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                StartCoroutine(EndOtherChat(14));
            }       // 황준원와 대화 끝

            if (nowLevel == 15 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                Debug.Log("화면 암전");
                Debug.Log("저희 학교의 자살 사건이 이번이 처음이 아니에요...");
            }
        }
        else if (nowChatIndex >= chats[nowLevel].chatSO.chat.Length && is_choosing == false)       // 현재 쳇팅 정도를 넘었고 선택중인 상태가 아닐 때
        {
            if (chats[nowLevel].chatSO.is_Ask)        // 만약 질문이 있다면
            {
                foreach (var askSO in chats[nowLevel].askAndReplySO)      // 선택지가 있을 때 모두 출력해주기
                {
                    if (askSO.ask.is_used == false)       // 사용되지 않은 질문이였다면
                    {
                        is_choosing = true;
                        TextBox.Instance.InputText(true, askSO.ask.ask);
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"이게 왜 나와, 선택중? : {is_choosing}, 지금 어디야? : {nowLevel}, 지금 채팅은? : {nowChatIndex}");
        }
    }


    public void answer(string str)     // 버튼을 클릭했을 때
    {
        TextBox.Instance.CurrentSpeechColorChange();
        foreach (var replySO in chats[nowLevel].askAndReplySO)
        {
            if (replySO.ask.ask == str)
            {
                replySO.ask.is_used = true;
                StartCoroutine(ReplyPrint(replySO.ask.GetReplys()));
            }
        }
    }

    private IEnumerator ReplyPrint(string[] replys)     // 질문들이 들어올 때 대답하도록
    {
        if (chats[nowLevel].askAndReplySO[0].askName == "First")
        {
            string name = replys[0].Substring(0, replys[0].IndexOf(' '));
            UpLoadFile(name);       // 파일을 업로드 하기 위해서, 보고서, 학교 이 2개가 들어옴.

            yield return delay;
            TextBox.Instance.InputText(false, replys[0]);       // "~~~을 옮겨드렸어요"
            yield return delay;

            string remainder = null;
            foreach (var noUse in chats[nowLevel].askAndReplySO)
            {
                if (noUse.ask.is_used == false)     // 남는거 하나 찾기
                {
                    remainder = noUse.ask.ask;          // "~~~부터 줘"
                    remainder = remainder.Substring(0, remainder.IndexOf("부터"));
                }
            }
            TextBox.Instance.InputText(false, $"그리고 나머지 {remainder}도 옮겨드렸어요.");
            yield return delay;

            name = remainder.Substring(0, remainder.IndexOf(' '));
            UpLoadFile(name);

            is_choosing = false;
            chats[nowLevel].chatSO.is_Ask = false;

            StartChatting(1);       // 이어서 연결되는 것이기 때문에
            yield break;

        }

        yield return delay;

        foreach (var text in replys)        // 대답들 추가해주기
        {
            TextBox.Instance.InputText(false, text);
            yield return delay;     // 딜레이 위치 판단하기!
        }

        is_choosing = false;

        // 이 아래로는 답변이 몇 개 이상일 때를 적어주는 곳임.
        if (chats[nowLevel].askAndReplySO[0].askName == "StudentMeet")
        {
            StartCoroutine(EndOtherChat(3));
            yield break;
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "Student")
        {
            studentChatCount++;
            if (studentChatCount == 3)      // 3개의 질문을 했다면
            {
                Debug.Log("3개 이상");
                StartCoroutine(EndOtherChat(4));
                yield break;
            }
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "JihyeonMeet")
        {
            StartCoroutine(EndOtherChat(7));
            yield break;
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "HyeonSeokMeet")
        {
            StartCoroutine(EndOtherChat(10));
            yield break;
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "LastMeet")
        {
            StartCoroutine(EndOtherChat(15));
            yield break;
        }

        Debug.Log("여기까지 온다고?");
        Chapter();
    }

    private IEnumerator EndOtherChat(int next)
    {
        yield return delay2;
        StartChatting(next);
    }

    private void UpLoadFile(string round)
    {
        Debug.Log(round);
        switch (round)
        {
            case "초동":
            case "보고서":
                InvisibleFileManager.Instance.ShowRoundFile("보고서");
                break;
            case "학교":
            case "학교에":
                InvisibleFileManager.Instance.ShowRoundFile("학교");
                break;
            case "채팅파일":
                InvisibleFileManager.Instance.ShowRoundFile("채팅파일");
                break;
            case "강지현물품":
                InvisibleFileManager.Instance.ShowRoundFile("강지현물품");
                break;
            case "부장교사물품":
                InvisibleFileManager.Instance.ShowRoundFile("부장교사물품");
                break;
            default:
                Debug.LogError($"{round}는 없는 이름입니다.");
                break;
        }
    }
}
