using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public enum ESaveLocation
    {
        NotSave,
        JH, // Jihyen
        JW, // Junwon
        HS, // Hyensuck
        CM, // Cheamin
        HG, // HyenGue
        DY, // Deayang

        Teacher,
        Friend,
        Home
    }

    public enum EChatState
    {
        Other = 0,
        Me = 1,
    }

    public enum EChatType
    {
        Text,
        Image,
        CutScene,
        Question,
        LockQuestion
    }

    public enum EFace
    {
        Default,       
        Blush,      
        Difficult   
    }

    public enum EChatEvent
    {
        Default,
        Vibration,
        Camera,
        LoadFile,
        LoadNextDialog
    }

/*    [Serializable]
    public class Chat 
    {
        public EChatState state; 
        public EChatType type;
        public string text;        // ?????β뼯爰귨㎘???
        public bool is_UseThis;     // ????????됲닓?꿔꺂???
        public EFace face;       // ???????????嶺뚮????
        public bool isCan;
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }

    [Serializable]
    public class AskAndReply
    {
        public string ask;        // ???ル봾??????????됲닓 ????ｋ??쑩됬빊??됱삩?
        public List<Chat> reply = new List<Chat>();     // ???녾컯嶺?竊됪뤃????????????
        public bool is_UseThis;     // ????????됲닓?꿔꺂???

        public ESaveLocation changeWhoName;
        public bool isChange;
        public string changeName;
    }

    [Serializable]
    public class LockAskAndReply
    {
        public List<string> evidence = new List<string>();
        public string ask;       
        public List<Chat> reply = new List<Chat>();  
        public bool is_UseThis; 
    }

    [Serializable]
    public class Chapter
    {
        public string showName;    
        public ESaveLocation saveLocation;   
        public List<Chat> chat = new List<Chat>();      
        public List<AskAndReply> askAndReply = new List<AskAndReply>();   
        public List<LockAskAndReply> lockAskAndReply = new List<LockAskAndReply>();      
        public List<string> round = new List<string>();       
      
        public bool isChapterEnd;     // is this chapter ended?
        public bool isCan;            // can this chapter play?
        public bool is_nextChapter;     // is this chapter have next chapter?
        public string nextChapterName;         // next chapter name
    }*/

    public class ChatStruct : MonoBehaviour { }
}