using ChatVisual;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

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

    public FolderFile(string name, string parent)
    {
        folderName = name;
        parentFolderName = parent;
        folderFiles = new List<VisualElement>();
        textFiles = new List<VisualElement>();
        imageFiles = new List<VisualElement>();
    }
}

public class UIReader_FileSystem : MonoBehaviour
{
    static public UIReader_FileSystem Instance;

    [SerializeField]
    private float fileAreaSizeOn, fileAreaSizeOff;
    [SerializeField]
    private Texture2D changeSizeBtnOn, changeSizeBtnOff;
    public bool isFileSystemOpen;

    Tween changeFileSystemSizeDOT;


    // root
    VisualElement root;

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
        Instance = this;

        fileFolders = new List<FolderFile>();
        filePathLisk = new Stack<string>();
        lockQuestions = new List<VisualElement>();

        //UI_Reader.Instance.MinWidth = 500f;
        //UI_Reader.Instance.MinHeight = 500f;
        //UI_Reader.Instance.MaxWidth = 1800f;
        //UI_Reader.Instance.MaxHeight = 980f;
    }

    private void OnEnable()
    {
        UXML_Load();
        Event_Load();

        fileFolders.Add(new FolderFile("Main", "Main"));
        AddFilePath("Main");
    }

    private void UXML_Load()
    {
        root = GameObject.Find("Game").GetComponent<UIDocument>().rootVisualElement;

        ui_fileSystemArea = root.Q<VisualElement>("FileSystem");
        ui_fileGround = UIReader_Main.Instance.root.Q<VisualElement>("FileGround");
        ui_filePathGround = UIReader_Main.Instance.root.Q<VisualElement>("FilePathGround");
        ui_changeSizeButton = UIReader_Main.Instance.root.Q<Button>("ChangeSize");
    }

    private void Event_Load()
    {
        ui_changeSizeButton.clicked += () =>
        {
            OnOffFileSystem(0.25f);
        };
    }

    private AskNode FindQuestion(VisualElement ask)
    {
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
                //Debug.Log(conditionNode);
                if (conditionNode != null)
                {
                    //Debug.Log(GameManager.Instance.fileManager.FindFile(fileName).fileName + " " + conditionNode.fileName);
                    //Debug.Log(GameManager.Instance.fileManager.FindFile(fileName).fileName.Trim() == conditionNode.fileName.Trim());

                    string fileName = file.Q<Label>("FileName").text;
                    string[] names = conditionNode.fileName.Split('/');

                    foreach (string name in names)
                    {
                        if (GameManager.Instance.fileManager.FindFile(fileName).fileName.Trim() == name.Trim())
                        {
                            Debug.Log((conditionNode.childList[0] as AskNode).askText);
                            // 컨디션 노드 열림
                            conditionNode.is_Unlock = true;
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
                    file = UIReader_Main.Instance.RemoveContainer(ux_folderFile.Instantiate());
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
                    file = UIReader_Main.Instance.RemoveContainer(ux_imageFile.Instantiate());
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
                    file = UIReader_Main.Instance.RemoveContainer(ux_textFile.Instantiate());
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
        VisualElement filePath = UIReader_Main.Instance.RemoveContainer(ux_filePath.Instantiate());
        filePath.Q<Button>().text = pathName + "> ";
        filePath.Q<Button>().clicked += () => { FolderPathEvent(pathName); };
        ui_filePathGround.Add(filePath);
        filePathLisk.Push(pathName);
    }

    private void FolderPathEvent(string folderName)
    {
        while (true)
        {
            if (filePathLisk.Peek() == folderName)
                break;
            ui_filePathGround.RemoveAt(ui_filePathGround.childCount - 1);
            filePathLisk.Pop();
        }

        DrawFile(filePathLisk.Peek());
    }

    public void HighlightingFolderPathEvent(string folderName)
    {
        Debug.Log(folderName);

        Stack<string> pathName = new Stack<string>();
        string top = folderName;

        // all remove paths
        for (int i = ui_filePathGround.childCount - 1; i >= 0; i--)
            ui_filePathGround.RemoveAt(i);

        while (top != "Main")
        {
            pathName.Push(top);
            top = GameManager.Instance.fileSystem.FindFolder(top).parentFolderName;
        }

        Debug.Log("Main");
        AddFilePath("Main");
        while (pathName.Count > 0)
        {
            AddFilePath(pathName.Peek());
            Debug.Log(pathName.Peek());
            pathName.Pop();
        }

        DrawFile(GameManager.Instance.fileSystem.FindFolder(folderName).parentFolderName);
        Debug.Log(folderName);
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