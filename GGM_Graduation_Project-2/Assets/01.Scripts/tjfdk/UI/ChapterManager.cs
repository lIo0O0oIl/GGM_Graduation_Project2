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
    //Button chatButton;

    //private void OnEnable()
    //{
    //    chatButton = root.Q<Button>("ABC");
    //    chatButton.clicked += () => { Input(); };
    //}

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

    //public void Input()
    //{
    //    Chapter now = chatContainer.NowChapter;

    //    if (now != null)
    //    {
    //        if (chatContainer.nowChaptersIndex < now.chat.Count)
    //        {
    //            chatSystem.InputChat(now.chat[chatContainer.nowChatIndex].state, now.saveLocation, now.chat[chatContainer.nowChatIndex].type,
    //                now.chat[chatContainer.nowChatIndex].face, now.chat[chatContainer.nowChaptersIndex].text, true);
    //            chatContainer.nowChaptersIndex++;
    //        }
    //        else
    //        {
    //            Debug.Log("챕터 끝남");
    //            chatButton.style.display = DisplayStyle.None;
    //            chatContainer.NowChapter = null;
    //            chatSystem.FindMember(now.saveLocation.ToString()).chapterName = "";

    //            if (now.is_nextChapter)
    //                NextChapter(now.nextChapterName);
    //            else
    //                Debug.Log("다음 챕터 없담!");
    //        }
    //    }
    //}

    public IEnumerator InputCChat(bool isReply, List<Chat> chats)
    {
        int i = 0;
        while (i != chats.Count)
        {
            if (i < chats.Count)
            {
                chatSystem.InputChat(chats[i].state, nowChapter.saveLocation,
                    chats[i].type, chats[i].face, chats[i].text, true);
                i++;
                yield return new WaitForSeconds(1.5f);
            }
        }

        if (isReply == false && nowChapter.askAndReply.Count > 0)
            InputQQuestion(nowChapter.askAndReply);
        else
            EndChapter();
    }

    public void InputQQuestion(List<AskAndReply> asks)
    {
        int i = 0;
        if (asks != null)
        {
            while (i != asks.Count)
            {
                chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                    EChatType.Question, asks[i].ask, true, InputCChat(true, asks[i].reply));                                            
                i++;
            }
        }

        if (nowChapter.lockAskAndReply.Count > 0)
            InputLockQuestion(nowChapter.lockAskAndReply);
    }

    public void InputLockQuestion(List<LockAskAndReply> locks)
    {
        int i = 0;
        if (locks != null)
        {
            while (i != locks.Count)
            {
                chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                    EChatType.LockQuestion, locks[i].ask, true, null);
                i++;
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
