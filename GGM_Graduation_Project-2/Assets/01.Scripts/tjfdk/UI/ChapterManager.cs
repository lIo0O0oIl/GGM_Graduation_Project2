using ChatVisual;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChapterManager : UI_Reader
{
    public Chapter previousChapter;
    public Chapter nowChapter;
    public int chatIndex;
    public int roundIndex;

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
        Debug.Log(chatSystem.FindMember(who).name + " " + FindChapter(name).showName);
        chatSystem.FindMember(who).chapterName = FindChapter(name).showName;
        if (previousChapter.saveLocation != ESaveLocation.NotSave)
        {
            if (chatSystem.FindMember(who).name == chatSystem.FindMember(previousChapter.saveLocation.ToString()).name)
            {
                Debug.Log("인물 변동 없어서 바로 진행");
                Chapter(name);
            }
        }
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

        //MemberChat member = chatSystem.FindMember(FindChapter(name).saveLocation.ToString());
        //chapterManager.Chapter(member.name, member.chapterName);

        // 만약... 전 챕터랑 같은 인물이면... 그냥 choice어쩌고 들릴 필요 없이 chapter 불러주기
        //FindChapter(chatContainer.NowChapter.nextChapterName).showName);
    }

    public void Chapter(string name)
    {
        nowChapter = FindChapter(name);

        Debug.Log(chatSystem.FindMember(nowChapter.saveLocation.ToString()).name + " " + chatSystem.currentMemberName);
        if (chatSystem.FindMember(nowChapter.saveLocation.ToString()).name == chatSystem.currentMemberName)
        {
            if (nowChapter.isCan == false)
            {
                chatContainer.NowChapter = nowChapter;
                chatContainer.NowChapter.isChapterEnd = false;
                roundIndex = 0;

                StartCoroutine(InputCChat(false, nowChapter.chat));
            }
        }
        else
            Debug.Log("현재 챕터 주인과 채팅창의 주인이 다름");
    }

    public void EndChapter()
    {
        //chatSystem.FindMember(nowChapter.saveLocation.ToString()).chapterName = "";
        nowChapter.isChapterEnd = true;
        previousChapter = nowChapter;

        if (nowChapter.is_nextChapter)
        {
            NextChapter(nowChapter.nextChapterName);
        }
        else
            Debug.Log("다음 챕터 없담!");
    }

    public IEnumerator InputCChat(bool isReply, List<Chat> chats)
    {
        chatIndex = 0;
        while (chatIndex != chats.Count)
        {
            if (chatIndex < chats.Count)
            {
                if (chats[chatIndex].isCan == false)
                {
                    chatSystem.InputChat(chats[chatIndex].state, nowChapter.saveLocation,
                        chats[chatIndex].type, chats[chatIndex].face, chats[chatIndex].text, true);
                    if (chats[chatIndex].textEvent.Count > 0)
                    {
                        foreach (EChatEvent evt in chats[chatIndex].textEvent)
                        {
                            switch (evt)
                            {
                                case EChatEvent.Vibration:
                                    break;
                                case EChatEvent.Round:
                                    {
                                        string fileName = nowChapter.round[roundIndex];
                                        FileT file = fileSystem.FindFile(fileName);
                                        fileSystem.AddFile(file.fileType, file.fileName, file.fileParentName);
                                        break;
                                    }
                                case EChatEvent.Camera:
                                    break;
                            }

                            roundIndex++;
                        }
                    }
                    chatIndex++;
                    yield return new WaitForSeconds(1.5f);
                }
                else
                    yield return null;
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
        chatIndex = 0;
        if (asks != null)
        {
            while (chatIndex != asks.Count)
            {
                if (asks[chatIndex].isChange)
                    chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                        EChatType.Question, asks[chatIndex].ask, true, InputCChat(true, asks[chatIndex].reply), 
                        (() => { AddChapter(asks[chatIndex].ask, asks[chatIndex].changeName); }));
                else
                    chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                        EChatType.Question, asks[chatIndex].ask, true, InputCChat(true, asks[chatIndex].reply));                                            
                chatIndex++;
            }
        }

        if (nowChapter.lockAskAndReply.Count > 0)
            InputLockQuestion(nowChapter.lockAskAndReply);
    }

    public void InputLockQuestion(List<LockAskAndReply> locks)
    {
        chatIndex = 0;
        if (locks != null)
        {
            while (chatIndex != locks.Count)
            {
                chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                    EChatType.LockQuestion, locks[chatIndex].ask, true, null);
                chatIndex++;
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