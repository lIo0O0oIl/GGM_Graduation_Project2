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
}
