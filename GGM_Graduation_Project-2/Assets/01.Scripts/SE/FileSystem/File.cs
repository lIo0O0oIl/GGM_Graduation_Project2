using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class File : MonoBehaviour, IPointerClickHandler
{
    public string myPath;
    public string goPath;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.GoFile(myPath, goPath);
        }
    }
}
