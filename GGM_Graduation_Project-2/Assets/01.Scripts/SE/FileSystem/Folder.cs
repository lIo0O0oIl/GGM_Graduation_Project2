using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Folder : MonoBehaviour, IPointerClickHandler
{
    public string myPath;
    private string goPath;

    private void Start()
    {
        goPath = $"{myPath}\\{GetComponentInChildren<TMP_Text>().text}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.GoFile(myPath, goPath);
        }
    }
}
