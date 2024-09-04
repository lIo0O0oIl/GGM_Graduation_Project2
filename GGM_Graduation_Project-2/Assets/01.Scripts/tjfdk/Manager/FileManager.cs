using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.VisualScripting;
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

public class FileManager : MonoBehaviour
{
    [SerializeField] List<FileSO> folderFiles = new List<FileSO>();
    //public Dictionary<string, FileSO> folderFileList;

    private void Awake()
    {
        //folderFileList = new Dictionary<string, FileSO>();
    }

    private void Start()
    {
        ReSetFileSO();
    }

    private void OnEnable()
    {
        //foreach (FileSO file in folderFiles)
        //    folderFileList.Add(file.fileName, file);
    }

    public void ReSetFileSO()
    {
        foreach (FileSO file in folderFiles)
            file.isRead = false;
    }

    public FileSO FindFile(string name) 
    {
        foreach (FileSO file in folderFiles)
        {
            if (file.fileName == name)
                return file;
        }

        return null;
    }

    public void UnlockChat(string triggerName)
    {
        if (triggerName != "")
        {
            // the same trigger name and file name
            if (!GameManager.Instance.chatHumanManager.checkEvidence.Contains(triggerName))     // 없으면
            {
                // 넣기
                GameManager.Instance.chatHumanManager.checkEvidence.Add(triggerName);
            }
        }
        else
            Debug.LogError("Trigger name is null");

        //GameManager.Instance.chatHumanManager.StartChatting();
    }
}
