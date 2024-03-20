using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingManager : Singleton<ChattingManager>
{
    [SerializeField]
    private List<DialogueSO> chapterSO = new List<DialogueSO>();

    public int currentChapter = 0;
    public int currentStep = 0;

    public bool isChoice = false;
    public bool isFunc;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Chapter();
    }

    public void Chapter()
    {
        isFunc = false;

        if (isChoice == false)
        {
            TextBox.Instance.InputText(false, chapterSO[currentChapter].temp[currentStep].text);

            if (chapterSO[currentChapter].temp[currentStep].next.Count == 0)
                currentStep++;
            else
            {
                foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)
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

    public void answer(string str)
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
