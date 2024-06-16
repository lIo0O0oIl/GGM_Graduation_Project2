using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "pngSO", menuName = "SO/PngSO")]
public class PngSO : ScriptableObject
{
    public string name;
    public Sprite image;
    public bool isOpen;

    public string memo;
    public bool importance;
    public Sprite saveSprite;
    public Vector2 size;
    public Vector2 pos;
    public Vector2 memoPos;
}
