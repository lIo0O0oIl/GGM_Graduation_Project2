using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChatState
{
    Other = 0,      // 조수쪽이 말하는 것
    Me = 1,      // 형사가 말하는 것
}

[Serializable]
public struct Chat      // 누가 어떤 말을 하는가
{
    public ChatState state;
    public string text;        // 말 하는 것.
}


[CreateAssetMenu(fileName = "ChatSO", menuName = "SO/ChatSO")]
public class ChatSO : ScriptableObject
{
    public Chat[] chat;
    public bool is_Ask;     // 묻는 것이 있냐
}       // 쳇팅만 하는 SO


/*
 SO 에 들어갈 것.

선택지가 없을 때  
1. 조수의 말
2. 나의 말

선택지가 있을 때
0. 쳇이 끝나고 묻는 것이 나옴.
1. 내가 선택할 수 있는 선택지들
2. 각 선택지에 따른 조수의 말
3. 내가 이 선택지를 선택했는지 안했는지

큐를 써서 화면을 누를 때마다 텍스트가 떠오르게 하기

한 챕터씩 나오게 만들기
 */