using ChatVisual;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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

    public bool isPathClick = false;

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
    public VisualElement ui_fileGround;
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

    private AskNode FindQuestion_t(VisualElement file, VisualElement ask)
    {
        // find current member
        MemberProfile member = GameManager.Instance.chatSystem.
            FindMember(GameManager.Instance.chatHumanManager.nowHumanName);

        // ask is exist question
        if (ask.parent.name == GameManager.Instance.chatSystem.ui_questionGround.name)
        {
            // for member's question
            for (int i = 0; i < member.questions.Count; ++i)
            {
                // get question's parent
                ConditionNode condition = member.questions[i].parent as ConditionNode;

                // file name
                string fileName = file.Q<Label>("FileName").text;
                // condition names
                string[] names = condition.fileName.Split('/');
                
                // for names
                foreach (string name in names)
                {
                    // name(condition) == fileName(file)
                    if (GameManager.Instance.fileManager.FindFile(name).fileName.Trim() == fileName.Trim())
                        return member.questions[i];
                }
            }
        }

        return null;
    }

    private VisualElement FindQuestion(VisualElement file, Vector2 position)
    {
        if (GameManager.Instance.chatSystem.ui_questionGround.worldBound.Contains(position))
            Debug.Log(GameManager.Instance.chatSystem.ui_questionGround.name + " : hit");
        VisualElement questions = GameManager.Instance.chatSystem.ui_questionGround;
        for (int i = 0; i < questions.childCount; ++i)
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


        if (GameManager.Instance.chatSystem.ui_questionGround.worldBound.Contains(position))
        {

        }
    }

    private void LoadDragAndDrop(VisualElement file, Action action)
    {
        // drl!
        file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
        {
            // questionGround 가져오고
            VisualElement questionGround = GameManager.Instance.chatSystem.ui_questionGround;
            // 만약 questionGround와 부딪혔다면
            if (questionGround.worldBound.Contains(evt.mousePosition))
            {
                // currnet member 가져오고
                MemberProfile member = GameManager.Instance.chatHumanManager.nowHuman;
                // current member의 question 다 돌고?
                for (int i = 0; i < member.questions.Count; ++i)
                {
                    // ask의 condition 가져오기
                    if (member.questions[i].parent is ConditionNode condition)
                    {
                        // file name 가져오고
                        string fileName = file.Q<Label>("FileName").text;
                        // condition names 가져와서
                        string[] names = condition.fileName.Split('/');

                        // 둘이 비교해
                        foreach (string name in names)
                        {
                            // name(condition) == fileName(file)
                            if (GameManager.Instance.fileManager.FindFile(name).fileName.Trim() == fileName.Trim())
                            {
                                Debug.Log(GameManager.Instance.fileManager.FindFile(name).fileName.Trim() + " " + fileName.Trim());
                                condition.is_Unlock = true;
                                // 해당 질문 visuaelelement ㅈ삭제하기
                                questionGround.RemoveAt(i);
                                //change from lockQustion to question - 질문으로 만드는 거
                                GameManager.Instance.chatSystem.InputQuestion(member.name, false, condition.childList[0] as AskNode);
                                // 질문 추가하기
                                member.questions.Add(condition.childList[0] as AskNode);

                                beforeSlot.Add(target);
                                return;
                            }
                        }

                        beforeSlot.Add(target);
                    }
                    else
                    {
                        beforeSlot.Add(target);
                        Debug.LogError("아무튼 오류임;");
                    }
                }
                
                //foreach (VisualElement question in questionGround.Children())
                //{
                //    if (question.)
                //}
            }
            else
                beforeSlot.Add(target);

            //var area = FindQuestion(file, evt.mousePosition);
            ////target.RemoveFromHierarchy();
            //if (area == null)
            //    beforeSlot.Add(target);
            //else
            //{
            //if (FindQuestion_t(file, area).parent is ConditionNode conditionNode)
            //    {
            //        // 컨디션 노드 열림
            //        conditionNode.is_Unlock = true;
            //        // remove this lockQuestion
            //        area.parent.Remove(area);
            //        //change from lockQustion to question
            //        GameManager.Instance.chatSystem.InputQuestion(GameManager.Instance.chatSystem.FindMember(GameManager.Instance.chatHumanManager.nowHumanName).name,
            //            false, conditionNode.childList[0] as AskNode);
            //        GameManager.Instance.chatSystem.FindMember(GameManager.Instance.chatHumanManager.nowHumanName).questions.Add(conditionNode.childList[0] as AskNode);
            //    }
            //    beforeSlot.Add(target);
            //}
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
                    LoadDragAndDrop(file, () => 
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
                    });

                    fileFolders.Add(new FolderFile(fileName, fileParentName));
                    parentFolder.folderFiles.Add(file);
                }
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
            {
                ui_fileGround.Add(folder);
            }

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
        filePath.Q<Button>().clicked += () => { FolderPathEvent(pathName); isPathClick = true; };
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
        isPathClick = true;

        Stack<string> pathName = new Stack<string>();
        string top = GameManager.Instance.fileSystem.FindFolder(folderName).parentFolderName;

        // all remove paths
        for (int i = ui_filePathGround.childCount - 1; i >= 0; i--)
            ui_filePathGround.RemoveAt(i);

        while (top != "Main")
        {
            pathName.Push(top);
            top = GameManager.Instance.fileSystem.FindFolder(top).parentFolderName;
        }

        AddFilePath("Main");
        while (pathName.Count > 0)
        {
            AddFilePath(pathName.Peek());
            pathName.Pop();
        }

        DrawFile(GameManager.Instance.fileSystem.FindFolder(folderName).parentFolderName);
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