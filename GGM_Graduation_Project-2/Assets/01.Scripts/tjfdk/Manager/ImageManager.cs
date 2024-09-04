using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//[Serializable]
//public class ImageDefualt
//{
//    public string name;
//    public Sprite image;
//    public bool isOpen;
//}

//[Serializable]
//public class ImageSmall : ImageDefualt
//{
//    public string memo;
//    public bool importance;
//    public Sprite saveSprite;
//    public Vector2 size;
//    public Vector2 pos;
//    public Vector2 memoPos;
//}

//[Serializable]
//public class ImageBig : ImageDefualt
//{
//    public List<string> pngName = new List<string>();
//}

//[Serializable]
//public class Text
//{
//    public string name;
//    public string memo;
//}

public class ImageManager : MonoBehaviour
{
    public List<ImageSO> images = new List<ImageSO>();
    public List<PngSO> pngs = new List<PngSO>();
    public List<TextSO> texts = new List<TextSO>();

    public Dictionary<string, ImageSO> imageList;
    public Dictionary<string, PngSO> pngList;
    public Dictionary<string, TextSO> textList;

    private void Awake()
    {
        imageList = new Dictionary<string, ImageSO>();
        pngList = new Dictionary<string, PngSO>();
        textList = new Dictionary<string, TextSO>();
    }

    private void OnEnable()
    {
        //foreach (ImageSO image in images)
        //    imageList.Add(image.name, image);

        //foreach (PngSO png in pngs)
        //    pngList.Add(png.name, png);

        //foreach (TextSO memo in texts)
        //    textList.Add(memo.name, memo);
    }

    public ImageSO FindImage(string name) 
    {
        foreach (ImageSO image in images)
        {
            if (image.name == name)
                return image;
        }

        return null;

        //return imageList[name]; 
    }

    public PngSO FindPng(string name)
    {
        foreach (PngSO png in pngs)
        {
            if (png.name == name)
                return png;
        }

        return null;

        //return pngList[name];
    }

    public TextSO FindText(string name) 
    {
        foreach (TextSO text in texts)
        {
            if (text.name == name)
                return text;
        }

        return null;

        //return textList[name]; 
    }
}
