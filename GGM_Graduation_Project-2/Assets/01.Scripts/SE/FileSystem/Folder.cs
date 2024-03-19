using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Folder : MonoBehaviour, IPointerClickHandler
{
    public bool is_Lock = false;
    public string password;
    private Image lockImage;

    public string myPath;
    private string goPath;
    private string fileName;

    private void Start()
    {
        fileName = GetComponentInChildren<TMP_Text>().text;
        goPath = $"{myPath}\\{fileName}";
        if (is_Lock)
        {
            lockImage = transform.GetChild(2).GetComponent<Image>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            if (is_Lock)
            {
                FileManager.instance.OpenLock(fileName, password, lockImage);
            }
            else
            {
                FileManager.instance.GoFile(myPath, goPath);
            }
        }
    }
}
