using ChatVisual;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChapterManager : UI_Reader
{
    public Chapter nowChapter;
    public int index;

    public Chat FindChat(string text)
    {
        Chat eventChat = null;

        foreach (Chat chat in nowChapter.chat)
        {
            Debug.Log(chat.text);
            if (chat.text == text)
                eventChat = chat;
        }

        if (eventChat != null)
            return eventChat;

        foreach (AskAndReply ask in nowChapter.askAndReply)
        {
            foreach (Chat chat in ask.reply)
            {
                Debug.Log(chat.text);
                if (chat.text == text)
                    eventChat = chat;
            }
        }

        return eventChat;
    }

    public void AddChapter(string who, string name)
    {
        //if (chatSystem.FindMember(who).chapterName == "")
            chatSystem.FindMember(who).chapterName = FindChapter(name).showName;
        //else
        //    Debug.Log("이미 챕터가 있어서 추가할 수 없음");
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

    public void NextChapter(string name)
    {
        AddChapter(FindChapter(name).saveLocation.ToString(), name);

        MemberChat member = chatSystem.FindMember(FindChapter(name).saveLocation.ToString());
        // 해당 인물에게 챕터를 읽자
        if (member.chapterName != "")
        {
            //Debug.Log("담 챕터 레츠고");
            chapterManager.Chapter(member.name, member.chapterName);
        }
        else
            Debug.Log("챕터가 비어있음");

        // 만약... 전 챕터랑 같은 인물이면... 그냥 choice어쩌고 들릴 필요 없이 chapter 불러주기
        //FindChapter(chatContainer.NowChapter.nextChapterName).showName);
    }

    public void Chapter(string who, string name)
    {
        nowChapter  = FindChapter(name);
        chatContainer.NowChapter = nowChapter;
        chatContainer.nowChaptersIndex = 0;

        //if (nowChapter.askAndReply.Count > 0 || nowChapter.lockAskAndReply.Count > 0)
        //    StartCoroutine(InputCChat(true, nowChapter.chat));
        //else
            StartCoroutine(InputCChat(false, nowChapter.chat));
    }

    public void EndChapter()
    {
        //chatSystem.FindMember(nowChapter.saveLocation.ToString()).chapterName = "";

        if (nowChapter.is_nextChapter)
            NextChapter(nowChapter.nextChapterName);
        else
            Debug.Log("다음 챕터 없담!");
    }

    public IEnumerator InputCChat(bool isReply, List<Chat> chats)
    {
        index = 0;
        while (index != chats.Count)
        {
            if (index < chats.Count)
            {
                if (chats[index].isCan == false)
                {
                    chatSystem.InputChat(chats[index].state, nowChapter.saveLocation,
                        chats[index].type, chats[index].face, chats[index].text, true);
                    index++;
                    yield return new WaitForSeconds(1.5f);
                }
                else
                {
                    yield return null;
                    Debug.Log("아직 활성화되지 않음 " + chats[index].text);
                }
            }
        }

        // 채팅 용도로 들어온거면 위에 다 해주고 질문 있으면 질문 넣어주기
        if (isReply == false && nowChapter.askAndReply.Count > 0)
            InputQQuestion(nowChapter.askAndReply);
        // 질문에 대한 대답 용도로 들어온거면 대답 다 해주고 챕터 끝내주기
        else
            EndChapter();
    }

    public void InputQQuestion(List<AskAndReply> asks)
    {
        index = 0;
        if (asks != null)
        {
            while (index != asks.Count)
            {
                chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                    EChatType.Question, asks[index].ask, true, InputCChat(true, asks[index].reply));                                            
                index++;
            }
        }

        if (nowChapter.lockAskAndReply.Count > 0)
            InputLockQuestion(nowChapter.lockAskAndReply);
    }

    public void InputLockQuestion(List<LockAskAndReply> locks)
    {
        index = 0;
        if (locks != null)
        {
            while (index != locks.Count)
            {
                chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                    EChatType.LockQuestion, locks[index].ask, true, null);
                index++;
            }
        }
    }

    //public IEnumerator InputReply(List<Chat> replies)
    //{
    //    int i = 0;
    //    if (replies != null)
    //    {
    //        while (i != replies.Count)
    //        {
    //            Debug.Log(replies[i].ask + " 질문");
    //            chatSystem.InputChat(chatContainer.NowChapter.saveLocation,
    //                EChatType.Text, replies[i].ask, true, InputReply(replies[i]));
    //            i++;
    //            yield return null;
    //        }
    //    }

    //    yield return null;
    //}
}
