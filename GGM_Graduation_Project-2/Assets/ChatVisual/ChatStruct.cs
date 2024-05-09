using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public enum ESaveLocation
    {
        None,
        Assistant,      // 조수
        HyeonSeok,      // 현석
        JiHyeon,        // 지현
        JunWon,         // 준원
    }

    public enum EChatState
    {
        Other = 0,      // 조수쪽이 말하는 것
        Me = 1,      // 형사가 말하는 것
        Ask = 2,        // 형사가 묻는 것임.
        LoadNext        // 또 이어질 대화가 있다면. 다른 사람이면.
    }

    public enum EFace
    {
        Default,        // 무표정
        Blush,          // 당황 (얼굴빨개짐.)
        Difficult       // 비협조적인, 곤란한
    }

    public enum EChatEvent
    {
        None = 0,
        Vibration,      // 텍스트 진동
        Round,      // 파일 추가해주기      
        Camera,     // 카메라 효과 넣어주기
    }

    [Serializable]
    public class Chat      // 누가 어떤 말을 하는가
    {
        public EChatState state;     // 말하는 것의 타입
        public string text;        // 말 하는 것.
        public EFace face;       // 말 할 때의 표정
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }

    [Serializable]
    public class AskAndReply
    {
        public string ask;        // 물을 수 있는 선택지
        public List<Chat> reply = new List<Chat>();     // 그에 대한 대답들
        public bool is_UseThis;     // 사용했는지
    }

    [Serializable]
    public class LockAskAndReply
    {
        public List<string> evidence = new List<string>();
        public string ask;        // 물을 수 있는 선택지
        public List<Chat> reply = new List<Chat>();     // 그에 대한 대답들
        public bool is_UseThis;     // 사용했는지
    }

    [Serializable]
    public class Chapter
    {
        public string showName;     // 보여질 이름
        public ESaveLocation saveLocation;     // 누군지
        public List<Chat> chat = new List<Chat>();         // 챗팅
        public List<AskAndReply> askAndReply = new List<AskAndReply>();           // 질문들
        public List<LockAskAndReply> lockAskAndReply = new List<LockAskAndReply>();       // 잠긴 질문들
        public List<string> round = new List<string>();           // 증거로 사용되어
    }

    public class ChatStruct : MonoBehaviour { }
}