using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public enum ESaveLocation
    {
        NotSave,
        /*獄쏅벤?썸뉩?      // 鈺곌퀣??
        ?⑥????      // ?袁⑷퐤
        揶쏅벡???        // 筌왖??
        ?????         // 餓Β??
        ??곸퐟沃?
        揶쏅베???*/
    }

    public enum EChatState
    {
        Other = 0,      // 鈺곌퀣?뷂쭫?뚯뵠 筌띾?釉??野?
        Me = 1,      // ?類ㅺ텢揶쎛 筌띾?釉??野?
        Ask = 2,        // ?類ㅺ텢揶쎛 ?얠궠??野껉퍔??
        LoadNext        // ????곷선筌????遺? ??덈뼄筌? ??삘뀲 ???????      // Ask ????욧탷 ?????딅막 揶쎛?關苑????됱벉 ?類ｂ봺??곻폒疫?.
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
        Default,        // ?얜똾紐??
        Blush,          // ?諭곸넺 (??⑤렗??몿而삼쭪?)
        Difficult       // ??쑵?딂?怨쀬읅?? ?ⓦ끇???
    }

    public enum EChatEvent
    {
        Default,
        Vibration,      // ??용뮞??筌욊쑬猷?
        Round,      // ???뵬 ?곕떽???곻폒疫?     
        Camera,     // 燁삳?李????ｋ궢 ?節뚮선雅뚯눊由?
    }

    [Serializable]
    public class Chat      // ?袁? ??堉?筌띾Ŋ????롫뮉揶쎛
    {
        public EChatState state;     // 筌띾?釉??野껉퍔??????
        public string text;        // 筌???롫뮉 野?
        public EFace face;       // 筌??????벥 ??뽰젟
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }

    [Serializable]
    public class AskAndReply
    {
        public string ask;        // ?얠눘??????덈뮉 ?醫뤾문筌왖
        public List<Chat> reply = new List<Chat>();     // 域밸챷肉?????????щ굶
        public bool is_UseThis;     // ?????덈뮉筌왖
    }

    [Serializable]
    public class LockAskAndReply
    {
        public List<string> evidence = new List<string>();
        public string ask;        // ?얠눘??????덈뮉 ?醫뤾문筌왖
        public List<Chat> reply = new List<Chat>();     // 域밸챷肉?????????щ굶
        public bool is_UseThis;     // ?????덈뮉筌왖
    }

    [Serializable]
    public class Chapter
    {
        public string showName;     // 癰귣똻肉э쭪???已?
        public ESaveLocation saveLocation;     // ?袁㏓럵筌왖
        public List<Chat> chat = new List<Chat>();         // 筌??샒
        public List<AskAndReply> askAndReply = new List<AskAndReply>();           // 筌욌뜄揆??
        public List<LockAskAndReply> lockAskAndReply = new List<LockAskAndReply>();       // ?醫됰┸ 筌욌뜄揆??
        public List<string> round = new List<string>();           // 筌앹빓援끾에??????뤿선

        public bool is_nextChapter;     // ??쇱벉筌?벤苑ｆ에???뤿선揶쎛?遺?
        public string nextChapterName;         // ??쇱벉 筌?벤苑ｆ에???뤿선揶쏄쑬?롳쭖?筌?벤苑???紐껊쑔??揶쎛 ?袁⑹뒄??
    }

    public class ChatStruct : MonoBehaviour { }
}