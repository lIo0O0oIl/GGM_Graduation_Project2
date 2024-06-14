using ChatVisual;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChapterManager : UI_Reader
{
   // public Chapter previousChapter;
    //public Chapter nowChapter;
    public int chatIndex;
    public int roundIndex;

/*    public Chat FindChat(string text)
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
    }*/

    public void AddChapter(string who, string name)
    {
        chatSystem.AddMember(who);
        //chatSystem.FindMember(who).chapterName = FindChapter(name).showName;

/*        if (previousChapter.saveLocation != ESaveLocation.NotSave)
        {
            if (chatSystem.FindMember(who).name == chatSystem.FindMember(previousChapter.saveLocation.ToString()).name)
            {
                Debug.Log("?筌뤾벳??곌떠?????怨룹꽑???꾩룆?餓?嶺뚯쉳?듸쭛?);
                Chapter(name);
            }
        }*/
    }

/*    public Chapter FindChapter(string name)
    {
        foreach (Chapter chapter in chatContainer.MainChapter)
        {
            if (chapter.showName == name)
                return chapter;
        }

        Debug.Log("癲?甕겹끂??癲ル슓??젆癒る뎨?????됰꽡");
        return null;
    }*/

    public void NextChapter(string name)
    {
       // AddChapter(FindChapter(name).saveLocation.ToString(), name); //

        //MemberChat member = chatSystem.FindMember(FindChapter(name).saveLocation.ToString());
        //chapterManager.Chapter(member.name, member.chapterName);

        // 嶺뚮씭?ｉ뜮?.. ??嶺?踰ㅸ땻???띠룇?? ?筌뤾벳?????.. ?잙갭梨뜻틦?choice??怨몄퍦?????섎뎅 ?熬곣뫗????怨몃턄 chapter ?釉띾쐞?????ㅲ뵛
        //FindChapter(chatContainer.NowChapter.nextChapterName).showName);
    }

    public void Chapter(string name)
    {
/*        nowChapter = FindChapter(name);

        if (chatSystem.FindMember(nowChapter.saveLocation.ToString()).name == chatSystem.currentMemberName)
        {
            if (nowChapter.isCan == false)
            {
                //chatContainer.NowChapter = nowChapter;
               // chatContainer.NowChapter.isChapterEnd = false;
                roundIndex = 0;

                chatSystem.changeMemberButton.pickingMode = PickingMode.Ignore;
                chatSystem.memberList.style.display = DisplayStyle.None;

                //StartCoroutine(InputCChat(false, nowChapter.chat));
            }
        }
        else
            Debug.Log("?熬곣뫗??嶺?踰ㅸ땻??낅슣???뗏?嶺????멥럶???꺄 ?낅슣???????담궖");*/
    }

    public void EndChapter()
    {
/*        nowChapter.isChapterEnd = true;
        previousChapter = nowChapter;

        chatSystem.changeMemberButton.pickingMode = PickingMode.Position;
        chatSystem.isMemberListOpen = false;
        chatSystem.ChangeMember();

        if (nowChapter.is_nextChapter)
        {
            NextChapter(nowChapter.nextChapterName);
        }
        else
            Debug.Log("???깅쾳 嶺?踰ㅸ땻????⑸폋!");*/
    }

/*    public IEnumerator InputCChat(bool isReply, List<Chat> chats)
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

        // 嶺??????紐껋┣?????곗꽑??節딇깴嶺??熬곣뫖??????怨삵룖??嶺뚯쉶?꾣룇 ???깅さ嶺?嶺뚯쉶?꾣룇 ?影??꽑?낅슣?딁뵳?
        if (isReply == false && nowChapter.askAndReply.Count > 0)
            InputQQuestion(nowChapter.askAndReply);
        // 嶺뚯쉶?꾣룇????????????紐껋┣?????곗꽑??節딇깴嶺?????????怨삵룖??嶺?踰ㅸ땻???硫몃??낅슣?딁뵳?
        else
            EndChapter();
    }*/

    /*public void InputQQuestion(List<AskAndReply> asks)
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
    }*/

   /* public void InputLockQuestion(List<LockAskAndReply> locks)
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
    }*/

    //public IEnumerator InputReply(List<Chat> replies)
    //{
    //    int i = 0;
    //    if (replies != null)
    //    {
    //        while (i != replies.Count)
    //        {
    //            Debug.Log(replies[i].ask + " 嶺뚯쉶?꾣룇");
    //            chatSystem.InputChat(chatContainer.NowChapter.saveLocation,
    //                EChatType.Text, replies[i].ask, true, InputReply(replies[i]));
    //            i++;
    //            yield return null;
    //        }
    //    }

    //    yield return null;
    //}
}
