using ChatVisual;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : UI_Reader
{
    public void AddChapter(string who, string name)
    {
        Debug.Log("梨뺥꽣 遺숈뿬以?: " + who + " " + name);
        if (chatSystem.FindMember(who).chapterName == "")
            chatSystem.FindMember(who).chapterName = FindChapter(name).showName;
        else
            Debug.Log("?대? 梨뺥꽣媛 ?덉뼱??異붽??????놁쓬");
    }

    public Chapter FindChapter(string name)
    {
/*        foreach (Chapter chapter in chatContainer.MainChapter)
        {
            if (chapter.showName == name)
                return chapter;
        }*/

        Debug.Log("梨뺥꽣 李얘린 ?ㅽ뙣");
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
        Debug.Log("肄붾（??吏꾩엯 " + chats.Count);
        i = 0;
        //for (int i = 0; i < chats.Count; i++)
        while (i != chats.Count)
        {
            if (i < chats.Count)
            {
                Debug.Log(chats[i].text + " ???");

                // ?ш린??StartCoroutine(Chatda(chats, chapter.askAndReply, chapter.lockAskAndReply, chapter)); ?닿굅 ???꾩?
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
        //    Debug.Log(asks[i].ask + " 吏덈Ц");
        //    chatSystem.InputQuestion(chapter.saveLocation, 
        //        EChatType.Question, asks[i].ask, true);
        //    yield return new WaitForSeconds(1.5f);
        //}

        //i = 0;
        ////for (int i = 0; i < lockasks.Count; i++)
        //while (i != lockasks.Count)
        //{
        //    Debug.Log(lockasks[i].ask + " ?좉릿吏덈Ц");
        //    chatSystem.InputQuestion(chapter.saveLocation,
        //        EChatType.LockQuestion, lockasks[i].ask, true);
        //    yield return new WaitForSeconds(1.5f);
        //}

        //yield break;
    }
}
