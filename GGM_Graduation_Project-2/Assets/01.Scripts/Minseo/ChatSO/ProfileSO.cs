using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpressionState
{
    Defualt = 0, // 기본 무표정
    Embarrassed = 1, // 당황한 표정
    Sadness = 2 // 슬픈? 표정
}

public enum HumanStage
{
    HyeonSeok = 0,
    Jihyeon= 1,
    Junwon = 2,             
    Suyeon = 3,
    Daeyang = 4
}

[Serializable]
public struct ProfileChat
{
    public ExpressionState expressionState;
    public HumanStage humanStage;
    public SpriteRenderer[] spriteRenderers;
}

[CreateAssetMenu(fileName = "ChatSO", menuName = "SO/ProfileSO")]
public class ProfileSO : ScriptableObject
{
    public ProfileChat state;
}
