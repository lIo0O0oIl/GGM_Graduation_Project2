using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Expression
{
    Defualt = 0,
    Embarrassed,
    Sadness
}

[CreateAssetMenu(fileName = "ChatSO", menuName = "SO/ProfileSO")]
public class ProfileSO : ScriptableObject
{
    Expression expression;
    WhoSO whoSO;

    // 스프라이트를  저장해두고 각자 표정을 바꾸면 그 해당 스프라이트로 바뀌기
    // 사람 체크도 해줘야 됨
}
