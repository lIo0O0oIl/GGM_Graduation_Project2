using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct Chapters
{
    public ChatSO chatSO;
    public AskAndReplySO[] askAndReplySO;
}

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    public Chapters[] chats;      // 쳇팅 SO들을 넣어줌.
    public int nowChatIndex = 0;            // 쳇팅들
    public int nowLevel = 0;            // 현재 쳇팅의 레벨
    private bool is_choosing;       // 선택지가 있어서 선택중일 때
    private bool is_Player;      // 플레이어가 말하는 중인가
    private int replyCount = 0;     // first 일 때 2번 하고 바로 나가게 해주는 것.

    private string selectCriminal;

    private WaitForSeconds delay = new WaitForSeconds(0.75f);       // 대화 딜레이 시간

    private void Start()
    {
        Instance = this;
        selectCriminal = chats[chats.Length - 1].askAndReplySO[0].ask.reply;
        StartChatting(5);           // 가장 처음은 0으로 해두기
    }

    private void OnDisable()        // SO 초기화
    {
        foreach (var chats in chats)
        {
            foreach(var ask in chats.askAndReplySO)
            {
                ask.ask.is_used = false;        // 사용하지 않았음.
            }
        }
        chats[0].chatSO.is_Ask = true;      // 첫번째꺼에 질문이 있음. 하드코딩.
    }

    public void StartChatting(int index)
    {
        if (index != 0) StopCoroutine(StartChattingCoroutine(index - 1));       // 전에꺼 꺼주기
        StartCoroutine(StartChattingCoroutine(index));          // 지금꺼 시작
    }

    private IEnumerator StartChattingCoroutine(int index)
    {
        int chatLenght = chats[index].chatSO.chat.Length;       // 쳇팅들의 길이
        Debug.Log(chatLenght);
        int askLenght = chats[index].askAndReplySO.Length == 0 ? 0 : 1;      // 질문들의 개수
        nowLevel = index;
        for (int i = 0; i < chatLenght + askLenght; i++)
        {
            Chapter();
            yield return delay;
        }
    }

    public void Chapter()
    {
        if (is_choosing == false && nowChatIndex < chats[nowLevel].chatSO.chat.Length)        // 선택중이 아니라면
        {
            bool state = chats[nowLevel].chatSO.chat[nowChatIndex].state == ChatState.Assistant ? false : true;       // 조수인지 플레이어(형사) 인지 형변환. 1이 플레이어임.
            TextBox.Instance.InputText(state, chats[nowLevel].chatSO.chat[nowChatIndex].text);
            nowChatIndex++;
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
            else
            {
                nowLevel++;
                nowChatIndex = 0;
                Chapter();
            }
        }
    }

    public void answerr(string str)     // 버튼을 클릭했을 때
    {
        Debug.Log(str);
        foreach (var replySO in chats[nowLevel].askAndReplySO)
        {
            if (replySO.ask.ask == str)
            {
                replySO.ask.is_used = true;
                StartCoroutine(ReplyPrint(replySO.ask.GetReplys()));
            }
        }
    }

    private IEnumerator ReplyPrint(string[] replys)     // first 질문들일 때 대답하도록
    {
        if (chats[nowLevel].askAndReplySO[0].askName == "First")
        {
            replyCount++;
            Debug.Log(replyCount);
            if (replyCount == 2)
            {
                yield return delay;
                TextBox.Instance.InputText(false, replys[0]);       // "~~~을 옮겨드렸어요"
                yield return delay;
                string remainder = null;
                foreach (var noUse in chats[nowLevel].askAndReplySO)
                {
                    if (noUse.ask.is_used == false)
                    {
                        remainder = noUse.ask.ask;          // "~~~부터 줘"
                        remainder = remainder.Substring(0, remainder.IndexOf("부터"));
                    }
                }
                TextBox.Instance.InputText(false, $"그리고 나머지 {remainder}도 옮겨드렸어요.");
                replyCount = 0;
                is_choosing = false;
                chats[nowLevel].chatSO.is_Ask = false;
                StartChatting(1);
                yield break;
            }
        }

        yield return delay;
        foreach(var text in replys)
        {
            TextBox.Instance.InputText(false, text);
            yield return delay;     // 딜레이 위치 판단하기!
            if (text == selectCriminal)
            {
                Debug.Log("범인찾기!");     // 범인을 찾는 것 적어주기!
                TextBox.Instance.InputText(false, $"네. 그럼 {000}씨를 구속하겠습니다.");
                yield break;
            }
        }

        is_choosing = false;
        Chapter();
    }
}
