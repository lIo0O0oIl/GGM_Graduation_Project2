using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
public class ImageDefualt
{
    public string name;
    public Sprite image;
    public bool isOpen;
}

[Serializable]
public class ImageSmall : ImageDefualt
{
    public string memo;
    public bool importance;
    public Sprite saveSprite;
    public Vector2 size;
    public Vector2 pos;
    public Vector2 memoPos;
}

[Serializable]
public class ImageBig : ImageDefualt
{
    public List<string> pngName = new List<string>();
}

[Serializable]
public class Text
{
    public string name;
    public string memo;
}

public class ImageManager : MonoBehaviour
{
    public List<ImageBig> images;
    public List<ImageSmall> pngs;
    public List<Text> texts;

    public Dictionary<string, ImageBig> imageList;
    public Dictionary<string, ImageSmall> pngList;
    public Dictionary<string, Text> textList;

    private void Awake()
    {
        images = new List<ImageBig>();
        pngs = new List<ImageSmall>();
        texts = new List<Text>();

        imageList = new Dictionary<string, ImageBig>();
        pngList = new Dictionary<string, ImageSmall>();
        textList = new Dictionary<string, Text>();
    }

    private void Start()
    {
        foreach (ImageBig image in images)
            imageList.Add(image.name, image);

        foreach (ImageSmall png in pngs)
            pngList.Add(png.name, png);

        foreach (Text memo in texts)
            textList.Add(memo.name, memo);
    }

    public ImageBig FindImage(string name) { return imageList[name]; }

    public ImageSmall FindPng(string name)
    {
        //foreach (ImageSmall png in  pngs)
        //{
        //    if (png.name == name) 
        //        return png;
        //}

        return pngList[name];
    }

    public Text FindText(string name) { return textList[name]; }
}
