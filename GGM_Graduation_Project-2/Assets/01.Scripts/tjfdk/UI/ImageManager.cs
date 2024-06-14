using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImageDefualt
{
    public string name;
    public Sprite image;
    public bool isOpen;
}

[Serializable]
public class ImagePng : ImageDefualt
{
    public string memo;
    public bool importance;
    public Sprite saveImage;
    public Vector2 size;
    public Vector2 pos;
    public Vector2 descriptePos;
}

[Serializable]
public class ImageB : ImageDefualt
{
    public List<string> pngName = new List<string>();
}

[Serializable]
public class Memo
{
    public string name;
    public string memo;
}

public class ImageManager : UI_Reader
{
    public List<ImageB> images = new List<ImageB>();
    public List<ImagePng> pngs = new List<ImagePng>();
    public List<Memo> memos = new List<Memo>();

    public Dictionary<string, string> memoDic = new Dictionary<string, string>();

    private void Start()
    {
        foreach (Memo memo in memos)
            memoDic.Add(memo.name, memo.memo);
    }

    public ImagePng FindPNG(string name)
    {
        foreach (ImagePng png in  pngs)
        {
            if (png.name == name) 
                return png;
        }

        return null;
    }
}
