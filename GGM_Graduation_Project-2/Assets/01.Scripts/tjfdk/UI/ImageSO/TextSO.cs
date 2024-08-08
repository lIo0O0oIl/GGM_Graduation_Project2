using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "textSO", menuName = "SO/TextSO")]
public class TextSO : ScriptableObject
{
    public string name;
    [TextArea(25, 50)]
    public string memo;
}
