using System;
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
        public EChatEvent[] textEvent;
    }

    [Serializable]
    public struct AskAndReply
    {
        public string ask;        // 물을 수 있는 선택지
        public Chat[] reply;     // 그에 대한 대답들
        public bool is_UseThis;     // 사용했는지
    }

    [Serializable]
    public struct LockAskAndReply
    {
        public string[] evidence;
        public string ask;        // 물을 수 있는 선택지
        public Chat[] reply;     // 그에 대한 대답들
        public bool is_UseThis;     // 사용했는지
    }

    [Serializable]
    public class Chapters
    {
        public string showName;     // 보여질 이름
        public ESaveLocation saveLocation;     // 누군지
        public Chat[] chat;         // 챗팅
        public AskAndReply[] askAndReply;           // 질문들
        public LockAskAndReply[] lockAskAndReply;       // 잠긴 질문들
        public string[] round;           // 증거로 사용되어
    }

    public class ChatStruct : MonoBehaviour { }
}