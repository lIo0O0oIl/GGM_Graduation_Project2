using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class IconsAtlas : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas iconAtlas;

    [SerializeField]
    private Image image;

    private void Start()
    {
        //image = iconAtlas[0]; 
    }


}
