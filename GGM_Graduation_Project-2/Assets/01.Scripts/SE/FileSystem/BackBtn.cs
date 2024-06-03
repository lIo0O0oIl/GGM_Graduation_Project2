using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBtn : MonoBehaviour
{
    public string nowPath;

    public void BackBtnClick()
    {
        if (nowPath.LastIndexOf('\\') != -1)
        {
            string goPath = nowPath.Substring(0, nowPath.LastIndexOf('\\'));
            //FileManager.instance.GoFile(nowPath, goPath);
            nowPath = goPath;
        }
    }
}
