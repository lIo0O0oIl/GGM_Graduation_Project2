using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class FileT
{
    public string fileName;
    public string fileParentName;
    public FileType fileType;
    public string eventName;
    public string lockQuestionName;
}

public class FileManager : UI_Reader
{
    public List<FileT> folderFiles = new List<FileT>();
    //    public List<FileImage> imageFiles = new List<FileImage>();
    //    public List<FileText> textFiles = new List<FileText>();

    public void MakeCan(FileT file)
    {
        chapterManager.FindChat(file.eventName).isCan = false;
    }
}
