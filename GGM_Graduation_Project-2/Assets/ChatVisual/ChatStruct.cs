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
        GJH, // GoJinHyek
        DY, // Deayang

        Teacher,
        Friend,
        Home,

        Tutorial1,
        Tutorial2
    }

    public enum EChatState
    {
        Other = 0,
        Me = 1,
    }

    public enum EChatType
    {
        Text = 0,
        Image,
        CutScene,
        Question,
        LockQuestion
    }

    public enum EFace
    {
        NotChange,       
        Default,       
        Blush,      
        Angry   
    }

    public enum EChatEvent
    {
        Default,
        Vibration,
        OneVibration,
        Camera,
        LoadFile,
        LoadNextDialog
    }

/*    [Serializable]
    public class Chat 
    {
        public EChatState state; 
        public EChatType type;
        public string text;        // ???????留⑶뜮??????猷몄굡???????
        public bool is_UseThis;     // ????????????????釉먮폁??????
        public EFace face;       // ?????????????遺얘턁???????
        public bool isCan;
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }

    [Serializable]
    public class AskAndReply
    {
        public string ask;        // ?????????????????????????????????????????癲ル슢????????μ떜媛?걫???
        public List<Chat> reply = new List<Chat>();     // ????????ロ깫?????우뒭亦낆쥋援??룰큿??????猷??????????????
        public bool is_UseThis;     // ????????????????釉먮폁??????

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