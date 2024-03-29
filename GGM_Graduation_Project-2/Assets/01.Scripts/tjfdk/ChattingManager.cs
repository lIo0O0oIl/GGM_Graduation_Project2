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
    private int replyCount = 0;     // first 일 때 2번 하고 바로 나가게 해주는 것.
    private bool is_SelectCriminalTiming = false;

    private string selectCriminal;

    private WaitForSeconds delay = new WaitForSeconds(0.75f);       // 대화 딜레이 시간

    private void Start()
    {
        Instance = this;
        selectCriminal = chats[chats.Length - 1].askAndReplySO[0].ask.GetReplys()[0];
        chattingHumanName.text = chats[0].whoSO.humanName;
        StartChatting(0);           // 가장 처음은 0으로 해두기
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
        if (chattingHumanName.text != chats[index].whoSO.humanName)     // 이름이 다르면
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
                    assistantChatList[i].gameObject.SetActive(true);
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
            if (nowLevel == 1 && nowChatIndex == 2) InvisibleFileManager.Instance.ShowRoundFile("1-1");     // 학교 파일 보내주기
            bool state = chats[nowLevel].chatSO.chat[nowChatIndex].state == ChatState.Assistant ? false : true;       // 조수인지 플레이어(형사) 인지 형변환. 1이 플레이어임.
            TextBox.Instance.InputText(state, chats[nowLevel].chatSO.chat[nowChatIndex].text, false);
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
        }
    }

    public void answer(string str)     // 버튼을 클릭했을 때
    {
        if (is_SelectCriminalTiming)
        {
            string name = str.Substring(4, 3);      // 3글자
            TextBox.Instance.InputText(false, $"네. 그럼 {name}씨를 구속하겠습니다.");
            StartCoroutine(End(name));
            return;     // 끝끝
        }

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
            string name = replys[0].Substring(0, replys[0].IndexOf(' '));
            UpLoadFile(null, name);       // 파일을 업로드 하기 위해서, 보고서, 용의자, 피해자가 들어옴

            replyCount++;

            if (replyCount == 2)
            {
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

                name = remainder.Substring(0, remainder.IndexOf(' '));
                UpLoadFile(null, name);

                replyCount = 0;
                is_choosing = false;
                chats[nowLevel].chatSO.is_Ask = false;

                StartChatting(1);
                yield break;
            }
        }

        yield return delay;
        foreach (var text in replys)
        {
            TextBox.Instance.CurrentSpeechColorChange();
            TextBox.Instance.InputText(false, text);
            yield return delay;     // 딜레이 위치 판단하기!
            if (text == selectCriminal)
            {
                Debug.Log("범인찾기!");     // 범인을 찾는 것 적어주기!
                is_SelectCriminalTiming = true;     // 지금은 범인을 찾는 것.
                TextBox.Instance.InputText(true, $"범인은 이수연씨야");
                TextBox.Instance.InputText(true, $"범인은 황준원씨야");
                TextBox.Instance.InputText(true, $"범인은 곽현석씨야.");
                TextBox.Instance.InputText(true, $"범인은 이태광씨야.");
                yield break;
            }
        }

        is_choosing = false;
        Chapter();
    }

    private void UpLoadFile(string round, string name = null)
    {
        if (name != null)
        {
            switch (name)
            {
                case "보고서":
                    InvisibleFileManager.Instance.ShowRoundFile("1-2");
                    InvisibleFileManager.Instance.ShowRoundFile("1-2-1");
                    break;
                case "용의자":
                    InvisibleFileManager.Instance.ShowRoundFile("1-3");
                    break;
                case "피해자":     // 유품
                    InvisibleFileManager.Instance.ShowRoundFile("1-2");
                    InvisibleFileManager.Instance.ShowRoundFile("1-2-2");
                    break;
                default:
                    Debug.LogError("없는 이름입니다.");
                    break;
            }
            return;
        }
        InvisibleFileManager.Instance.ShowRoundFile(round);
    }

    private IEnumerator End(string answer)
    {
        yield return delay;
        Debug.Log(answer + "로 끝남");
        SelectSuspectManager.Instance.Select(answer);
    }
}
