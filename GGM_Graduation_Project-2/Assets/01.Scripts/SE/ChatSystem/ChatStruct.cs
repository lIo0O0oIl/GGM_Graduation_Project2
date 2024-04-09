using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChatState
{
    Other = 0,      // 조수쪽이 말하는 것
    Me = 1,      // 형사가 말하는 것
    Ask = 2,        // 형사가 묻는 것임.
    LoadNext        // 또 이어질 대화가 있다면. 다른 사람이면.
}

[Serializable]
public struct Chat      // 누가 어떤 말을 하는가
{
    public ChatState state;
    public string text;        // 말 하는 것.
}

[Serializable]
public struct AskAndReply
{
    public string ask;        // 물을 수 있는 선택지
    public string[] reply;     // 그에 대한 대답들
}

[Serializable]
public struct Chapters
{
    public Chat[] chat;
    public AskAndReply[] askAndReply;
    public string who;     // 누군데?
}

public class ChatStruct : MonoBehaviour { }
