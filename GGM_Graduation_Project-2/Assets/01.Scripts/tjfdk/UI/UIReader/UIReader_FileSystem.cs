using ChatVisual;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum FileType
{
    FOLDER,
    IMAGE,
    TEXT
}

[Serializable]
public class FolderFile
{
    public string folderName;
    public string parentFolderName;
    public List<VisualElement> folderFiles;
    public List<VisualElement> textFiles;
    public List<VisualElement> imageFiles;
    //public List<VisualElement> folderFiles;
    //public List<VisualElement> textFiles;
    //public List<VisualElement> imageFiles;

    public FolderFile(string name, string parent)
    {
        folderName = name;
        parentFolderName = parent;
        folderFiles = new List<VisualElement>();
        textFiles = new List<VisualElement>();
        imageFiles = new List<VisualElement>();

        //void FolderFile(string name, string parent)
        //{
        //    folderName = name;
        //    parentFolderName = parent;
        //}
    }
}

public class UIReader_FileSystem : UI_Reader
{
    [SerializeField]
    private float fileAreaSizeOn, fileAreaSizeOff;
    [SerializeField]
    private Texture2D changeSizeBtnOn, changeSizeBtnOff;
    public bool isFileSystemOpen;

    Tween changeFileSystemSizeDOT;



    // UXML
    VisualElement ui_fileSystemArea;
    VisualElement ui_fileGround;
    VisualElement ui_filePathGround;
    VisualElement ui_mainFilePath;
    [HideInInspector] public Button ui_changeSizeButton;



    // Template
    [Header("Template")]
    [SerializeField] VisualTreeAsset ux_filePath;
    [SerializeField] VisualTreeAsset ux_fileGround;
    [SerializeField] VisualTreeAsset ux_folderFile;
    [SerializeField] VisualTreeAsset ux_imageFile;
    [SerializeField] VisualTreeAsset ux_textFile;



    // folder
    public string currentFolderName;
    [SerializeField] List<FolderFile> fileFolders; // this is test, fileFolders -> fileFolderList X!!
    // if dictionary is correct working, remove fileFolders!
    //public Dictionary<string, FolderFile> fileFolderList;

    // path
    private Stack<string> filePathLisk;
    public FolderFile currentFileFolder;

    // file drag and drop
    private VisualElement fileDefaultArea;
    private List<VisualElement> lockQuestions;

    private void Awake()
    {
        base.Awake();

        fileFolders = new List<FolderFile>();
        //fileFolderList = new Dictionary<string, FolderFile>();
        filePathLisk = new Stack<string>();
        lockQuestions = new List<VisualElement>();

        MinWidth = 500f;
        MinHeight = 500f;
        MaxWidth = 1800f;
        MaxHeight = 980f;
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Event_Load();

        fileFolders.Add(new FolderFile("Main", "Main"));
        //fileFolderList.Add("Main", new FolderFile("Main"));
        AddFilePath("Main");
    }

    private void UXML_Load()
    {
        ui_fileSystemArea = root.Q<VisualElement>("FileSystem");
        ui_fileGround = root.Q<VisualElement>("FileGround");
        ui_filePathGround = root.Q<VisualElement>("FilePathGround");
        ui_changeSizeButton = root.Q<Button>("ChangeSize");
    }

    private void Event_Load()
    {
        //AddFolderGround("Main");
        //AddFilePath("Main", () => FolderPathEvent("Main"));

        ui_changeSizeButton.clicked += () =>
        {
            OnOffFileSystem(0.25f);
        };
    }

    //private void FindLockQuestion()
    //{
    //    MemberProfile member = GameManager.Instance.chatSystem.FindMember(GameManager.Instance.chatHumanManager.nowHumanName);
    //    for (int i = 0; i < member.questions.Count; ++i)
    //    {
    //        if (member.questions[i].parent is ConditionNode)
    //        {
    //            Debug.Log(member.questions[i].parent.name);
    //            lockQuestions.Add(GameManager.Instance.chatSystem.ui_questionGround.ElementAt(i));
    //        }
    //    }
    //}

    private AskNode FindQuestion(VisualElement ask)
    {
        //Chapter chapter = chapterManager.FindChapter(chatSystem.FindMember(chatSystem.currentMemberName).chapterName);
        //List<LockAskAndReply> asks = chapter.lockAskAndReply;
        //for (int i = 0; i < chapter.lockAskAndReply.Count; ++i)
        //{
        //    if (chapter.lockAskAndReply[i].ask == ask.name)
        //        return chapter.lockAskAndReply[i];
        //}

        //return new LockAskAndReply();


        MemberProfile member = GameManager.Instance.chatSystem.FindMember(GameManager.Instance.chatHumanManager.nowHumanName);

        for (int i = 0; i < member.questions.Count; ++i)
        {
            Debug.Log(member.questions[i].askText.Trim() == ask.name.Trim());
            if (member.questions[i].askText.Trim() == ask.name.Trim())
            {
                Debug.Log("???곗꽑?띠럾???");
                return member.questions[i];
            }
        }

        return null;
    }

