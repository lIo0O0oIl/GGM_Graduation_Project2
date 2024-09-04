using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "pngSO", menuName = "SO/PngSO")]
public class PngSO : ScriptableObject
{
    public bool isRead;
    public bool isOpen;

    public Sprite image;
    public Vector2 pos;
    public Vector2 size;
    public string memo;
    public Vector2 memoPos;

    public bool is_save;
    public string saveName;
    public Sprite saveSprite;
}
