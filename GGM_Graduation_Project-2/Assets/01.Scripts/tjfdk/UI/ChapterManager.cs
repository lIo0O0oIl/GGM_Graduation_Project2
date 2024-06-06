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
            if (chat.text == text)
                eventChat = chat;
        }

        if (eventChat != null)
            return eventChat;

        foreach (AskAndReply ask in nowChapter.askAndReply)
        {
            foreach (Chat chat in ask.reply)
            {
                if (chat.text == text)
                    eventChat = chat;
            }
        }

        return eventChat;
    }

    public void AddChapter(string who, string name)
    {
        chatSystem.AddMember(who);
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
        AddChapter(FindChapter(name).saveLocation.ToString(), name); //

        //MemberChat member = chatSystem.FindMember(FindChapter(name).saveLocation.ToString());
        //chapterManager.Chapter(member.name, member.chapterName);

        // 만약... 전 챕터랑 같은 인물이면... 그냥 choice어쩌고 들릴 필요 없이 chapter 불러주기
        //FindChapter(chatContainer.NowChapter.nextChapterName).showName);
    }

    public void Chapter(string name)
    {
        nowChapter = FindChapter(name);

        if (chatSystem.FindMember(nowChapter.saveLocation.ToString()).name == chatSystem.currentMemberName)
        {
            if (nowChapter.isCan == false)
            {
                chatContainer.NowChapter = nowChapter;
                chatContainer.NowChapter.isChapterEnd = false;
                roundIndex = 0;

                chatSystem.changeMemberButton.pickingMode = PickingMode.Ignore;
                chatSystem.memberList.style.display = DisplayStyle.None;

                StartCoroutine(InputCChat(false, nowChapter.chat));
            }
        }
        else
            Debug.Log("현재 챕터 주인과 채팅창의 주인이 다름");
    }

    public void EndChapter()
    {
        Debug.Log("챕터 끝");

        nowChapter.isChapterEnd = true;
        previousChapter = nowChapter;

        chatSystem.changeMemberButton.pickingMode = PickingMode.Position;
        chatSystem.isMemberListOpen = false;
        chatSystem.ChangeMember();

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
        while (chatIndex != chats.Count && chats != null)
        {
            if (chatIndex < chats.Count)
            {
                if (chats[chatIndex].isCan == false)
                {
                    if (chats[chatIndex].is_UseThis == false)
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
                        chats[chatIndex].is_UseThis = true;
                        chatIndex++;
                        yield return new WaitForSeconds(1.5f);
                    }
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
                if (asks[chatIndex].is_UseThis == false)
                {
                    int questionIndex = chatIndex;

                    if (asks[chatIndex].isChange)
                    {
                        string who = asks[chatIndex].changeWhoName.ToString();
                        string chapterName = asks[chatIndex].changeName;

                        chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                            EChatType.Question, asks[chatIndex].ask, true, InputCChat(true, asks[chatIndex].reply),
                            (() => { AddChapter(who, chapterName); asks[questionIndex].is_UseThis = true; }));
                    }
                    else
                    {
                        Debug.Log(asks[chatIndex].changeWhoName.ToString());
                        chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                            EChatType.Question, asks[chatIndex].ask, true, InputCChat(true, asks[chatIndex].reply),
                            (() => { asks[questionIndex].is_UseThis = true; }));
                    }
                }

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
                if (locks[chatIndex].is_UseThis == false)
                {
                    chatSystem.InputQuestion(chatContainer.NowChapter.saveLocation,
                        EChatType.LockQuestion, locks[chatIndex].ask, true, null);
                    chatIndex++;
                }
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
