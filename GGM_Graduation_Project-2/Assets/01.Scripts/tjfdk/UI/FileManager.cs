using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class File
{
    public string fileName;
    public string fileParentName;
    public FileType fileType;
    public string eventName;
    public string lockQuestionName;
}

public class FileManager : UI_Reader
{
    [SerializeField] List<File> folderFiles;
    public Dictionary<string, File> folderFileList;

    private void Awake()
    {
        folderFiles = new List<File>();
        folderFileList = new Dictionary<string, File>();
    }

    private void Start()
    {
        foreach (File file in folderFiles)
            folderFileList.Add(file.fileName, file);
    }

    public File FindFile(string name) { return folderFileList[name]; }
    public void UnlockChapter(File file)
    {
        if (file.eventName != "")
        {
/*            Chapter chapter = chapterManager.FindChapter(file.eventName);
            if (chapter != null)
            {
                if (chapter.isCan == true)
                {
                    chapter.isCan = false;
                    if (chapterManager.previousChapter.isChapterEnd)
                    {
                        chapterManager.NextChapter(chapter.showName);
                        Debug.Log("ddakakakkakakakakak");

                    }
                }
            }
            else
                Debug.Log("chat ?몃━嫄??우쓬!!");*/
        }
    }

    public void UnlockChat(File file)
    {
        if (file.eventName != "")
        {
/*            if (chapterManager.FindChat(file.eventName) != null)
            {
                if (chapterManager.FindChat(file.eventName).isCan == true)
                    chapterManager.FindChat(file.eventName).isCan = false;
            }
            else
                Debug.Log("梨뺥꽣 ?몃━嫄??우쓬!");*/
        }
    }
}
