using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[Serializable]
//public class ImageDefualt
//{
//    public string imageName;
//    public Sprite image;
//}

//[Serializable]
//public class ImageEvidence
//{
//    public Sprite evidenceImage;
//    public string evidenceName;
//    public string evidenceMemo;

//    public Vector2 evidenceSize;
//    public Vector2 evidencePos;

//    public bool importance;
//}

//[Serializable]
//public class ImageFile1 : ImageDefualt
//{
//    public List<ImageEvidence> evidences = new List<ImageEvidence>();
//}

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
    public Vector2 size;
    public Vector2 pos;
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

public class ImageManager : MonoBehaviour
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

    //public Dictionary<string, ImageDefualt> imageDic = new Dictionary<string, ImageDefualt>();

    //private void Start()
    //{
    //    foreach (var image in images)
    //    {
    //        imageDic[image.name] = image;
    //    }
    //    foreach (var png in pngs)
    //    {
    //        imageDic[png.name] = png;
    //    }
    //}

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
