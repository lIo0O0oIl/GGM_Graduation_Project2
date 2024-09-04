using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "imageSO", menuName = "SO/ImageSO")]
public class ImageSO : ScriptableObject
{
    public bool isRead;
    public bool isOpen;

    public Sprite image;

    public List<string> pngName = new List<string>();
    public List<string> textName = new List<string>();
}
