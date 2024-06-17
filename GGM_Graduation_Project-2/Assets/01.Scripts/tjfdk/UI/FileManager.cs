using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

//[Serializable]
//public class File
//{
//    public string fileName;
//    public string fileParentName;
//    public FileType fileType;
//    public string eventName;
//    public string lockQuestionName;
//}

public class FileManager : UI_Reader
{
    [SerializeField] List<FileSO> folderFiles = new List<FileSO>();
    //public Dictionary<string, FileSO> folderFileList;

    private void Awake()
    {
        //folderFileList = new Dictionary<string, FileSO>();
    }

    private void OnEnable()
    {
        //foreach (FileSO file in folderFiles)
        //    folderFileList.Add(file.fileName, file);
    }

    public FileSO FindFile(string name) 
    {
        //foreach (var f in folderFileList.Values)
        //    Debug.Log(f.fileName + " " + name);

        //FileSO file = folderFileList[name];
        //if (file != null)
        //    return file;
        //else
        //    return null;

        ////return folderFileList[name]; 
        ///

        foreach (FileSO file in folderFiles)
        {
            if (file.fileName == name)
                return file;
        }

        return null;
    }

    public void UnlockChat(FileSO file)
    {
        Debug.Log(file.fileName + " " + file.eventName);
        if (file.eventName != "")
        {
            Debug.Log("트리거 호출");
            GameManager.Instance.chapterManager.nowCondition.is_UseThis = true;
            GameManager.Instance.chapterManager.StartChatting();

            //if (chapterManager.FindChat(file.eventName) != null)
            //{
            //    if (chapterManager.FindChat(file.eventName).isCan == true)
            //        chapterManager.FindChat(file.eventName).isCan = false;
            //}
            //else
            //    Debug.Log("筌?벤苑??紐꺿봺椰???곗벉!");
        }
    }
}
