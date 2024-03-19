using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingManager : Singleton<ChattingManager>
{
    // 대화 관리

    // 진행을 위한 조수의 대화 (시작점)
    // 유저의 선택 또는 질문에 대한 답
    // 
    // = 다이얼로그 제작.

    // 챕터 별로 관리? -> so 만들기?

    [SerializeField]
    private List<DialogueSO> chapterSO = new List<DialogueSO>();

    public int currentChapter = 0;
    public int currentStep = 0;

    public bool isChoice = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Chapter();
    }

    public void Chapter()
    {
        if (isChoice == false)
        {
            TextBox.Instance.InputText(false, chapterSO[currentChapter].temp[currentStep].text);

            if (chapterSO[currentChapter].temp[currentStep].next.Count == 0)
                currentStep++;
            else
            {
                foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)
                {
                    isChoice = true;
                    TextBox.Instance.InputText(true, ttt.text);
                }
            }

            //if (chapterSO[currentChapter].temp.Count >= currentStep)
            //    ChapterReset(); 
        }
    }

    public void answer(string str)
    {
        foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)
        {
            if (ttt.text == str)
            {
                foreach (test ttttt in ttt.next)
                    TextBox.Instance.InputText(false, ttttt.text);
            }
        }
        isChoice = false;
        //currentStep++;
    }

    public void ChapterReset()
    {
        currentStep = 0;
    }
}
