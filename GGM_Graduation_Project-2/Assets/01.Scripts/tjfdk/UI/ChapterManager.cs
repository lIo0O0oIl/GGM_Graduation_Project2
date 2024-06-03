using ChatVisual;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : UI_Reader
{
    public void AddChapter(string who, string name)
    {
        Debug.Log("챕터 붙여줌 : " + who + " " + name);
        if (chatSystem.FindMember(who).chapterName == "")
            chatSystem.FindMember(who).chapterName = FindChapter(name).showName;
        else
            Debug.Log("이미 챕터가 있어서 추가할 수 없음");
    }

    public Chapter FindChapter(string name)
    {
        foreach (Chapter chapter in chatContainer.MainChapter)
        {
            if (chapter.showName == name)
                return chapter;
        }

        Debug.Log("챕터 찾기 실패");
        return null;
    }

    public void NextChapter()
    {
        //AddChapter(FindChapter(chatContainer.NowChapter.nextChapterName).saveLocation.ToString(),
        //FindChapter(chatContainer.NowChapter.nextChapterName).showName);
    }

    public void Chapter(string name)
    {
        Chapter chapter = FindChapter(name);
        var chats = chapter.chat;

        //AddChapter(FindChapter(chapter.nextChapterName).saveLocation.ToString(), chapter.nextChapterName);
        StartCoroutine(Chatda(chats, chapter.askAndReply, chapter.lockAskAndReply, chapter));
    }

    int i;
    private IEnumerator Chatda(List<Chat> chats, List<AskAndReply> asks, List<LockAskAndReply> lockasks, Chapter chapter)
    {
        Debug.Log("코루틴 진입 " + chats.Count);
        i = 0;
        //for (int i = 0; i < chats.Count; i++)
        while (i != chats.Count)
        {
            if (i < chats.Count)
            {
                Debug.Log(chats[i].text + " 대사");

                // 여기서 StartCoroutine(Chatda(chats, chapter.askAndReply, chapter.lockAskAndReply, chapter)); 이거 널 띄움
                chatSystem.InputChat(chats[i].state, chapter.saveLocation,
                    chats[i].type, chats[i].face, chats[i].text, true);
                i++;
                yield return new WaitForSeconds(1.5f);
            }
        }

        //i = 0;
        ////for (int i = 0; i < asks.Count; i++)
        //while (i != asks.Count)
        //{
        //    Debug.Log(asks[i].ask + " 질문");
        //    chatSystem.InputQuestion(chapter.saveLocation, 
        //        EChatType.Question, asks[i].ask, true);
        //    yield return new WaitForSeconds(1.5f);
        //}

        //i = 0;
        ////for (int i = 0; i < lockasks.Count; i++)
        //while (i != lockasks.Count)
        //{
        //    Debug.Log(lockasks[i].ask + " 잠긴질문");
        //    chatSystem.InputQuestion(chapter.saveLocation,
        //        EChatType.LockQuestion, lockasks[i].ask, true);
        //    yield return new WaitForSeconds(1.5f);
        //}

        //yield break;
    }
}
