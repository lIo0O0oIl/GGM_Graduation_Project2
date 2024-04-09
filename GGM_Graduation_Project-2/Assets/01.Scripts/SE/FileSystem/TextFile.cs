using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextFile : MonoBehaviour, IPointerClickHandler
{
    [TextArea(3, 10)]
    public string text;        // 보여질 텍스트
    private string fileName;

    public int index = -1;

    private void Start()
    {
        fileName = GetComponentInChildren<TMP_Text>().text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.OpenTextFile(text, fileName, index);
        }
    }
}
