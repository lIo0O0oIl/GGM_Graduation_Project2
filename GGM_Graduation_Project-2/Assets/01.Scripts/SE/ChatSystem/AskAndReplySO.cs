using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AskReply       // 하나의 질문과 그에 대한 답변. 답변이 2개 이상에 메세지일 경우 \이걸로 나눠서 표시한다.
{
    public string ask;        // 물을 수 있는 선택지들
    public string reply;     // 그에 대한 대답들
    public bool is_used;

    public string[] GetReplys()
    {
        return reply.Split('\\');
    }
}

[CreateAssetMenu(fileName = "AskAndReplySO", menuName = "SO/AskAndReplySO")]           // 이거 새로 스크립트 파서 만들어주기
public class AskAndReplySO : ScriptableObject
{
    public string askName;      // 묻는 것의 이름
    public AskReply ask;     // 묻는 것
}
