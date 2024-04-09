using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageFile : MonoBehaviour, IPointerClickHandler
{
    public Sprite image;        // 보여질 이미지 스프라이트
    public Vector2 showScale = new Vector2(500, 500);       // 보여질 크기
    private string fileName;

    public int index = -1;      // 기본적으로 -1, 뭐가 있는 애들만 바꿔주기

    //public bool userKnowThis = false;       // 유져가 이걸 한번이라도 열어봤으면

    private void Start()
    {
        fileName = GetComponentInChildren<TMP_Text>().text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.OpenImageFile(image, showScale, fileName, index);
        }
    }
}
