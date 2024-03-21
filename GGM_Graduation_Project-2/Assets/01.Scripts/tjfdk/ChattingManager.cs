using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingManager : Singleton<ChattingManager>
{
    [SerializeField]
    private List<DialogueSO> chapterSO = new List<DialogueSO>();

    public ChatSO[] chats;      // 쳇팅 SO들을 넣어줌.
    public int nowChatIndex = 0;
    private bool is_choosing;       // 선택지가 있어서 선택중일 때
    private bool is_Player;      // 플레이어가 말하는 중인가

    public int currentChapter = 0;      // 지금 챕터
    public int currentStep = 0;         // 지금 챕터의 대화들

    public bool isChoice = false;       // 선택지를 고르고 있는 중일 때
    public bool isFunc;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Chapter();

        if (Input.GetKeyDown(KeyCode.C))
        {
            Chapterr();
        }
    }

    public void Chapter()
    {
        isFunc = false;

        if (isChoice == false)      // 뭘 고르고 있는 상태라면
        {
            TextBox.Instance.InputText(false, chapterSO[currentChapter].temp[currentStep].text);
            Debug.Log(chapterSO[currentChapter].temp[currentStep].text);

            if (chapterSO[currentChapter].temp[currentStep].next.Count == 0)
            {
                currentStep++;
                Debug.Log("스텝 증가");
            }
            else
            {
                foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)      // 선택지가 있을 때 모두 출력해주기
                {
                    if (ttt.isDone == false)
                    {
                        isChoice = true;
                        TextBox.Instance.InputText(true, ttt.text);
                        isFunc = true;
                    }
                }

                if (isFunc == false)
                    currentStep++;
            }
        }
    }

    public void Chapterr()
    {
        if (is_choosing == false)        // 선택중이 아니라면
        {
            bool state = chats[0].chat[nowChatIndex].state == ChatState.Assistant ? false : true;       // 조수인지 플레이어(형사) 인지 형변환. 1이 플레이어임.
            TextBox.Instance.InputText(state, chats[0].chat[nowChatIndex].text);
            nowChatIndex++;
        }
    }

    public void answer(string str)      // 선택지에서 눌린 것.
    {
        foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)
        {
            if (ttt.text == str)
            {
                ttt.isDone = true;
                foreach (test ttttt in ttt.next)
                {
                    {
                        TextBox.Instance.InputText(false, ttttt.text);
                    }
                }
                    // 현재 눌린 거 삭제 (so에서?...)ㄴ
                    // 걍 bool로 확인했는지 확인하고 전부 다 확인했다면 step++
            }
        }
        isChoice = false;

        // if 현재 눌렸을 때 ttttt가 눌린 것 밖에 없다면
    }

    public void ChapterReset()
    {
        currentStep = 0;
    }
}