    private VisualElement FindMoveArea(Vector2 position)
    {
        VisualElement questions = GameManager.Instance.chatSystem.ui_questionGround;
        for (int i = 0; i < GameManager.Instance.chatSystem.ui_questionGround.childCount; ++i)
        {
            // lock question
            if (questions.ElementAt(i).Q<VisualElement>("LockIcon") != null)
            {
                if (questions.ElementAt(i).worldBound.Contains(position))
                {
                    return questions.ElementAt(i);
                }
            }
            else
            {
                Debug.Log(questions.ElementAt(i).childCount);
            }
        }

        return null;
    }

    private void LoadDragAndDrop(VisualElement file, Action action)
    {
        // drl!
        file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
        {
            var area = FindMoveArea(evt.mousePosition);
            //target.RemoveFromHierarchy();
            if (area == null)
                beforeSlot.Add(target);
            else
            {
                ConditionNode conditionNode = FindQuestion(area).parent as ConditionNode;
                Debug.Log(conditionNode);
                if (conditionNode != null)
                {
                    string fileName = file.Q<Label>("FileName").text;

                    Debug.Log(GameManager.Instance.fileManager.FindFile(fileName).fileName + " " + conditionNode.fileName);
                    Debug.Log(GameManager.Instance.fileManager.FindFile(fileName).fileName.Trim() == conditionNode.fileName.Trim());
                    if (GameManager.Instance.fileManager.FindFile(fileName).fileName.Trim() == conditionNode.fileName.Trim())
                    {
                        Debug.Log((conditionNode.childList[0] as AskNode).askText);
                        // remove this lockQuestion
                        area.parent.Remove(area);
                        //change from lockQustion to question
                        GameManager.Instance.chatSystem.InputQuestion(GameManager.Instance.chatSystem.FindMember(GameManager.Instance.chatHumanManager.nowHumanName).name,
                            true, conditionNode.childList[0] as AskNode);
                        GameManager.Instance.chatSystem.FindMember(GameManager.Instance.chatHumanManager.nowHumanName).questions.Add(conditionNode.childList[0] as AskNode);
                        beforeSlot.Add(target);
                    }
                    else
                        beforeSlot.Add(target);
                }
                else
                    Debug.Log("it's not found in questions(current AskNode list)");
            }
        },
        () => { action(); }
        ));
    }

    private FolderFile FindFolder(string name)
    {
        foreach (FolderFile folder in fileFolders)
        {
            if (folder.folderName == name)
                return folder;
        }

        return null;


        //FolderFile file = fileFolderList[name];
        //if (file != null)
        //    return file;
        //else
        //    return null;
    }

    public void AddFile(FileType fileType, string fileName, string fileParentName)
    {
        // find parentFolder
        FolderFile parentFolder = FindFolder(fileParentName);

        // if exist parenteFolder
        if (parentFolder != null)
        {
            VisualElement file;
            // register folder to parentFolder
            switch (fileType)
            {
                case FileType.FOLDER:
                {
                    FileSO folder = GameManager.Instance.fileManager.FindFile(fileName);
                    // create uxml
                    file = RemoveContainer(ux_folderFile.Instantiate());
                    // change file name
                    file.Q<Label>("FileName").text = folder.fileName;
                    // connection click event
                    file.Q<Button>("FileImage").clicked += () =>
                    {
                        GameManager.Instance.fileManager.FindFile(fileName).isRead = true;
                        // image check action
                        if (folder != null)
                            GameManager.Instance.fileManager.UnlockChat(folder.name);
                        if (GameManager.Instance.fileManager.FindFile(fileName).isRead == true)
                            file.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;
                        
                        // draw current foluder
                        DrawFile(folder.fileName);
                        // add current folder path
                        AddFilePath(folder.fileName);
                    };

                    fileFolders.Add(new FolderFile(fileName, fileParentName));
                    parentFolder.folderFiles.Add(file);
                }
                //fileFolderList.Add(fileName, new FolderFile(fileName));
                break;
                case FileType.IMAGE:
                {
                    FileSO image = GameManager.Instance.fileManager.FindFile(fileName);
                    // create uxml
                    file = RemoveContainer(ux_imageFile.Instantiate());
                    // change file name
                    file.Q<Label>("FileName").text = image.fileName;
                    // connection drag and drop & button click event
                    LoadDragAndDrop(file, () => { GameManager.Instance.imageSystem.OpenImage(file, image.fileName); });
                    parentFolder.imageFiles.Add(file);
                }
                break;
                case FileType.TEXT:
                {
                    FileSO text = GameManager.Instance.fileManager.FindFile(fileName);
                    // create uxml
                    file = RemoveContainer(ux_textFile.Instantiate());
                    // change file name
                    file.Q<Label>("FileName").text = text.fileName;
                    // connection drag and drop & button click event
                    LoadDragAndDrop(file, () => { GameManager.Instance.imageSystem.OpenText(file, text.fileName); });
                    parentFolder.textFiles.Add(file);
                }
                break;
            }
        }
        // if not exist parenteFolder
        else
        {
            Debug.Log(fileParentName + " this is null");
        }

        if (currentFolderName == "")
            currentFolderName = fileParentName;
        if (currentFolderName == fileParentName)
            DrawFile(currentFolderName);
    }

    // ===============================================
    // I will use this function, don't remove - tjfdk
    // ===============================================
    //private FolderFile CreateNewParent(FileType fileType, string fileParentName, string fileName)
    //{
    //    // file - parent
    //    // create new parentFolder
    //    FolderFile newParentFolder = new FolderFile(fileParentName, GameManager.Instance.fileManager.FindFile(fileParentName).fileParentName);

    //    // register parentFolder
    //    fileFolders.Add(newParentFolder);
    //    //fileFolderList.Add(newParentFolder.folderName, newParentFolder);

    //    // add file to newParentFolder
    //    switch (fileType)
    //    {
    //        case FileType.FOLDER:
    //            newParentFolder.folderFiles.Add(fileName);
    //            break;
    //        case FileType.IMAGE:
    //            newParentFolder.imageFiles.Add(fileName);
    //            break;
    //        case FileType.TEXT:
    //            newParentFolder.textFiles.Add(fileName);
    //            break;
    //    }
    //    // parent - super parent
    //    // find super parent name
    //    string superParentFolderName = GameManager.Instance.fileManager.FindFile(fileParentName).fileParentName;
    //    // find super parent
    //    FolderFile superParentFolder = FindFolder(superParentFolderName);
    //    // if superParent is exist, add newParentFolder to it's parent
    //    if (superParentFolder != null)
    //        superParentFolder.folderFiles.Add(newParentFolder.folderName);
    //    // if superParent isn't exist, add newParentFolder to it's new parent
    //    else
    //        CreateNewParent(FileType.FOLDER, superParentFolderName, fileParentName)
    //            .folderFiles.Add(newParentFolder.folderName);
    //    // return new parent
    //    return newParentFolder;
    //}

    public void DrawFile(string folderName)
    {
        // fileGround - current folder ground
        // fileFolders - current folder list
        // folderName - current folder name

        // change current folder
        currentFolderName = folderName;
        currentFileFolder = FindFolder(folderName);

        // all file remove of fileGround
        RemoveFile();

        // find current folder
        //currentFileFolder = fileFolderList[folderName];

        // folder isn't null
        if (currentFileFolder != null)
        {
            // create current folder's childen
            foreach (VisualElement folder in currentFileFolder.folderFiles)
                ui_fileGround.Add(folder);

            foreach (VisualElement image in currentFileFolder.imageFiles)
                ui_fileGround.Add(image);

            foreach (VisualElement text in currentFileFolder.textFiles)                
                ui_fileGround.Add(text);
        }
        else
            Debug.Log("current folder file null");
    }

    private void RemoveFile()
    {
        for (int i = ui_fileGround.childCount - 1; i >= 0; i--)
            ui_fileGround.RemoveAt(i);
    }

    private void AddFilePath(string pathName)
    {
        VisualElement filePath = RemoveContainer(ux_filePath.Instantiate());
        filePath.Q<Button>().text = pathName + "> ";
        filePath.Q<Button>().clicked += () => { FolderPathEvent(pathName); };
        ui_filePathGround.Add(filePath);
        filePathLisk.Push(pathName);
    }

    private void FolderPathEvent(string fileName)
    {
        while (true)
        {
            if (filePathLisk.Peek() == fileName)
                break;
            ui_filePathGround.RemoveAt(ui_filePathGround.childCount - 1);
            filePathLisk.Pop();
        }

        DrawFile(filePathLisk.Peek());
    }

    public void OnOffFileSystem(float during)
    {
        isFileSystemOpen = !isFileSystemOpen;

        if (changeFileSystemSizeDOT != null)
        {
            changeFileSystemSizeDOT.Complete();
            changeFileSystemSizeDOT = null;
        }

        if (isFileSystemOpen)
        {
            changeFileSystemSizeDOT = DOTween.To(() => ui_fileSystemArea.style.flexBasis.value.value, x =>
                ui_fileSystemArea.style.flexBasis = x, fileAreaSizeOn, during);
            ui_changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOn);
        }
        else
        {
            changeFileSystemSizeDOT = DOTween.To(() => ui_fileSystemArea.style.flexBasis.value.value, x =>
                ui_fileSystemArea.style.flexBasis = x, fileAreaSizeOff, during);
            ui_changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOff);
        }
    }
}