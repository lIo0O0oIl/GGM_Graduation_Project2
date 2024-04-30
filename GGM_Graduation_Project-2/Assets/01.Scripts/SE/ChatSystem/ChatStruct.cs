using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ChatState
{
    Other = 0,      // 조수쪽이 말하는 것
    Me = 1,      // 형사가 말하는 것
    Ask = 2,        // 형사가 묻는 것임.
    LoadNext        // 또 이어질 대화가 있다면. 다른 사람이면.
}

public enum E_Face
{
    Default,        // 무표정
    Blush,          // 당황 (얼굴빨개짐.)
    Difficult       // 비협조적인, 곤란한
}

public enum E_Event
{
    None,
    Speed,
    Camera,
}

[Serializable]
public struct Chat      // 누가 어떤 말을 하는가
{
    public E_ChatState state;     // 말하는 것의 타입
    public string text;        // 말 하는 것.
    public E_Face face;       // 말 할 때의 표정
    public E_Event textEvent;
    public int speed;
}

[Serializable]
public struct AskAndReply
{
    public string ask;        // 물을 수 있는 선택지
    public string[] reply;     // 그에 대한 대답들
}

[Serializable]
public struct Round
{
    public string round;
    public string text;
}

[Serializable]
public class Chapters
{
    public string who;     // 누군지
    public Chat[] chat;         // 챗팅
    public AskAndReply[] askAndReply;           // 질문들
    public Round[] round;               // 열리는 파일들
    public string[] evidence;           // 증거
}

public class ChatStruct : MonoBehaviour { }
