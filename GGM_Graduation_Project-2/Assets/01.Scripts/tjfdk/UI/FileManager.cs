using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FileT
{
    public string fileName;
    public string fileParentName;
    public bool isCheck;
    public FileType fileType;
    public string lockQuestionName;

    public void IsCheck() { isCheck = true; }
}

public class FileManager : UI_Reader
{
    public List<FileT> folderFiles = new List<FileT>();
    //    public List<FileImage> imageFiles = new List<FileImage>();
    //    public List<FileText> textFiles = new List<FileText>();
}
